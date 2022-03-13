namespace diplom.Models
{
    public class Exchange
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public Exchange(string? name)
        {
            Name = name;
        }
    }
}
