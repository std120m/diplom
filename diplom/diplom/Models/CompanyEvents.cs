using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("Company_Events")]
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
