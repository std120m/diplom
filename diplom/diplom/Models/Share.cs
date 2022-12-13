using diplom.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using Tinkoff.InvestApi.V1;
using ApiShare = Tinkoff.InvestApi.V1.Share;

namespace diplom.Models
{
    public class Share
    {
        public int Id { get; set; }
        public string? Figi { get; set; }
        public string? Ticker { get; set; }
        public string? ClassCode { get; set; }
        public string? Currency { get; set; }
        public string? Name { get; set; }
        public virtual Exchange? Exchange { get; set; }
        public DateTime? IpoDate { get; set; }
        public long? IssueSize { get; set; }
        public long? IssuePlanSize { get; set; }
        public virtual Country? Country { get; set; }
        [JsonIgnore]
        public virtual Company? Company { get; set; }
        public int? CompanyId { get; set; }
        public virtual Sector? Sector { get; set; }
        public virtual ShareType? ShareType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Candle> Candles { get; set; } = new List<Candle>();

        public Share() 
        {
        }

        public Share(string? figi, string? ticker, string? classCode, string? currency, string? name, Exchange exchange, DateTime? ipoDate, long? issueSize, long? issuePlanSize, Country country, Sector sector, ShareType shareType) : this()
        {
            Figi = figi;
            Ticker = ticker;
            ClassCode = classCode;
            Currency = currency;
            Name = name;
            Exchange = exchange;
            IpoDate = ipoDate;
            IssueSize = issueSize;
            IssuePlanSize = issuePlanSize;
            Country = country;
            Sector = sector;
            ShareType = shareType;
        }

        public Share(ApiShare apiShare):this()
        {
            this.Update(apiShare);
        }
        public Share(ApiShare apiShare, Exchange exchange, Country country, Sector sector):this()
        {
            this.Update(apiShare, exchange, country, sector);
        }

        public void Update(ApiShare apiShare)
        {
            Figi = apiShare.Figi;
            Ticker = apiShare.Ticker;
            ClassCode = apiShare.ClassCode;
            Currency = apiShare.Currency;
            Name = apiShare.Name;
            ShareType = apiShare.ShareType;
            IssueSize = apiShare.IssueSize;
            IssuePlanSize = apiShare.IssueSizePlan;
            IpoDate = (apiShare.IpoDate == null ? null : apiShare.IpoDate.ToDateTime());
        }
        public void Update(ApiShare apiShare, Exchange exchange, Country country, Sector sector)
        {
            this.Exchange = exchange;
            this.Country = country;
            this.Sector = sector;
            this.Update(apiShare);
        }

        public object[] GetCandlesArray()
        {
            List<Dictionary<string, object>> candles = new List<Dictionary<string, object>>();

            foreach (Candle candle in this.Candles)
            {
                Dictionary<string, object> candleDetail = new Dictionary<string, object>();
                candleDetail.Add("low", candle.Low);
                candleDetail.Add("high", candle.High);
                candleDetail.Add("volume", candle.Volume);
                candleDetail.Add("date", candle.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                candleDetail.Add("open", candle.Open);
                candleDetail.Add("close", candle.Close);
                candleDetail.Add("share_id", candle.Share.Id);

                candles.Add(candleDetail);
            }

            return candles.ToArray();
        }

        public List<CandlesByDay> GetCandlesByDay(diplomContext context)
        {
            context.Database.ExecuteSqlRaw(@"
                DROP VIEW if exists View_CandlesByDay;
                CREATE VIEW View_CandlesByDay AS
                select
                    min(low) as low,
                    max(high) as high,
                    avg(volume) as volume,
                    DATE_FORMAT(Time, '%Y-%m-%d') as date,
                    ShareId,
                    CAST(substring_index(group_concat(cast(open as CHAR) order by Time asc), ',', 1) AS DECIMAL(9, 2)) as open,
                    CAST(substring_index(group_concat(cast(close as CHAR) order by Time desc), ',', 1) AS DECIMAL(9, 2)) as close
                from
                    candles
                group by
                    DATE_FORMAT(date, '%Y-%m-%d'), ShareId
                order by
                    date
                ;"
            );

            return context.CandlesByDay.Where(c => c.ShareId == this.Id).ToList();
        }
    }
}
