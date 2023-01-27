using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace diplom.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? BrandInfo { get; set; }
        //Стоимость компании
        public long? EnterpriseValue { get; set; }
        //это отношение текущей рыночной цены акций компании к ожидаемой будущей прибыли на акцию (Р/Е)
        public double? ForwardPE { get; set; }
        //Net Margin отражает насколько хорошо и эффективно компания управляет своими затратами.
        public double? ProfitMargins { get; set; }
        public long? FloatShares { get; set; }
        public int? FullTimeEmployees { get; set; }
        //размещенные акции
        public long? SharesOutstanding { get; set; }
        //акций в шорте
        public long? SharesShort { get; set; }
        //акций в шорте за последний месяц
        public long? SharesShortPriorMonth { get; set; }
        //процент шорта от всего объема
        public double? ShortRatio { get; set; }
        //процент шорта от акций в свободном доступе
        public double? ShortPercentOfFloat { get; set; }
        //капитал компании.
        public double? BookValuePerShare { get; set; }
        //отношению текущей рыночной капитализации компании к её балансовой стоимости
        public double? PriceToBook { get; set; }
        //Чистая прибыль, оставшуюся после вычета расходов, налогов и дивидендов на привилегированные акции
        public long? NetIncomeToCommon { get; set; }
        //Сумма компании прибыль на акцию за предыдущие четыре квартала.
        public double? TrailingEps { get; set; }
        //Коэффициент отношения ценности предприятия к доходу
        public double? EnterpriseToRevenue { get; set; }
        //стоимости компании к полученной ею прибыли до вычета процентов, налога на прибыль и амортизации активов
        public double? EnterpriseToEbitda { get; set; }
        public double? Week52Change { get; set; }
        //изменения выручки и цены за 52 недели
        public double? SandP52WeekChange { get; set; }
        //наличных актив
        public long? TotalCash { get; set; }
        //наличных актив на акцитю
        public double? TotalCashPerShare { get; set; }
        //рибыли до вычета процентов, налога на прибыль и амортизации активов
        public long? Ebitda { get; set; }
        //Обязательства 
        public long? TotalDebt { get; set; }
        //отношение оборотных активов компании к краткосрочным обязательствам
        public double? CurrentRatio { get; set; }
        //выручка
        public long? Revenue { get; set; }
        //соотношения заемного и собственного капитала компании
        public double? DebtToEquity { get; set; }
        public double? RevenuePerShare { get; set; }
        //эффективно использовать имеющиеся у нее активы для создания прибыли
        public double? ReturnOnAssets { get; set; }
        //доходность, на которую может рассчитывать инвестор
        public double? ReturnOnEquity { get; set; }
        //выручка - расходы на производство
        public long? GrossProfits { get; set; }
        //денежные средства компания после инвестиций на поддержание или расширение своей базы активов
        public long? FreeCashflow { get; set; }
        //выручка - операционные издержки
        public long? OperatingCashflow { get; set; }
        //Рост выручки
        public double? RevenueGrowth { get; set; }
        //сколько генерирует компания операционной прибыли на одну единицу выручки
        public double? OperatingMargins { get; set; }
        public virtual ICollection<Share> Shares { get; set; }
        public virtual ICollection<CompanyEvents> Events { get; set; }
        public virtual ICollection<CompanyFilings> Filings { get; set; }
        [JsonIgnore]
        public virtual ICollection<NewsQuotesImpact> NewsQuotesImpacts { get; set; }
        //public virtual ICollection<WorldNews> WorldNews { get; set; }
        public string? Logo{ get; set; }

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
            //WorldNews = new List<WorldNews>();
            NewsQuotesImpacts = new List<NewsQuotesImpact>();
        }
    }
}
