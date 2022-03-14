using Tinkoff.InvestApi.V1;

namespace diplom.Helpers
{
    public static class Helper
    {
        public static double ConvertQuotationToDouble(Quotation quotation)
        {
            return quotation.Units + quotation.Nano / Math.Pow(10, 9);
        }
    }
}
