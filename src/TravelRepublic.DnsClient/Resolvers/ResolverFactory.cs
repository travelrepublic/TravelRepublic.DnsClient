using System;
using System.Net.Sockets;

namespace TravelRepublic.DnsClient.Resolvers
{
    class ResolverFactory
    {
        readonly TcpResolver _tcpResolver;
        readonly UdpResolver _udpResolver;

        public ResolverFactory(TcpResolver tcpResolver, UdpResolver udpResolver)
        {
            _tcpResolver = tcpResolver;
            _udpResolver = udpResolver;
        }

        public IResolver Get(ProtocolType protocol)
        {
            switch (protocol)
            {
                case ProtocolType.Tcp:
                {
                    return _tcpResolver;
                }
                case ProtocolType.Udp:
                {
                    return _udpResolver;
                }
                default:
                {
                    throw new InvalidOperationException("Invalid Protocol: " + protocol);
                }
            }            
        }
    }
}