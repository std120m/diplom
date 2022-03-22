namespace diplom.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Share> Shares { get; set; }

        public Sector(string? name)
        {
            Shares = new List<Share>();
            Name = name;
        }
    }
}
