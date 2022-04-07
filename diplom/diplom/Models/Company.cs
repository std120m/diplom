using System.Globalization;
using System.Text.Json;

namespace diplom.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? BrandInfo { get; set; }
        public long? EnterpriseValue { get; set; }
        public double? ForwardPE { get; set; }
        public double? ProfitMargins { get; set; }
        public long? FloatShares { get; set; }
        public int? FullTimeEmployees { get; set; }
        public long? SharesOutstanding { get; set; }
        public long? SharesShort { get; set; }
        public long? SharesShortPriorMonth { get; set; }
        public double? ShortRatio { get; set; }
        public double? ShortPercentOfFloat { get; set; }
        public double? BookValuePerShare { get; set; }
        public double? PriceToBook { get; set; }
        public long? NetIncomeToCommon { get; set; }
        public double? TrailingEps { get; set; }
        public double? EnterpriseToRevenue { get; set; }
        public double? EnterpriseToEbitda { get; set; }
        public double? Week52Change { get; set; }
        public double? SandP52WeekChange { get; set; }
        public long? TotalCash { get; set; }
        public double? TotalCashPerShare { get; set; }
        public long? Ebitda { get; set; }
        public long? TotalDebt { get; set; }
        public double? CurrentRatio { get; set; }
        public long? Revenue { get; set; }
        public double? DebtToEquity { get; set; }
        public double? RevenuePerShare { get; set; }
        public double? ReturnOnAssets { get; set; }
        public double? ReturnOnEquity { get; set; }
        public long? GrossProfits { get; set; }
        public long? FreeCashflow { get; set; }
        public long? OperatingCashflow { get; set; }
        public double? RevenueGrowth { get; set; }
        public double? OperatingMargins { get; set; }
        public ICollection<Share> Shares { get; set; }
        public ICollection<CompanyEvents> Events { get; set; }
        public ICollection<CompanyFilings> Filings { get; set; }
        public ICollection<NewsQuotesImpact> NewsQuotesImpacts { get; set; }
        public ICollection<WorldNews> WorldNews { get; set; }

        public static string[] ApiModulesParams = new string[]
        {
            "assetProfile",
            "defaultKeyStatistics",
            "financialData",
            "recommendationTrend",
            "upgradeDowngradeHistory",
            "majorHoldersBreakdown",
            "insiderHolders",
            "netSharePurchaseActivity",
            "earnings",
            "earningsHistory",
            "earningsTrend",
            "industryTrend",
            "indexTrend",
            "sectorTrend",
            "secFilings",
            "calendarEvents",
            "institutionOwnership"
        };

        public Company()
        {
            Shares = new List<Share>();
            Events = new List<CompanyEvents>();
            Filings = new List<CompanyFilings>();
            WorldNews = new List<WorldNews>();
            NewsQuotesImpacts = new List<NewsQuotesImpact>();
        }
    }
}
