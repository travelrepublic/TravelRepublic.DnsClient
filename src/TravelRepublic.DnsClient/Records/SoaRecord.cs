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
namespace TravelRepublic.DnsClient.Records
{
    public class SoaRecord
        : Record
    {
        readonly string _primaryNameServer;
        readonly string _responsiblePerson;
        readonly uint _serial;
        readonly uint _refreshInterval;
        readonly uint _retryInterval;
        readonly uint _expirationLimit;
        // RFC 1034: TTL - only positive values of a signed 32 bit number.
        readonly int _minimumTimeToLive;

        public SoaRecord(
            string primaryNameServer,
            string responsiblePerson,
            uint serial,
            uint refreshInterval,
            uint retryInterval,
            uint expirationLimit,
            int minimumTimeToLive,
            string answer
            )
            : base(answer)
        {
            _primaryNameServer = primaryNameServer;
            _responsiblePerson = responsiblePerson;
            _serial = serial;
            _refreshInterval = refreshInterval;
            _retryInterval = retryInterval;
            _expirationLimit = expirationLimit;
            _minimumTimeToLive = minimumTimeToLive;
        } 

        public string PrimaryNameServer
        {
            get { return _primaryNameServer; }
        }

        public string ResponsiblePerson
        {
            get { return _responsiblePerson; }
        }

        public uint Serial
        {
            get { return _serial; }
        }

        public uint RefreshInterval
        {
            get { return _refreshInterval; }
        }

        public uint RetryInterval
        {
            get { return _retryInterval; }
        }

        public uint ExpirationLimit
        {
            get { return _expirationLimit; }
        }

        public int MinimumTimeToLive
        {
            get { return _minimumTimeToLive; }
        }
    }
}