using System.Globalization;
using System.Text.Json;

namespace diplom.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
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
        public Share? Share { get; set; }
        public ICollection<CompanyEvents> Events { get; set; }
        public ICollection<CompanyFilings> Filings { get; set; }

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
            Events = new List<CompanyEvents>();
            Filings = new List<CompanyFilings>();
        }
        public Company(Share share) : this()
        {
            Share = share;
            Update();
        }

        public async void Update()
        {
            if (Share == null)
                return;

            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("https://query1.finance.yahoo.com/v10/finance/quoteSummary/" + Share.Ticker + "?modules=" + string.Join(',', Company.ApiModulesParams));
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;
            JsonElement companyInfo = root.GetProperty("quoteSummary").GetProperty("result")[0];
            Website = companyInfo.GetProperty("assetProfile").GetProperty("website").ToString();
            Description = companyInfo.GetProperty("assetProfile").GetProperty("longBusinessSummary").ToString();
            EnterpriseValue = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseValue").GetProperty("raw").ToString());
            ForwardPE = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("forwardPE").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            ProfitMargins = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("profitMargins").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            FloatShares = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("floatShares").GetProperty("raw").ToString());
            FullTimeEmployees = int.Parse(companyInfo.GetProperty("assetProfile").GetProperty("fullTimeEmployees").ToString());
            SharesOutstanding = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesOutstanding").GetProperty("raw").ToString());
            SharesShort = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesShort").GetProperty("raw").ToString());
            SharesShortPriorMonth = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesShortPriorMonth").GetProperty("raw").ToString());
            ShortRatio = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("shortRatio").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            ShortPercentOfFloat = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("shortPercentOfFloat").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            BookValuePerShare = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("bookValue").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            PriceToBook = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("priceToBook").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            NetIncomeToCommon = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("netIncomeToCommon").GetProperty("raw").ToString());
            TrailingEps = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("trailingEps").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            EnterpriseToRevenue = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseToRevenue").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            EnterpriseToEbitda = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseToEbitda").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            Week52Change = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("52WeekChange").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            SandP52WeekChange = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("SandP52WeekChange").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            TotalCash = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalCash").GetProperty("raw").ToString());
            TotalCashPerShare = double.Parse(companyInfo.GetProperty("financialData").GetProperty("totalCashPerShare").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            Ebitda = long.Parse(companyInfo.GetProperty("financialData").GetProperty("ebitda").GetProperty("raw").ToString());
            TotalDebt = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalDebt").GetProperty("raw").ToString());
            CurrentRatio = double.Parse(companyInfo.GetProperty("financialData").GetProperty("currentRatio").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            Revenue = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalRevenue").GetProperty("raw").ToString());
            DebtToEquity = double.Parse(companyInfo.GetProperty("financialData").GetProperty("debtToEquity").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            RevenuePerShare = double.Parse(companyInfo.GetProperty("financialData").GetProperty("revenuePerShare").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            ReturnOnAssets = double.Parse(companyInfo.GetProperty("financialData").GetProperty("returnOnAssets").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            ReturnOnEquity = double.Parse(companyInfo.GetProperty("financialData").GetProperty("returnOnEquity").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            GrossProfits = long.Parse(companyInfo.GetProperty("financialData").GetProperty("grossProfits").GetProperty("raw").ToString());
            FreeCashflow = long.Parse(companyInfo.GetProperty("financialData").GetProperty("freeCashflow").GetProperty("raw").ToString());
            OperatingCashflow = long.Parse(companyInfo.GetProperty("financialData").GetProperty("operatingCashflow").GetProperty("raw").ToString());
            RevenueGrowth = double.Parse(companyInfo.GetProperty("financialData").GetProperty("revenueGrowth").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            OperatingMargins = double.Parse(companyInfo.GetProperty("financialData").GetProperty("operatingMargins").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
        }
    }
}
