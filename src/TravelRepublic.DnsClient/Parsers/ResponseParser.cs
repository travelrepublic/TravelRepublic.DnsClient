/**********************************************************************
 * Copyright (c) 2010, j. montgomery                                  *
 * All rights reserved.                                               *
 *                                                                    *
 * Redistribution and use in source and binary forms, with or without *
 * modification, are permitted provided that the following conditions *
 * are met:                                                           *
 *                                                                    *
 * + Redistributions of source code must retain the above copyright   *
 *   notice, this list of conditions and the following disclaimer.    *
 *                                                                    *
 * + Redistributions in binary form must reproduce the above copyright*
 *   notice, this list of conditions and the following disclaimer     *
 *   in the documentation and/or other materials provided with the    *
 *   distribution.                                                    *
 *                                                                    *
 * + Neither the name of j. montgomery's employer nor the names of    *
 *   its contributors may be used to endorse or promote products      *
 *   derived from this software without specific prior written        *
 *   permission.                                                      *
 *                                                                    *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS*
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT  *
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS  *
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE     *
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,*
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES           *
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR *
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) *
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,*
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)      *
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED*
 * OF THE POSSIBILITY OF SUCH DAMAGE.                                 *
 **********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TravelRepublic.DnsClient.Records;

namespace TravelRepublic.DnsClient.Parsers
{
    // RFC 1034
    //
    // 4.1.1. Header section format
    // 
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                      ID                       |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    QDCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ANCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    NSCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    // |                    ARCOUNT                    |
    // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    class ResponseParser
    {
        readonly RecordNameParser _recordNameParser;
        readonly ParserFactory _parserFactory;

        public ResponseParser(
            RecordNameParser recordNameParser,
            ParserFactory parserFactory
            )
        {
            _recordNameParser = recordNameParser;
            _parserFactory = parserFactory;
        }

        public Response ParseResponse(byte[] recvBytes)
        {
            var memoryStream = new MemoryStream(recvBytes);
            var flagBytesBuffer = new byte[2];
            var transactionIdBuffer = new byte[2];
            var questionsBuffer = new byte[2];
            var answerRRsBuffer = new byte[2];
            var authorityRRsBuffer = new byte[2];
            var additionalRRCountBytesBuffer = new byte[2];
            var nsTypeBuffer = new byte[2];
            var nsClassBuffer = new byte[2];

            var bytesReceived = recvBytes.Length;

            // Parse DNS Response
            memoryStream.Read(transactionIdBuffer, 0, 2);
            memoryStream.Read(flagBytesBuffer, 0, 2);
            memoryStream.Read(questionsBuffer, 0, 2);
            memoryStream.Read(answerRRsBuffer, 0, 2);
            memoryStream.Read(authorityRRsBuffer, 0, 2);
            memoryStream.Read(additionalRRCountBytesBuffer, 0, 2);

            // Parse Header
            var transactionId = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(transactionIdBuffer, 0));
            var flags = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(flagBytesBuffer, 0));
            var queryResponse = (QueryResponse)(flags & (ushort)FlagMasks.QueryResponseMask);
            var opCode = (OpCode)(flags & (ushort)FlagMasks.OpCodeMask);
            var nsFlags = (NsFlags)(flags & (ushort)FlagMasks.NsFlagMask);
            var rCode = (RCode)(flags & (ushort)FlagMasks.RCodeMask);

            // Parse Questions Section
            var questions = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(questionsBuffer, 0));
            var answerRRs = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(answerRRsBuffer, 0));
            var authorityRRs = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(authorityRRsBuffer, 0));
            var additionalRRCount = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(additionalRRCountBytesBuffer, 0));
            var additionalRecords = new List<Record>();
            var answers = new Record[answerRRs];
            var authoritiveNameServers = new Record[authorityRRs];

            var name = _recordNameParser.ParseName(ref memoryStream);

            // Read dnsType
            memoryStream.Read(nsTypeBuffer, 0, 2);

            // Read dnsClass
            memoryStream.Read(nsClassBuffer, 0, 2);

            var nsType = (NsType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(nsTypeBuffer, 0));
            var nsClass = (NsClass)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(nsClassBuffer, 0));

            var headerParser = new RecordHeaderParser(_recordNameParser);
            
            // Read in Answer Blocks
            for (var i = 0; i < answerRRs; i++)
            {
                var header = headerParser.Parse(ref memoryStream);
                var parser = _parserFactory.Get(header.NsType);
                answers[i] = parser.ParseRecord(header, ref memoryStream);
            }

            // Parse Authority Records
            for (var i = 0; i < authorityRRs; i++)
            {
                var header = headerParser.Parse(ref memoryStream);
                var parser = _parserFactory.Get(header.NsType);
                authoritiveNameServers[i] = parser.ParseRecord(header, ref memoryStream);
            }

            // Parse Additional Records
            for (var i = 0; i < additionalRRCount; i++)
            {
                var header = headerParser.Parse(ref memoryStream);
                var parser = _parserFactory.Get(header.NsType);
                additionalRecords.Add(parser.ParseRecord(header, ref memoryStream));
            }

            return new Response(transactionId, flags, queryResponse, opCode, nsFlags, rCode, questions, answerRRs, authorityRRs, name, nsType, nsClass, additionalRecords, bytesReceived, answers, authoritiveNameServers);
        }
    }
}
