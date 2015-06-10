using System;
using System.Net;
using System.Net.Sockets;

namespace TravelRepublic.DnsClient.Resolvers
{
    class UdpResolver
        : IResolver
    {
        readonly int _timeout;

        public UdpResolver(int timeout)
        {
            _timeout = timeout;
        }

        public byte[] Resolve(byte[] query, IPEndPoint endPoint)
        {
            // UDP messages, data size = 512 octets or less
            byte[] receivedBytes = null;
            using (var udpClient = new UdpClient())
            {
                udpClient.Client.ReceiveTimeout = _timeout;
                udpClient.Connect(endPoint);
                udpClient.Send(query, query.Length);

                try
                {
                    receivedBytes = udpClient.Receive(ref endPoint);      
                }
                catch (SocketException exception)
                {
                    throw new InvalidOperationException(
                        String.Format("Can't connect to dns server at {0}", endPoint),
                        exception);
                }
            }
            return receivedBytes;
        }
    }
}