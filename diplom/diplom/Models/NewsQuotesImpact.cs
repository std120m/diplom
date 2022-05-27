using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("Company_World_News")]
    public class NewsQuotesImpact
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public int WorldNewsId { get; set; }
        public virtual WorldNews WorldNews { get; set; }

        public double Influence { get; set; }

        public NewsQuotesImpact()
        {
        }

        public NewsQuotesImpact(Company company, WorldNews worldNews, double influence)
        {
            Company = company;
            WorldNews = worldNews;
            Influence = influence;
        }
    }
}
