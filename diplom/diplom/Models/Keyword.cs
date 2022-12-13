using System.ComponentModel.DataAnnotations.Schema;

namespace diplom.Models
{
    [Table("Keywords")]
    public class Keyword
    {
        public long Id { get; set; }
        public string? Value { get; set; }

        public Keyword(string? value)
        {
            Value = value;
        }
    }
}
