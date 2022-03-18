namespace diplom.Models
{
    public class CompanyFilings
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
    }
}
