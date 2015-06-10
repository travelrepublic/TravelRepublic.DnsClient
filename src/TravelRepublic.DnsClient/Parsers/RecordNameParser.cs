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
using System.Diagnostics;
using System.IO;
using System.Text;
using TravelRepublic.DnsClient.Logging;

namespace TravelRepublic.DnsClient.Parsers
{
    // RFC 
    //        4.1.4. Message compression
    //
    // In order to reduce the size of messages, the domain system utilizes a
    // compression scheme which eliminates the repetition of domain names in a
    // message.  In this scheme, an entire domain name or a list of labels at
    // the end of a domain name is replaced with a pointer to a prior occurance
    // of the same name.
    //
    // The pointer takes the form of a two octet sequence:
    //
    //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //    | 1  1|                OFFSET                   |
    //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    //
    // The first two bits are ones.  This allows a pointer to be distinguished
    // from a label, since the label must begin with two zero bits because
    // labels are restricted to 63 octets or less.  (The 10 and 01 combinations
    // are reserved for future use.)  The OFFSET field specifies an offset from
    // the start of the message (i.e., the first octet of the ID field in the
    // domain header).  A zero offset specifies the first byte of the ID field,
    // etc.
    //
    // The compression scheme allows a domain name in a message to be
    // represented as either:
    //
    //   - a sequence of labels ending in a zero octet
    //   - a pointer
    //   - a sequence of labels ending with a pointer
    //
    class RecordNameParser
    {
        readonly ILogger _logger;

        public RecordNameParser(ILogger logger)
        {
            _logger = logger;
        }

        public string ParseName(ref MemoryStream ms)
        {
            _logger.Trace("Reading Name...");
            var sb = new StringBuilder();

            var next = (uint)ms.ReadByte();
            _logger.Trace("Next is 0x" + next.ToString("x2"));

            while ((next != 0x00))
            {
                // Isolate 2 most significat bits -> e.g. 11xx xxxx
                // if it's 0xc0 (11000000b} then pointer
                switch (0xc0 & next)
                {
                    // 0xc0 -> Name is a pointer.
                    case 0xc0:
                        {
                            // Isolate Offset
// ReSharper disable once InconsistentNaming
                            const int offsetMASK = ~0xc0;

                            // Example on how to calculate the offset
                            // e.g. 
                            // 
                            // So if given the following 2 bytes - 0xc1 0x1c (11000001 00011100)
                            //
                            //  The pointer takes the form of a two octet sequence:
                            //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                            //    | 1  1|                OFFSET                   |
                            //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                            //    | 1  1| 0  0  0  0  0  1  0  0  0  1  1  1  0  0|
                            //    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                            //
                            // A pointer is indicated by the a 1 in the two most significant bits
                            // The Offset is the remaining bits.
                            //
                            // The Pointer = 0xc0 (11000000 00000000)
                            // The offset = 0x11c (00000001 00011100)

                            // Move offset into the proper position
                            var offset = (int)(offsetMASK & next) << 8;

                            // extract the pointer to the data in the stream
                            int bPointer = ms.ReadByte() + offset;
                            // store the position so we can resume later
                            var oldPtr = ms.Position;
                            // Move to the specified position in the stream and 
                            // parse the name (recursive call)
                            ms.Position = bPointer;
                            sb.Append(ParseName(ref ms));
                            _logger.Trace(sb.ToString());
                            // Move back to original position, and continue
                            ms.Position = oldPtr;
                            next = 0x00;
                            break;
                        }
                    case 0x00:
                        {
                            Debug.Assert(next < 0xc0, "Offset cannot be greater then 0xc0.");
                            var buffer = new byte[next];
                            ms.Read(buffer, 0, (int)next);
                            sb.Append(Encoding.ASCII.GetString(buffer) + ".");
                            next = (uint)ms.ReadByte();
                            _logger.Trace("0x" + next.ToString("x2"));
                            break;
                        }
                    default:
                        throw new InvalidOperationException("There was a problem decompressing the DNS Message.");
                }
            }
            return sb.ToString();
        }
    }
}
