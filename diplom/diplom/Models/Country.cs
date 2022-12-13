using System.Text.Json.Serialization;

namespace diplom.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        [JsonIgnore]
        public virtual ICollection<Share> Shares { get; set; }

        public Country(string? name, string? code)
        {
            Shares = new List<Share>();
            Name = name;
            Code = code;
        }
    }
}
