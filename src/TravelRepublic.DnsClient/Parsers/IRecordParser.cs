using System.IO;
using TravelRepublic.DnsClient.Records;

namespace TravelRepublic.DnsClient.Parsers
{
    interface IRecordParser
    {
        Record ParseRecord(RecordHeader headerParser, ref MemoryStream ms);
    }
}