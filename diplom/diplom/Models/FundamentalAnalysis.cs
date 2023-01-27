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
        private const double EnterpriseToEbitdaTurningPoint = 10;

        private const double DeptToEBITDAMaxTurningPoint = 2.5;
        private const double DeptToEBITDAMinTurningPoint = 2;

        private const double ForwardPETurningPoint = 15;

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

        public static StockRecommendation PriceToBookRation(Share share)
        {
            if (share.Company == null || share.Company.PriceToBook == null)
                return StockRecommendation.Undefined;

            if (share.Company.PriceToBook <= PriceToBookTurningPoint)
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
            result += ParseRecommendationToInt(PriceToBookRation(share));

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

        public static bool IsHighPotentialShare(Share share)
        {
            if (share.Company == null || share.Candles.ToList().Count() < 0)
                return false;


            var lastCandle = share.Candles.Last();
            var preLastCandleTime = lastCandle.Time.AddYears(-1);
            Candle preLastCandle = null;

            foreach (var candle in share.Candles.Reverse())
            {
                if (candle.Time <= preLastCandleTime)
                {
                    preLastCandle = candle;
                    break;
                }
            }

            if(preLastCandle == null)
            {
                return false;
            }

            var delta = Math.Round(((double)preLastCandle.Close - (double)lastCandle.Close) / (double)lastCandle.Close * 100, 2);
            return share.Company.EnterpriseValue >= 10000000000 && share.Company.ForwardPE >= 15 && delta >= 10;
        }

        public static bool IsStableGrowthShare(Share share)
        {
            if (share.Company == null || share.Candles.ToList().Count() < 0)
                return false;


            var lastCandle = share.Candles.Last();
            var preLastCandle = share.Candles.First();

            var delta = Math.Round(((double)preLastCandle.Close - (double)lastCandle.Close) / (double)lastCandle.Close * 100, 2);
            return share.Company.EnterpriseValue >= 10000000000
                && (share.Company.ForwardPE >= 15 && share.Company.ForwardPE <= 35)
                && share.Company.DebtToEquity <= 300
                && share.Company.ProfitMargins >= 0.05
                && delta >= 20;
        }

        public static bool IsFastGrowingProfitShare(Share share)
        {
            if (share.Company == null || share.Candles.ToList().Count() < 0)
                return false;


            var lastCandle = share.Candles.Last();
            var preLastCandle = share.Candles.First();

            var delta = Math.Round(((double)preLastCandle.Close - (double)lastCandle.Close) / (double)lastCandle.Close * 100, 2);
            return share.Company.ForwardPE >= 15
                && share.Company.DebtToEquity <= 300
                && share.Company.TrailingEps >= 15
                && delta >= 30;
        }
    }
}
