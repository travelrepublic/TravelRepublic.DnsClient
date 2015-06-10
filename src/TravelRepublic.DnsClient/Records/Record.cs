namespace TravelRepublic.DnsClient.Records
{
    public abstract class Record
    {
        readonly string _answer;

        protected Record(string answer)
        {
            _answer = answer;
        }

        public string Answer
        {
            get { return _answer; }
        }
    }
}