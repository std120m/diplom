namespace diplom.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public Sector(string? name)
        {
            Name = name;
        }
    }
}
