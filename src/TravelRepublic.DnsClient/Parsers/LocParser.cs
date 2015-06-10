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
    class LocParser
        : IRecordParser
    {
        private char[] _latDirection = new char[2] {'N', 'S'};
		private char[] _longDirection = new char[2] {'E', 'W'};

		public Record ParseRecord(RecordHeader header, ref MemoryStream ms) 
		{
			var latitudeBuffer = new byte[4];
            var longitudeBuffer = new byte[4];
            var altitudeBuffer = new byte[4];

			var version = (byte)ms.ReadByte();
			var size = (byte)ms.ReadByte();
			var horPrecision = (byte)ms.ReadByte();
			var vertPrecision = (byte)ms.ReadByte();

            ms.Read(latitudeBuffer, 0, latitudeBuffer.Length);
			// _latitude = Tools.ByteToUInt(latitude);
            var latitude = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(latitudeBuffer, 0));

            ms.Read(longitudeBuffer, 0, longitudeBuffer.Length);
			var longitude = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(longitudeBuffer, 0));


            ms.Read(altitudeBuffer, 0, altitudeBuffer.Length);
			// _altitude = Tools.ByteToUInt(altitude);
            var altitude = (uint)IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(altitudeBuffer, 0));
			
			var sb = new StringBuilder();
            sb.Append("Version: ");
            sb.Append(version);
            sb.Append("\r\n");

			sb.Append("Size: ");
            sb.Append(CalcSize(size));
            sb.Append(" m\r\n");

			sb.Append("Horizontal Precision: ");
            sb.Append(CalcSize(horPrecision));
            sb.Append(" m\r\n");

			sb.Append("Vertical Precision: ");
            sb.Append(CalcSize(vertPrecision));
            sb.Append(" m\r\n");
			
            sb.Append("Latitude: ");
            sb.Append(CalcLoc(latitude, _latDirection));
            sb.Append("\r\n");
			
            sb.Append("Longitude: ");
            sb.Append(CalcLoc(longitude, _longDirection));
            sb.Append("\r\n");
			
            sb.Append("Altitude: ");
            sb.Append((altitude - 10000000) / 100.0);
            sb.Append(" m\r\n");

            return new LocRecord(
                version,
                size,
                horPrecision,
                vertPrecision,
                latitude,
                longitude,
                altitude,
                sb.ToString());
		}

        static string CalcLoc(uint angle, char[] nsew) 
		{
			char direction;
			if (angle < 0x80000000) 
			{
				angle = 0x80000000 - angle;
				direction = nsew[1];
			} 
			else 
			{
				angle = angle - 0x80000000;
				direction = nsew[0];
			}
			
			var tsecs = angle % 1000;
			angle = angle / 1000;
			var secs = angle % 60;
			angle = angle / 60;
			var minutes = angle % 60;
			var degrees = angle / 60;
			
			return degrees + " deg, " + minutes + " min " + secs+ "." + tsecs + " sec " + direction;
		}

        // return size in meters
        static double CalcSize(byte val)
        {
            var size = (val & 0xF0) >> 4;
            var exponent = (val & 0x0F);
            while (exponent != 0)
            {
                size *= 10;
                exponent--;
            }
            return size / 100;
        }
    }
}
