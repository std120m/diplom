using System.Text.Json.Serialization;

namespace diplom.Models
{
    public class SectorsStats
    {
        public double? It { get; set; }
        public double? Consumer { get; set; }
        public double? HealthCare { get; set; }
        public double? Financial { get; set; }
        public double? Industrials { get; set; }
        public double? Energy { get; set; }
        public double? Telecom { get; set; }
        public double? Other { get; set; }
        public double? Materials { get; set; }
        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }
    }
}
