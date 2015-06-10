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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TravelRepublic.DnsClient.Logging;

namespace TravelRepublic.DnsClient
{
    class RequestBuilder
    {
        readonly Random _random = new Random();
        readonly ILogger _logger;

        public RequestBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public byte[] Build(string host, NsType queryType, NsClass queryClass, ProtocolType protocol)
        {
            // Combind the NsFlags with our constant flags
            const ushort flags = (ushort)((ushort)QueryResponse.Query | (ushort)OpCode.QUERY | (ushort)NsFlags.RD);

            var bDnsQuery = GetMessageBytes(
                (ushort)_random.Next(),
                flags,
                1,
                0,
                0,
                host,
                queryType,
                queryClass);

            // Add two byte prefix that contains the packet length per RFC 1035 section 4.2.2
            if (protocol == ProtocolType.Tcp)
            {
                // 4.2.2. TCP usageMessages sent over TCP connections use server port 53 (decimal).  
                // The message is prefixed with a two byte length field which gives the message 
                // length, excluding the two byte length field.  This length field allows the 
                // low-level processing to assemble a complete message before beginning to parse 
                // it.
                var len = bDnsQuery.Length;
                Array.Resize<byte>(ref bDnsQuery, len + 2);
                Array.Copy(bDnsQuery, 0, bDnsQuery, 2, len);
                bDnsQuery[0] = (byte)((len >> 8) & 0xFF);
                bDnsQuery[1] = (byte)((len & 0xFF));
            }

            return bDnsQuery;
        }

        byte[] GetMessageBytes(
            ushort transactionId,
            ushort flags,
            ushort questions,
            ushort answerRRs,
            ushort authorityRRs,
            string name,
            NsType nsType,
            NsClass nsClass
            )
        {
            var memoryStream = new MemoryStream();
            var data = new byte[2];

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(transactionId) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(flags) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(questions) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(answerRRs) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(authorityRRs) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(0) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = CanonicaliseDnsName(name, false);
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder((ushort)nsType) >> 16));
            memoryStream.Write(data, 0, data.Length);

            data = BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder((ushort)nsClass) >> 16));
            memoryStream.Write(data, 0, data.Length);

            _logger.Trace("The message bytes: {0}", DumpArrayToString(memoryStream.ToArray()));

            return memoryStream.ToArray();
        }

        static byte[] CanonicaliseDnsName(string name, bool lowerCase)
        {
            if (!name.EndsWith("."))
            {
                name += ".";
            }

            if (name == ".")
            {
                return new byte[1];
            }

            var sb = new StringBuilder();

            sb.Append('\0');

            for (int i = 0, j = 0; i < name.Length; i++, j++)
            {
                if (lowerCase)
                {
                    sb.Append(char.ToLower(name[i]));
                }
                else
                {
                    sb.Append(name[i]);
                }

                if (name[i] == '.')
                {
                    sb[i - j] = (char)(j & 0xff);
                    j = -1;
                }
            }

            sb[sb.Length - 1] = '\0';

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        static string DumpArrayToString(byte[] bytes)
        {
            var builder = new StringBuilder();

            builder.Append("[");

            foreach (byte b in bytes)
            {
                builder.Append(" ");
                builder.Append((sbyte)b);
                builder.Append(" ");
            }

            builder.Append("]");

            return builder.ToString();
        }
    }
}
