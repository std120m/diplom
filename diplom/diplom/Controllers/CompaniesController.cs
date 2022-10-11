#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using diplom.Data;
using diplom.Models;
using System.Text.Json;
using System.Globalization;
using diplom.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace diplom.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly diplomContext _context;

        public CompaniesController(diplomContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Website,Description,EnterpriseValue,ForwardPE,ProfitMargins,FloatShares,FullTimeEmployees,SharesOutstanding,SharesShort,SharesShortPriorMonth,ShortRatio,ShortPercentOfFloat,BookValuePerShare,PriceToBook,NetIncomeToCommon,TrailingEps,EnterpriseToRevenue,EnterpriseToEbitda,Week52Change,SandP52WeekChange,TotalCash,TotalCashPerShare,Ebitda,TotalDebt,CurrentRatio,Revenue,DebtToEquity,RevenuePerShare,ReturnOnAssets,ReturnOnEquity,GrossProfits,FreeCashflow,OperatingCashflow,RevenueGrowth,OperatingMargins")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Website,Description,EnterpriseValue,ForwardPE,ProfitMargins,FloatShares,FullTimeEmployees,SharesOutstanding,SharesShort,SharesShortPriorMonth,ShortRatio,ShortPercentOfFloat,BookValuePerShare,PriceToBook,NetIncomeToCommon,TrailingEps,EnterpriseToRevenue,EnterpriseToEbitda,Week52Change,SandP52WeekChange,TotalCash,TotalCashPerShare,Ebitda,TotalDebt,CurrentRatio,Revenue,DebtToEquity,RevenuePerShare,ReturnOnAssets,ReturnOnEquity,GrossProfits,FreeCashflow,OperatingCashflow,RevenueGrowth,OperatingMargins")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        public async Task UpdateCompany(Share share, Company company)
        {
            if (share == null)
                return;
            if ((company.Shares as List<Share>).IndexOf(share) < 0)
                company.Shares.Add(share);

            string result = String.Empty;
            HttpClient client = new HttpClient();
            await SetBrandInfo(company, share);
            try
            {
                result = await client.GetStringAsync("https://query1.finance.yahoo.com/v10/finance/quoteSummary/" + share.Ticker + "?modules=" + string.Join(',', Company.ApiModulesParams));
                // result = await client.GetStringAsync("https://www.tinkoff.ru/invest/stocks/AAPL/fundamentals/");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await SaveCompany(company);
                return;
            }
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;
            JsonElement companyInfo = root.GetProperty("quoteSummary").GetProperty("result")[0];
            company.Website = Helper.GetValueFromJson(companyInfo, "assetProfile.website");
            company.Description = Helper.GetValueFromJson(companyInfo, "assetProfile.longBusinessSummary");
            bool parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseValue.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long enterpriseValue);
            company.EnterpriseValue = parseResult ? enterpriseValue : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.forwardPE.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double forwardPE);
            company.ForwardPE = parseResult ? forwardPE : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.profitMargins.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double profitMargins);
            company.ProfitMargins = parseResult ? profitMargins : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.floatShares.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long floatShares);
            company.FloatShares = parseResult ? floatShares : null;
            parseResult = int.TryParse(Helper.GetValueFromJson(companyInfo, "assetProfile.fullTimeEmployees"), out int fullTimeEmployees);
            company.FullTimeEmployees = parseResult ? fullTimeEmployees : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesOutstanding.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long sharesOutstanding);
            company.SharesOutstanding = parseResult ? sharesOutstanding : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesShort.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long sharesShort);
            company.SharesShort = parseResult ? sharesShort : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesShortPriorMonth.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long sharesShortPriorMonth);
            company.SharesShortPriorMonth = parseResult ? sharesShortPriorMonth : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.shortRatio.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double shortRatio);
            company.ShortRatio = parseResult ? shortRatio : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.shortPercentOfFloat.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double shortPercentOfFloat);
            company.ShortPercentOfFloat = parseResult ? shortPercentOfFloat : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.bookValue.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double bookValuePerShare);
            company.BookValuePerShare = parseResult ? bookValuePerShare : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.priceToBook.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double priceToBook);
            company.PriceToBook = parseResult ? priceToBook : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.netIncomeToCommon.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long netIncomeToCommon);
            company.NetIncomeToCommon = parseResult ? netIncomeToCommon : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.trailingEps.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double trailingEps);
            company.TrailingEps = parseResult ? trailingEps : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseToRevenue.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double enterpriseToRevenue);
            company.EnterpriseToRevenue = parseResult ? enterpriseToRevenue : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseToEbitda.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double enterpriseToEbitda);
            company.EnterpriseToEbitda = parseResult ? enterpriseToEbitda : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.52WeekChange.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double week52Change);
            company.Week52Change = parseResult ? week52Change : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.SandP52WeekChange.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double sandP52WeekChange);
            company.SandP52WeekChange = parseResult ? sandP52WeekChange : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.totalCash.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long totalCash);
            company.TotalCash = parseResult ? totalCash : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.totalCashPerShare.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double totalCashPerShare);
            company.TotalCashPerShare = parseResult ? totalCashPerShare : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.ebitda.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long ebitda);
            company.Ebitda = parseResult ? ebitda : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.totalDebt.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long totalDebt);
            company.TotalDebt = parseResult ? totalDebt : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.currentRatio.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double currentRatio);
            company.CurrentRatio = parseResult ? currentRatio : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.totalRevenue.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long revenue);
            company.Revenue = parseResult ? revenue : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.debtToEquity.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double debtToEquity);
            company.DebtToEquity = parseResult ? debtToEquity : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.revenuePerShare.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double revenuePerShare);
            company.RevenuePerShare = parseResult ? revenuePerShare : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.returnOnAssets.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double returnOnAssets);
            company.ReturnOnAssets = parseResult ? returnOnAssets : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.returnOnEquity.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double returnOnEquity);
            company.ReturnOnEquity = parseResult ? returnOnEquity : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.grossProfits.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long grossProfits);
            company.GrossProfits = parseResult ? grossProfits : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.freeCashflow.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long freeCashflow);
            company.FreeCashflow = parseResult ? freeCashflow : null;
            parseResult = long.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.operatingCashflow.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out long operatingCashflow);
            company.OperatingCashflow = parseResult ? operatingCashflow : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.revenueGrowth.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double revenueGrowth);
            company.RevenueGrowth = parseResult ? revenueGrowth : null;
            parseResult = double.TryParse(Helper.GetValueFromJson(companyInfo, "financialData.operatingMargins.raw"), NumberStyles.Any, CultureInfo.InvariantCulture, out double operatingMargins);
            company.OperatingMargins = parseResult ? operatingMargins : null;

            await SaveCompany(company);
        }

        private async Task SaveCompany(Company company)
        {
            if (company.Id > 0)
                _context.Companies.Update(company);
            else
                _context.Companies.Add(company);

            await _context.SaveChangesAsync();
        }
        private async Task SetBrandInfo(Company company, Share share)
        {
            string result = String.Empty;
            HttpClient client = new HttpClient();
            try
            {
                result = await client.GetStringAsync("https://www.tinkoff.ru/api/trading/symbols/brands");

                JObject brandInfo = (JObject)JsonConvert.DeserializeObject(result);
                JToken brands = brandInfo.First.Next.First.First.First;

                foreach (JToken brand in brands)
                {
                    JToken tickers = brand["tickers"];
                    foreach (JToken ticker in tickers)
                    {
                        if (ticker.ToString() == share.Ticker)
                        {
                            company.BrandInfo = brand["brandInfo"].ToString();
                            company.Logo = "https://invest-brands.cdn-tinkoff.ru/"+brand["logoName"].ToString().Split('.')[0]+ "x640.png";
                            break;
                        }
                    }
                    if (company.BrandInfo != null)
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
