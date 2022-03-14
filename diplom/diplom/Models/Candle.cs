using diplom.Helpers;
using Tinkoff.InvestApi.V1;
using ApiCandle = Tinkoff.InvestApi.V1.HistoricCandle;

namespace diplom.Models
{
    public class Candle
    {
        public long Id { get; set; }
        public double? Open { get; set; }
        public double? Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public DateTime Time { get; set; }
        public long? Volume { get; set; }
        public Share? Share { get; set; }

        public Candle() { }

        public Candle(ApiCandle apiCandle)
        {
            this.Update(apiCandle);
        }

        public void Update(ApiCandle apiCandle)
        {
            Open = Helper.ConvertQuotationToDouble(apiCandle.Open);
            Close = Helper.ConvertQuotationToDouble(apiCandle.Close);
            High = Helper.ConvertQuotationToDouble(apiCandle.High);
            Low = Helper.ConvertQuotationToDouble(apiCandle.Low);
            Time = apiCandle.Time.ToDateTime();
            Volume = apiCandle.Volume;
        }
    }
}
