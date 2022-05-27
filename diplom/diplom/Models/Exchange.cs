namespace diplom.Models
{
    public class Exchange
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<Share> Shares { get; set; }

        public Exchange(string? name)
        {
            Shares = new List<Share>();
            Name = name;
        }
    }
}
