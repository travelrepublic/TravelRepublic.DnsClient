using TravelRepublic.DnsClient.Parsers;

namespace TravelRepublic.DnsClient.Parsers
{
    class ParserFactory
    {
        readonly AParser _aParser;
        readonly AaaaParser _aaaaParser;
        readonly MxParser _mxParser;
        readonly RpParser _rpParser;
        readonly MrParser _mrParser;
        readonly MbParser _mbParser;
        readonly MgParser _mgParser;
        readonly NsParser _nsParser;
        readonly CNameParser _cNameParser;
        readonly PtrParser _ptrParser;
        readonly HInfoParser _hInfoParser;
        readonly MInfoParser _mInfoParser;
        readonly X25Parser _x25Parser;
        readonly TxtParser _txtParser;
        readonly LocParser _locParser;
        readonly SoaParser _soaParser;
        readonly SrvParser _srvParser;
        readonly AfsdbParser _afsdbParser;
        readonly AtmaParser _atmParser;
        readonly IsdnParser _isdnParser;
        readonly RtParser _rtParser;
        readonly WksParser _wksParser;
        readonly UnknownParser _unkowParser;

        public ParserFactory(
            AParser aParser, 
            AaaaParser aaaaParser, 
            MxParser mxParser, 
            RpParser rpParser,
            MrParser mrParser, 
            MbParser mbParser, 
            MgParser mgParser, 
            NsParser nsParser, 
            CNameParser cNameParser, 
            PtrParser ptrParser, 
            HInfoParser hInfoParser, 
            MInfoParser mInfoParser, 
            X25Parser x25Parser, 
            TxtParser txtParser, 
            LocParser locParser, 
            SoaParser soaParser, 
            SrvParser srvParser, 
            AfsdbParser afsdbParser, 
            AtmaParser atmParser, 
            IsdnParser isdnParser, 
            RtParser rtParser, 
            UnknownParser unkowParser, 
            WksParser wksParser)
        {
            _aParser = aParser;
            _aaaaParser = aaaaParser;
            _mxParser = mxParser;
            _rpParser = rpParser;
            _mrParser = mrParser;
            _mbParser = mbParser;
            _mgParser = mgParser;
            _nsParser = nsParser;
            _cNameParser = cNameParser;
            _ptrParser = ptrParser;
            _hInfoParser = hInfoParser;
            _mInfoParser = mInfoParser;
            _x25Parser = x25Parser;
            _txtParser = txtParser;
            _locParser = locParser;
            _soaParser = soaParser;
            _srvParser = srvParser;
            _afsdbParser = afsdbParser;
            _atmParser = atmParser;
            _isdnParser = isdnParser;
            _rtParser = rtParser;
            _unkowParser = unkowParser;
            _wksParser = wksParser;
        }

        public IRecordParser Get(NsType type)
        {
            switch (type)
            {
                case NsType.A:
                {
                    return _aParser;
                }
                case NsType.AAAA:
                {
                    return _aaaaParser;
                }
                case NsType.MX:
                {
                    return _mxParser;
                }
                case NsType.RP:
                {
                    return _rpParser;
                }
                case NsType.MR:
                {
                    return _mrParser;
                }
                case NsType.MB:
                {
                    return _mbParser;
                }
                case NsType.MG:
                {
                    return _mgParser;
                }
                case NsType.NS:
                {
                    return _nsParser;
                }
                case NsType.CNAME:
                {
                    return _cNameParser;
                }
                case NsType.PTR:
                {
                    return _ptrParser;
                }
                case NsType.HINFO:
                {
                    return _hInfoParser;
                }
                case NsType.MINFO:
                {
                    return _mInfoParser;
                }
                case NsType.X25:
                {
                    return _x25Parser;
                }
                case NsType.TXT:
                {
                    return _txtParser;
                }
                case NsType.LOC:
                {
                    return _locParser;
                }
                case NsType.SOA:
                {
                    return _soaParser;
                }
                case NsType.SRV:
                {
                    return _srvParser;
                }
                case NsType.AFSDB:
                {
                    return _afsdbParser;
                }
                case NsType.ATMA:
                {
                    return _atmParser;
                }
                case NsType.ISDN:
                {
                    return _isdnParser;
                }
                case NsType.RT:
                {
                    return _rtParser;
                }
                case NsType.WKS:
                {
                    return _wksParser;
                }
                default:
                {
                    return _unkowParser;
                }            
            }
        }
    }
}