using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("World_News")]
    public class WorldNews
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public String Url { get; set; }
        public String Text { get; set; }
        public String Title { get; set; }
        public virtual ICollection<NewsQuotesImpact> NewsQuotesImpacts { get; set; } = new List<NewsQuotesImpact>();
        //public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

        public WorldNews()
        {
            Url = String.Empty;
            Text = String.Empty;
            Title = String.Empty;
        }

        public WorldNews(DateTime dateTime, string url, string text, string title)
        {
            DateTime = dateTime;
            Url = url;
            Text = text;
            Title = title;
        }
    }
}
