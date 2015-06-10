namespace TravelRepublic.DnsClient.Logging
{
    public interface ILogger
    {
        void Trace(string message, params object[] args);
    }
}
