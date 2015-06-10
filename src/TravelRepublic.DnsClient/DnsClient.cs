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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using TravelRepublic.DnsClient.Parsers;
using TravelRepublic.DnsClient.Resolvers;

namespace TravelRepublic.DnsClient
{
    class DnsClient
        : IDnsClient
    {
        readonly IPEndPoint _serverEndPoint;
        readonly RequestBuilder _requestBuilder;
        readonly DnsPermission _dnsPermissions;
        readonly ResolverFactory _resolverFactory;
        readonly ResponseParser _responseParser;
        
        internal DnsClient(
            IPEndPoint serverEndPoint,
            RequestBuilder requestBuilder,
            ResolverFactory resolverFactory,
            ResponseParser responseParser)
        {
            _dnsPermissions = new DnsPermission(PermissionState.Unrestricted);
            _serverEndPoint = serverEndPoint;
            _requestBuilder = requestBuilder;
            _resolverFactory = resolverFactory;
            _responseParser = responseParser;
        }

        /// <PermissionSet>
        ///     <IPermission class="System.Net.DnsPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Response Query(string host, NsType queryType, NsClass queryClass, ProtocolType protocol)
        {
            // Do stack walk and Demand all callers have DnsPermission.
            _dnsPermissions.Demand();

            var query = _requestBuilder.Build(host, queryType, queryClass, protocol);

            // Connect to DNS server and get the record for the current server.

            byte[] responseBytes = null;

            var resolver = _resolverFactory.Get(protocol);
            responseBytes = resolver.Resolve(query, _serverEndPoint);

            Trace.Assert(responseBytes != null, "Failed to retrieve data from the remote DNS server.");

            var response = _responseParser.ParseResponse(responseBytes);

            return response;
        }
    }
}
