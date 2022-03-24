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
            company.Share = share;
            if (company.Share == null)
                return;

            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("https://query1.finance.yahoo.com/v10/finance/quoteSummary/" + company.Share.Ticker + "?modules=" + string.Join(',', Company.ApiModulesParams));
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;
            JsonElement companyInfo = root.GetProperty("quoteSummary").GetProperty("result")[0];
            company.Website = companyInfo.GetProperty("assetProfile").GetProperty("website").ToString();
            company.Description = companyInfo.GetProperty("assetProfile").GetProperty("longBusinessSummary").ToString();
            company.EnterpriseValue = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseValue").GetProperty("raw").ToString());
            company.ForwardPE = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("forwardPE").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.ProfitMargins = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("profitMargins").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.FloatShares = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("floatShares").GetProperty("raw").ToString());
            company.FullTimeEmployees = int.Parse(companyInfo.GetProperty("assetProfile").GetProperty("fullTimeEmployees").ToString());
            company.SharesOutstanding = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesOutstanding").GetProperty("raw").ToString());
            company.SharesShort = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesShort").GetProperty("raw").ToString());
            company.SharesShortPriorMonth = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("sharesShortPriorMonth").GetProperty("raw").ToString());
            company.ShortRatio = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("shortRatio").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.ShortPercentOfFloat = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("shortPercentOfFloat").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.BookValuePerShare = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("bookValue").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.PriceToBook = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("priceToBook").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.NetIncomeToCommon = long.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("netIncomeToCommon").GetProperty("raw").ToString());
            company.TrailingEps = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("trailingEps").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.EnterpriseToRevenue = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseToRevenue").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.EnterpriseToEbitda = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("enterpriseToEbitda").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.Week52Change = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("52WeekChange").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.SandP52WeekChange = double.Parse(companyInfo.GetProperty("defaultKeyStatistics").GetProperty("SandP52WeekChange").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.TotalCash = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalCash").GetProperty("raw").ToString());
            company.TotalCashPerShare = double.Parse(companyInfo.GetProperty("financialData").GetProperty("totalCashPerShare").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.Ebitda = long.Parse(companyInfo.GetProperty("financialData").GetProperty("ebitda").GetProperty("raw").ToString());
            company.TotalDebt = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalDebt").GetProperty("raw").ToString());
            company.CurrentRatio = double.Parse(companyInfo.GetProperty("financialData").GetProperty("currentRatio").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.Revenue = long.Parse(companyInfo.GetProperty("financialData").GetProperty("totalRevenue").GetProperty("raw").ToString());
            company.DebtToEquity = double.Parse(companyInfo.GetProperty("financialData").GetProperty("debtToEquity").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.RevenuePerShare = double.Parse(companyInfo.GetProperty("financialData").GetProperty("revenuePerShare").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.ReturnOnAssets = double.Parse(companyInfo.GetProperty("financialData").GetProperty("returnOnAssets").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.ReturnOnEquity = double.Parse(companyInfo.GetProperty("financialData").GetProperty("returnOnEquity").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.GrossProfits = long.Parse(companyInfo.GetProperty("financialData").GetProperty("grossProfits").GetProperty("raw").ToString());
            company.FreeCashflow = long.Parse(companyInfo.GetProperty("financialData").GetProperty("freeCashflow").GetProperty("raw").ToString());
            company.OperatingCashflow = long.Parse(companyInfo.GetProperty("financialData").GetProperty("operatingCashflow").GetProperty("raw").ToString());
            company.RevenueGrowth = double.Parse(companyInfo.GetProperty("financialData").GetProperty("revenueGrowth").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);
            company.OperatingMargins = double.Parse(companyInfo.GetProperty("financialData").GetProperty("operatingMargins").GetProperty("raw").ToString(), CultureInfo.InvariantCulture);

            if (company.Id > 0)
                _context.Companies.Update(company);
            else
                _context.Companies.Add(company);

            await _context.SaveChangesAsync();
        }
    }
}
