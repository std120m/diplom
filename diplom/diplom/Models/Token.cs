namespace diplom.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string ClientToken { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Client? Client { get; set; }

        public Token(string clientToken, DateTime start, DateTime end)
        {
            ClientToken = clientToken;
            Start = start;
            End = end;
        }
    }
}
