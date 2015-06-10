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
using System.Text;
using TravelRepublic.DnsClient.Records;

namespace TravelRepublic.DnsClient.Parsers
{
    class SoaParser
        : IRecordParser
    {
        readonly RecordNameParser _parser;

        public SoaParser(RecordNameParser parser)
        {
            _parser = parser;
        }

        public Record ParseRecord(RecordHeader header, ref MemoryStream ms)
        {
            var sb = new StringBuilder();
            // Parse Name
            var primaryNameServer = _parser.ParseName(ref ms);
            sb.Append("Primary NameServer: ");
            sb.Append(primaryNameServer);
            sb.Append("\r\n");

            // Parse Responsible Persons Mailbox (Parse Name)
            var responsiblePerson = _parser.ParseName(ref ms);
            sb.Append("Responsible Person: ");
            sb.Append(responsiblePerson);
            sb.Append("\r\n");

            var serialBuffer = new byte[4];
            var refreshIntervalBuffer = new byte[4];
            var retryIntervalBuffer = new byte[4];
            var expirationLimitBuffer = new byte[4];
// ReSharper disable once InconsistentNaming
            var minTTLBuffer = new byte[4];

            // Parse Serial (4 bytes)
            ms.Read(serialBuffer, 0, 4);
            //_serial = Tools.ByteToUInt(serial);
            var serial = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(serialBuffer, 0));
            sb.Append("Serial: ");
            sb.Append(serial);
            sb.Append("\r\n");

            // Parse Refresh Interval (4 bytes)
            ms.Read(refreshIntervalBuffer, 0, 4);
            // _refreshInterval = Tools.ByteToUInt(refreshInterval);
            var refreshInterval = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(refreshIntervalBuffer, 0));
            sb.Append("Refresh Interval: ");
            sb.Append(refreshInterval);
            sb.Append("\r\n");

            // Parse Retry Interval (4 bytes)
            ms.Read(retryIntervalBuffer, 0, 4);
            //_retryInterval = Tools.ByteToUInt(retryInterval);
            var retryInterval = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(retryIntervalBuffer, 0));
            sb.Append("Retry Interval: ");
            sb.Append(retryInterval);
            sb.Append("\r\n");

            // Parse Expiration limit (4 bytes)
            ms.Read(expirationLimitBuffer, 0, 4);
            // _expirationLimit = Tools.ByteToUInt(expirationLimit);
            var expirationLimit = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(expirationLimitBuffer, 0));
            sb.Append("Expire: ");
            sb.Append(expirationLimit);
            sb.Append("\r\n");

            // Parse Min TTL (4 bytes)
            ms.Read(minTTLBuffer, 0, 4);
            // _minTTL = Tools.ByteToUInt(minTTL);
            var minimumTimeToLive = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(minTTLBuffer, 0));
            sb.Append("TTL: ");
            sb.Append(minimumTimeToLive);
            sb.Append("\r\n");

            return new SoaRecord(
                primaryNameServer, 
                responsiblePerson, 
                serial, 
                refreshInterval, 
                retryInterval, 
                expirationLimit, 
                minimumTimeToLive, 
                sb.ToString());
        }
    }
}
