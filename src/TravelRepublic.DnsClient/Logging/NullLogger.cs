namespace TravelRepublic.DnsClient.Logging
{
    public class NullLogger
        : ILogger
    {
        public void Trace(string message, params object[] args)
        {
        }
    }
}
