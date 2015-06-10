using System.Net;

namespace TravelRepublic.DnsClient.Resolvers
{
    interface IResolver
    {
        byte[] Resolve(byte[] query, IPEndPoint endPoint);
    }
}