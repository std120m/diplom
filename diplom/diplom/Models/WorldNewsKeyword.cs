using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("World_News_Keyword")]
    public class WorldNewsKeyword
    {
        public long Id { get; set; }
        public int WorldNewsId { get; set; }
        public virtual WorldNews WorldNews { get; set; }
        public long KeywordId { get; set; }
        public virtual Keyword Keyword { get; set; }

        public WorldNewsKeyword()
        {
        }

        public WorldNewsKeyword(WorldNews worldNews, Keyword keyword)
        {
            WorldNews = worldNews;
            Keyword = keyword;
        }
    }
}
