using System.Collections.Generic;
using TravelRepublic.DnsClient.Records;

namespace TravelRepublic.DnsClient
{
    public class Response
    {
        /// <summary>
        /// ID - A 16 bit identifier. This identifier is copied
        /// the corresponding reply and can be used by the requester
        /// to match up replies to outstanding queries.
        /// </summary>
        readonly ushort _transactionId;
        /// <summary>
        /// _flags will store a combination of the enums that make up the 16 bits after the 
        ///  TransactionID in the DNS protocol header
        /// </summary>
        readonly ushort _flags;
        /// <summary>
        /// A one bit field that specifies whether this message is a
        /// query (0), or a response (1).
        /// </summary>
        readonly QueryResponse _queryResponse;
        /// <summary>
        /// OPCODE - A four bit field that specifies kind of query in this
        /// message.  This value is set by the originator of a query
        /// and copied into the response.  
        /// </summary>
        readonly OpCode _opCode;
        /// <summary>
        ///  - A combination of flag fields in the DNS header (|AA|TC|RD|RA|)
        /// </summary>
        readonly NsFlags _nsFlags;
        /// <summary>
        /// Response code - this 4 bit field is set as part of
        /// responses only. 
        /// </summary>
        readonly RCode _rCode;
        /// <summary>
        /// QDCOUNT - an unsigned 16 bit integer specifying the number of
        /// entries in the question section.
        /// </summary>
        readonly ushort _questions;
        /// <summary>
        /// ANCOUNT - an unsigned 16 bit integer specifying the number of
        /// resource records in the answer section.
        /// </summary>
        readonly ushort _answerRRs;
        /// <summary>
        /// NSCOUNT - an unsigned 16 bit integer specifying the number of name
        /// server resource records in the authority records
        /// section.
        /// </summary>
        readonly ushort _authorityRRs;

        // RFC 1034
        // 4.1.2. Question section format
        //                                 1  1  1  1  1  1
        //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                                               |
        // /                     QNAME                     /
        // /                                               /
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                     QTYPE                     |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
        // |                     QCLASS                    |
        // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

        /// <summary>
        /// QNAME - a domain name represented as a sequence of labels, where
        /// each label consists of a length octet followed by that
        /// number of octets.  The domain name terminates with the
        /// zero length octet for the null label of the root.  Note
        /// that this field may be an odd number of octets; no
        /// padding is used
        /// </summary>
        readonly string _name;
        /// <summary>
        /// QTYPE - a two octet code which specifies the type of the query.
        /// The values for this field include all codes valid for a
        /// TYPE field, together with some more general codes which
        /// can match more than one type of RR.
        /// </summary>
        readonly NsType _nsType;
        /// <summary>
        /// QCLASS - a two octet code that specifies the class of the query.
        /// For example, the QCLASS field is IN for the Internet.
        /// </summary>
        readonly NsClass _nsClass;

        /// <summary>
        /// The additional records for the DNS Query
        /// </summary>
        readonly List<Record> _additionalRecords = new List<Record>();

        readonly Record[] _answers;
        readonly Record[] _authoritiveNameServers;
        readonly int _bytesReceived = 0;
            
        public Response(
            ushort transactionId, 
            ushort flags, 
            QueryResponse queryResponse, 
            OpCode opCode, 
            NsFlags nsFlags, 
            RCode rCode, 
            ushort questions, 
            ushort answerRRs, 
            ushort authorityRRs, 
            string name, 
            NsType nsType, 
            NsClass nsClass,
            List<Record> additionalRecords,
            int bytesReceived,
            Record[] answers,
            Record[] authoritiveNameServers
            )
        {
            _bytesReceived = bytesReceived;
            _answers = answers;
            _authoritiveNameServers = authoritiveNameServers;
            _transactionId = transactionId;
            _flags = flags;
            _queryResponse = queryResponse;
            _opCode = opCode;
            _nsFlags = nsFlags;
            _rCode = rCode;
            _questions = questions;
            _answerRRs = answerRRs;
            _authorityRRs = authorityRRs;
            _name = name;
            _nsType = nsType;
            _nsClass = nsClass;
            _additionalRecords = additionalRecords;
        }

        /// <summary>
        /// ID - A 16 bit identifier. This identifier is copied
        /// the corresponding reply and can be used by the requester
        /// to match up replies to outstanding queries.
        /// </summary>
        public ushort TransactionID
        {
            get { return _transactionId; }
        }

        /// <summary>
        /// A one bit field that specifies whether this message is a
        /// query (0), or a response (1).
        /// </summary>
        public QueryResponse QueryResponse
        {
            get { return _queryResponse; }
        }

        /// <summary>
        /// OPCODE - A four bit field that specifies kind of query in this
        /// message.  This value is set by the originator of a query
        /// and copied into the response.  
        /// </summary>
        public OpCode OpCode
        {
            get { return _opCode; }
        }

        /// <summary>
        /// NsFlags - A combination of flag fields in the DNS header (|AA|TC|RD|RA|)
        /// </summary>
        public NsFlags NsFlags
        {
            get { return _nsFlags; }
        }

        /// <summary>
        /// Response code - this 4 bit field is set as part of
        /// responses only. 
        /// </summary>
        public RCode RCode
        {
            get { return _rCode; }
        }

        /// <summary>
        /// QDCOUNT - an unsigned 16 bit integer specifying the number of
        /// entries in the question section.
        /// </summary>
        public ushort Questions
        {
            get { return _questions; }
        }

        /// <summary>
        /// ANCOUNT - an unsigned 16 bit integer specifying the number of
        /// resource records in the answer section.
        /// </summary>
        public ushort AnswerRRs
        {
            get { return _answerRRs; }
        }

        /// <summary>
        /// NSCOUNT - an unsigned 16 bit integer specifying the number of name
        /// server resource records in the authority records
        /// section.
        /// </summary>
        public ushort AuthorityRRs
        {
            get { return _authorityRRs; }
        }

        /// <summary>
        /// ARCOUNT - an unsigned 16 bit integer specifying the number of
        /// resource records in the additional records section.
        /// </summary>
        public ushort AdditionalRRs
        {
            get { return (ushort)_additionalRecords.Count; }
        }

        /// <summary>
        /// QNAME - a domain name represented as a sequence of labels, where
        /// each label consists of a length octet followed by that
        /// number of octets.  The domain name terminates with the
        /// zero length octet for the null label of the root.  Note
        /// that this field may be an odd number of octets; no
        /// padding is used
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// QTYPE - a two octet code which specifies the type of the query.
        /// The values for this field include all codes valid for a
        /// TYPE field, together with some more general codes which
        /// can match more than one type of RR.
        /// </summary>
        public NsType NsType
        {
            get { return _nsType; }
        }

        /// <summary>
        /// QCLASS - a two octet code that specifies the class of the query.
        /// For example, the QCLASS field is IN for the Internet.
        /// </summary>
        public NsClass NsClass
        {
            get { return _nsClass; }
        }

        public List<Record> AdditionalRRecords
        {
            get { return _additionalRecords; }
        }

        public Record[] Answers
        {
            get { return _answers; }
        }

        public Record[] AuthoritiveNameServers
        {
            get { return _authoritiveNameServers; }
        }

        public int BytesReceived
        {
            get { return _bytesReceived; }
        }
    }
}