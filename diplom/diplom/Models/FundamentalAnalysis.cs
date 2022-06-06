namespace diplom.Models
{
    public enum StockRecommendation
    {
        Buy,
        Sell,
        Hold,
        Undefined
    }

    public static class FundamentalAnalysis
    {
        private const double EnterpriseToEbitdaTurningPoint = 3;

        private const double DeptToEBITDAMaxTurningPoint = 2.5;
        private const double DeptToEBITDAMinTurningPoint = 2;

        private const double ForwardPETurningPoint = 5;

        private const double PriceToBookTurningPoint = 1;

        public static StockRecommendation EnterpriseValueToEBITDARatio(Share share)
        {
            if (share.Company == null || share.Company.EnterpriseToEbitda == null)
                return StockRecommendation.Undefined;

            if (share.Company.EnterpriseToEbitda <= EnterpriseToEbitdaTurningPoint)
                return StockRecommendation.Buy;
            else
                return StockRecommendation.Sell;
        }

        public static double DeptToEBITDARatioValue(Share share)
        {
            if (share.Company == null || share.Company.TotalDebt == null || share.Company.Ebitda == null)
                return double.NaN;

            return Math.Round((double)(double.Parse(share.Company.TotalDebt.ToString()) / share.Company.Ebitda));
        }

        public static StockRecommendation DeptToEBITDARatio(Share share)
        {
            if (share.Company == null || share.Company.TotalDebt == null || share.Company.Ebitda == null)
                return StockRecommendation.Undefined;

            double deptToEBITDARatio = DeptToEBITDARatioValue(share);
            if (deptToEBITDARatio <= 0 || deptToEBITDARatio >= DeptToEBITDAMaxTurningPoint)
                return StockRecommendation.Sell;
            if (deptToEBITDARatio < DeptToEBITDAMinTurningPoint)
                return StockRecommendation.Hold;
            else
                return StockRecommendation.Buy;
        }

        public static StockRecommendation ForwardPERatio(Share share)
        {
            if (share.Company == null || share.Company.ForwardPE == null)
                return StockRecommendation.Undefined;

           if (share.Company.ForwardPE <= 0 || share.Company.ForwardPE >= ForwardPETurningPoint)
                return StockRecommendation.Sell;
            else
                return StockRecommendation.Buy;
        }

        public static StockRecommendation GetSummaryRecommendation(Share share)
        {
            int result = 0;
            result += ParseRecommendationToInt(EnterpriseValueToEBITDARatio(share));
            result += ParseRecommendationToInt(DeptToEBITDARatio(share));
            result += ParseRecommendationToInt(ForwardPERatio(share));

            if (result > 0)
                return StockRecommendation.Buy;
            if (result < 0)
                return StockRecommendation.Sell;
            else
                return StockRecommendation.Hold;
        }

        private static int ParseRecommendationToInt(StockRecommendation recommendation)
        {
            switch (recommendation)
            {
                case StockRecommendation.Buy:
                    return 1;
                case StockRecommendation.Sell:
                    return -1;
                default:
                    return 0;
            }
        }
    }
}
