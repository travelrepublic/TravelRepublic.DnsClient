using System.Net.Sockets;

namespace TravelRepublic.DnsClient
{
    public interface IDnsClient
    {
        Response Query(
            string host, 
            NsType queryType, 
            NsClass queryClass, 
            ProtocolType protocol);
    }
}
