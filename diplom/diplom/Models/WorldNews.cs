using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("World_News")]
    public class WorldNews
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public ICollection<NewsQuotesImpact> NewsQuotesImpacts { get; set; } = new List<NewsQuotesImpact>();
        public ICollection<Company> Companies { get; set; } = new List<Company>();
    }
}
