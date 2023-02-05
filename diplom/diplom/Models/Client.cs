namespace diplom.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TokenId { get; set; }
        public Token? Token { get; set; }

        public Client(string name)
        {
            Name = name;
        }
    }
}
