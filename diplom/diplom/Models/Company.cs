namespace diplom.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Symbol { get; set; }
        public string? longName { get; set; }
        public static string[] ApiModulesParams = new string[]
        {
            "modules=assetProfile",
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
            "sectorTrend"
        };
    }
}
