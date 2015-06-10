using TravelRepublic.DnsClient.Parsers;

namespace TravelRepublic.DnsClient
{
    public class RecordHeader
    {
        // NAME            an owner name, i.e., the name of the node to which this
        //                 resource record pertains.
        readonly string _name;
        // TYPE            two octets containing one of the RR TYPE codes.
        readonly NsType _nsType;
        // CLASS - two octets containing one of the RR CLASS codes.
        readonly NsClass _nsClass;
        // TTL - a 32 bit signed integer that specifies the time interval
        //       that the resource record may be cached before the source
        //       of the information should again be consulted.  Zero
        //       values are interpreted to mean that the RR can only be
        //       used for the transaction in progress, and should not be
        //       cached.  For example, SOA records are always distributed
        //       with a zero TTL to prohibit caching.  Zero values can
        ///      also be used for extremely volatile data.
        readonly int _timeToLive;
        // RDLENGTH - an unsigned 16 bit integer that specifies the length in
        //            octets of the RDATA field.
        readonly short _dataLength;

        /// <summary>
        /// Initalise the <see cref="RecordHeaderParser"/>
        /// </summary>
        /// <param name="name">The header name</param>
        /// <param name="nsType">The resource type</param>
        /// <param name="nsClass">The class type</param>
        /// <param name="timeToLive">The time to live</param>
        /// /// <param name="dataLength">Length of header data</param>
        public RecordHeader(
            string name, 
            NsType nsType, 
            NsClass nsClass, 
            int timeToLive,
            short dataLength)
        {
            _name = name;
            _nsType = nsType;
            _nsClass = nsClass;
            _timeToLive = timeToLive;
            _dataLength = dataLength;
        }

        /// <summary>
        /// NAME - an owner name, i.e., the name of the node to which this
        ///        resource record pertains.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// TYPE    two octets containing one of the RR TYPE codes.
        /// </summary>
        public NsType NsType
        {
            get { return _nsType; }
        }

        /// <summary>
        /// CLASS - two octets containing one of the RR CLASS codes.
        /// </summary>
        public NsClass NsClass
        {
            get { return _nsClass; }
        }

        /// <summary>
        /// TTL - a 32 bit signed integer that specifies the time interval
        ///       that the resource record may be cached before the source
        ///       of the information should again be consulted.  Zero
        ///       values are interpreted to mean that the RR can only be
        ///       used for the transaction in progress, and should not be
        ///       cached.  For example, SOA records are always distributed
        ///       with a zero TTL to prohibit caching.  Zero values can
        ///       also be used for extremely volatile data.
        /// </summary>
        public int TimeToLive
        {
            get { return _timeToLive; }
        }

        /// <summary>
        /// RDLENGTH - an unsigned 16 bit integer that specifies the length in
        ///            octets of the RDATA field.
        /// </summary>
        public short DataLength
        {
            get { return _dataLength; }
        }        
    }
}