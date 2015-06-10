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
using TravelRepublic.DnsClient.Records;

namespace TravelRepublic.DnsClient.Parsers
{
    class WksParser
        : IRecordParser
    {
        public Record ParseRecord(RecordHeader header, ref MemoryStream ms)
        {
            // Bit map is the data length minus the IpAddress (4 bytes) and the Protocol (1 byte), 5 bytes total.
            var bitMapLen = header.DataLength - 4 - 1;
            var ipAddressBuffer = new byte[4];
            var bitMapBuffer = new byte[bitMapLen];

            ms.Read(ipAddressBuffer, 0, 4);
            // _ipAddress = new IPAddress(Tools.ToUInt32(ipAddr, 0));
            var ipAddress = new IPAddress((uint)IPAddress.NetworkToHostOrder(BitConverter.ToUInt32(ipAddressBuffer, 0)));
            var protocolType = (ProtocolType)ms.ReadByte();
            ms.Read(bitMapBuffer, 0, bitMapBuffer.Length);
            var ports = GetKnownServices(bitMapBuffer);
            var answer = protocolType + ": " + GetServByPort(ports, protocolType);

            return new WksRecord(protocolType, ipAddress, ports, answer);
        }

        static short[] GetKnownServices(byte[] bitMap)
        {
            var tempPortArr = new short[1024];
            var portCount = 0;
            // mask to isolate left most bit
            const byte mask = 0x80;
            // Iterate through each byte
            for (var i = 0; i < bitMap.Length; i++)
            {
                var currentByte = bitMap[i];
                var count = 0;
                // iterate through each bit
                for (byte j = 0x07; j != 0xFF; j--)
                {
                    var port = (((i * 8) + count++) + 1);
                    currentByte = (byte)(currentByte << 1);
                    // is the flag set?
                    if ((mask & currentByte) == 0x80)
                    {
                        tempPortArr[portCount] = (short)port;
                        portCount++;
                    }
                }
            }
            var portArr = new short[portCount];
            Array.Copy(tempPortArr, 0, portArr, 0, portCount);
            return portArr;
        }

        /// <summary>
        /// Look up the port names for the given array of port numbers.
        /// </summary>
        /// <param name="port">An array of port numbers.</param>
        /// <param name="proto">The protocol type. (TCP or UPD)</param>
        /// <returns>The name of the port.</returns>
        static string GetServByPort(short[] port, ProtocolType proto)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < port.Length; i++)
            {
                sb.Append(GetServByPort(port[i], proto));
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// Look up the port name for any given port number.
        /// </summary>
        /// <param name="port">The port number.</param>
        /// <param name="proto">The protocol type. (TCP or UPD)</param>
        /// <returns>The name of the port.</returns>
        static string GetServByPort(short port, ProtocolType proto)
        {
            var ans = new StringBuilder();

            switch (proto)
            {
                case ProtocolType.Tcp:
                    {
                        var tcps = (TcpServices)port;
                        ans.Append(tcps);
                        ans.Append("(");
                        ans.Append(port);
                        ans.Append(")");
                        break;
                    }
                case ProtocolType.Udp:
                    {
                        var udps = (UdpServices)port;
                        ans.Append(udps);
                        ans.Append("(");
                        ans.Append(port);
                        ans.Append(")");
                        break;
                    }
                default:
                    {
                        ans.Append("(");
                        ans.Append(port);
                        ans.Append(")");
                        break;
                    }
            }
            return ans.ToString();
        }

        /// <summary>
        /// Defines Well Known TCP Ports for Services
        /// </summary>
        enum TcpServices : short
        {
            /// <summary>
            /// Domain Name Server Port
            /// </summary>
            Domain = 53
        }

        /// <summary>
        /// Defines Well Known UDP Ports for Services
        /// </summary>
        enum UdpServices : short
        {
            /// <summary>
            /// Domain Name Server Protocol Port
            /// </summary>
            Domain = 53
        }
    }
}
