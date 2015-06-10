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

namespace TravelRepublic.DnsClient.Parsers
{
    /// <summary>
    /// The DnsRecordHeader class contains fields, properties and 
    /// parsing cababilities within the DNS Record except the the 
    /// RDATA.  The Name, Type, Class, TTL, and RDLength.  
    ///
    /// This class is used in the DnsRecordFactory to instantiate 
    /// concrete DnsRecord Classes.
    /// 
    /// RFC 1035
    /// 
    /// 3.2.1. Format
    /// 
    /// All RRs have the same top level format shown below:
    /// 
    ///                                     1  1  1  1  1  1
    ///       0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    ///     |                                               |
    ///     /                                               /
    ///     /                      NAME                     /
    ///     |                                               |
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    ///     |                      TYPE                     |
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    ///     |                     CLASS                     |
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    ///     |                      TTL                      |
    ///     |                                               |
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    ///     |                   RDLENGTH                    |
    ///     +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
    ///  
    /// where:
    /// 
    /// NAME            an owner name, i.e., the name of the node to which this
    ///                 resource record pertains.
    /// 
    /// TYPE            two octets containing one of the RR TYPE codes.
    /// 
    /// CLASS           two octets containing one of the RR CLASS codes.
    /// 
    /// TTL             a 32 bit signed integer that specifies the time interval
    ///                 that the resource record may be cached before the source
    ///                 of the information should again be consulted.  Zero
    ///                 values are interpreted to mean that the RR can only be
    ///                 used for the transaction in progress, and should not be
    ///                 cached.  For example, SOA records are always distributed
    ///                 with a zero TTL to prohibit caching.  Zero values can
    ///                 also be used for extremely volatile data.
    /// 
    /// RDLENGTH        an unsigned 16 bit integer that specifies the length in
    ///                 octets of the RDATA field.
    /// 
    /// </summary>
    public class RecordHeaderParser
    {
        readonly RecordNameParser _parser;

        internal RecordHeaderParser(RecordNameParser parser)
        {
            _parser = parser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ms"></param>
        public RecordHeader Parse(ref MemoryStream ms)
        {
            var nsTypeBuffer = new byte[2];
            var nsClassBuffer = new byte[2];
            var nsTtlBuffer = new byte[4];
            var nsDataLengthBuffer = new byte[2];

            // Read the name
            var name = _parser.ParseName(ref ms);

            // Read the data header
            ms.Read(nsTypeBuffer, 0, 2);
            ms.Read(nsClassBuffer, 0, 2);
            ms.Read(nsTtlBuffer, 0, 4);
            ms.Read(nsDataLengthBuffer, 0, 2);

            var nsType = (NsType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(nsTypeBuffer, 0));
            var nsClass = (NsClass)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(nsClassBuffer, 0));

            var timeToLive = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(nsTtlBuffer, 0));
            var dataLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(nsDataLengthBuffer, 0));

            return new RecordHeader(name, nsType, nsClass, timeToLive, dataLength);
        }
    }
}
