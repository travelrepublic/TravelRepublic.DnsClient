﻿/**********************************************************************
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
namespace TravelRepublic.DnsClient.Records
{
    public class SrvRecord
        : Record
    {
        readonly ushort _priority;
        readonly ushort _weight;
        readonly ushort _port;
        readonly string _hostName;

        public SrvRecord(
            ushort priority,
            ushort weight,
            ushort port,
            string hostName,
            string answer
            )
            : base (answer)
        {
            _priority = priority;
            _weight = weight;
            _port = port;
            _hostName = hostName;
        }

        public ushort Priority
        {
            get { return _priority; }
        }
        public ushort Weight
        {
            get { return _weight; }
        }

        public ushort Port
        {
            get { return _port; }
        }

        public string HostName
        {
            get { return _hostName; }
        }        
    }
}