namespace diplom.Models
{
    public class CompanyEvents
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? Name { get; set; }

        public CompanyEvents(string? name, DateTime date)
        {
            Name = name;
            Date = date;
        }
    }
}
