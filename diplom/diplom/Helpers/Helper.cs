using System.Net;
using System.Text;
using System.Text.Json;
using Tinkoff.InvestApi.V1;

namespace diplom.Helpers
{
    public static class Helper
    {
        public static Dictionary<string, string> MonthToNumber = new Dictionary<string, string>()
        {
            { "января", "01" },
            { "февраля", "02" },
            { "марта", "03" },
            { "апреля", "04" },
            { "мая", "05" },
            { "июня", "06" },
            { "июля", "07" },
            { "августа", "08" },
            { "сентября", "09" },
            { "октября", "10" },
            { "ноября", "11" },
            { "декабря", "12" }
        };

        public static double ConvertQuotationToDouble(Quotation quotation)
        {
            return quotation.Units + quotation.Nano / Math.Pow(10, 9);
        }

        public static string GetValueFromJson(JsonElement json, string path)
        {
            string[] propertyNames = path.Split('.');

            JsonElement result = json;
            foreach (string propertyName in propertyNames)
            {
                result = result.GetProperty(propertyName);
            }

            return result.ToString();
        }

        public static bool IsCurrentYear(DateTime date)
        {
            return date.Year == DateTime.Now.Year;
        }

        public static string GetStringFromHtml(string url, Encoding encoding)
        {
            byte[] htmlData = new byte[0];
            using (WebClient client = new WebClient())
            {
                client.Encoding = encoding;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                try
                {
                    htmlData = client.DownloadData(url);
                    Console.WriteLine($"{url} --- was parsed");
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error --- {e}");

                    Thread.Sleep(15000);
                    GetStringFromHtml(url, encoding);
                }
                return encoding.GetString(htmlData);
            }
        }
    }
}
