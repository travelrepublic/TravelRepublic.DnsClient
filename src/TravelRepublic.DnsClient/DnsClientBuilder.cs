using System.Net;
using TravelRepublic.DnsClient.Logging;
using TravelRepublic.DnsClient.Parsers;
using TravelRepublic.DnsClient.Resolvers;

namespace TravelRepublic.DnsClient
{
    public class DnsClientBuilder
    {
        IPEndPoint _serverEndPoint;
        ILogger _logger = new NullLogger();
        int _timeout = 5000;

        public DnsClientBuilder WithDnsServer(IPEndPoint endPoint)
        {
            _serverEndPoint = endPoint;
            return this;
        }

        public DnsClientBuilder WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public DnsClientBuilder WithTimeout(int timeout)
        {
            _timeout = timeout;
            return this;
        }

        public IDnsClient Build()
        {
            var requestBuilder = new RequestBuilder(_logger);

            var recordTextParser = new RecordTextParser();
            var recordParser = new RecordNameParser(new NullLogger());
            
            var parserFactory = new ParserFactory(
                new AParser(),
                new AaaaParser(),
                new MxParser(recordParser),
                new RpParser(recordParser),
                new MrParser(),
                new MbParser(),
                new MgParser(),
                new NsParser(),
                new CNameParser(recordParser),
                new PtrParser(),
                new HInfoParser(recordTextParser),
                new MInfoParser(recordParser),
                new X25Parser(recordTextParser),
                new TxtParser(recordTextParser),
                new LocParser(),
                new SoaParser(recordParser),
                new SrvParser(recordParser),
                new AfsdbParser(recordParser),
                new AtmaParser(),
                new IsdnParser(recordTextParser),
                new RtParser(recordParser),
                new UnknownParser(),
                new WksParser());

            var resolverFactory = new ResolverFactory(
                new TcpResolver(_timeout),
                new UdpResolver(_timeout));

            var responseParser = new ResponseParser(
                new RecordNameParser(_logger),
                parserFactory);

            return new DnsClient(
                _serverEndPoint,
                requestBuilder,
                resolverFactory,
                responseParser);
        }
    }
}