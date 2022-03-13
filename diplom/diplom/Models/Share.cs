using diplom.Data;
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
        public Exchange? Exchange { get; set; }
        public DateTime? IpoDate { get; set; }
        public long? IssueSize { get; set; }
        public long? IssuePlanSize { get; set; }
        public Country? Country { get; set; }
        public Sector? Sector { get; set; }
        public ShareType ShareType { get; set; }

        public Share() { }

        public Share(string? figi, string? ticker, string? classCode, string? currency, string? name, Exchange? exchange, DateTime? ipoDate, long? issueSize, long? issuePlanSize, Country? country, Sector? sector, ShareType shareType)
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

        public Share(ApiShare apiShare)
        {
            this.Update(apiShare);
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
            IpoDate = apiShare.IpoDate.ToDateTime();
        }
    }
}
