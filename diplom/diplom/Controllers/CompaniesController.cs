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
            company.Website = Helper.GetValueFromJson(companyInfo, "assetProfile.website");
            company.Description = Helper.GetValueFromJson(companyInfo, "assetProfile.longBusinessSummary");
            company.EnterpriseValue = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseValue.raw"), CultureInfo.InvariantCulture);
            company.ForwardPE = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.forwardPE.raw"), CultureInfo.InvariantCulture);
            company.ProfitMargins = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.profitMargins.raw"), CultureInfo.InvariantCulture);
            company.FloatShares = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.floatShares.raw"), CultureInfo.InvariantCulture);
            company.FullTimeEmployees = int.Parse(Helper.GetValueFromJson(companyInfo, "assetProfile.fullTimeEmployees"));
            company.SharesOutstanding = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesOutstanding.raw"), CultureInfo.InvariantCulture);
            company.SharesShort = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesShort.raw"), CultureInfo.InvariantCulture);
            company.SharesShortPriorMonth = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.sharesShortPriorMonth.raw"), CultureInfo.InvariantCulture);
            company.ShortRatio = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.shortRatio.raw"), CultureInfo.InvariantCulture);
            company.ShortPercentOfFloat = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.shortPercentOfFloat.raw"), CultureInfo.InvariantCulture);
            company.BookValuePerShare = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.bookValue.raw"), CultureInfo.InvariantCulture);
            company.PriceToBook = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.priceToBook.raw"), CultureInfo.InvariantCulture);
            company.NetIncomeToCommon = long.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.netIncomeToCommon.raw"), CultureInfo.InvariantCulture);
            company.TrailingEps = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.trailingEps.raw"), CultureInfo.InvariantCulture);
            company.EnterpriseToRevenue = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseToRevenue.raw"), CultureInfo.InvariantCulture);
            company.EnterpriseToEbitda = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.enterpriseToEbitda.raw"), CultureInfo.InvariantCulture);
            company.Week52Change = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.52WeekChange.raw"), CultureInfo.InvariantCulture);
            company.SandP52WeekChange = double.Parse(Helper.GetValueFromJson(companyInfo, "defaultKeyStatistics.SandP52WeekChange.raw"), CultureInfo.InvariantCulture);
            company.TotalCash = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.totalCash.raw"), CultureInfo.InvariantCulture);
            company.TotalCashPerShare = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.totalCashPerShare.raw"), CultureInfo.InvariantCulture);
            company.Ebitda = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.ebitda.raw"), CultureInfo.InvariantCulture);
            company.TotalDebt = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.totalDebt.raw"), CultureInfo.InvariantCulture);
            company.CurrentRatio = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.currentRatio.raw"), CultureInfo.InvariantCulture);
            company.Revenue = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.totalRevenue.raw"), CultureInfo.InvariantCulture);
            company.DebtToEquity = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.debtToEquity.raw"), CultureInfo.InvariantCulture);
            company.RevenuePerShare = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.revenuePerShare.raw"), CultureInfo.InvariantCulture);
            company.ReturnOnAssets = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.returnOnAssets.raw"), CultureInfo.InvariantCulture);
            company.ReturnOnEquity = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.returnOnEquity.raw"), CultureInfo.InvariantCulture);
            company.GrossProfits = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.grossProfits.raw"), CultureInfo.InvariantCulture);
            company.FreeCashflow = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.freeCashflow.raw"), CultureInfo.InvariantCulture);
            company.OperatingCashflow = long.Parse(Helper.GetValueFromJson(companyInfo, "financialData.operatingCashflow.raw"), CultureInfo.InvariantCulture);
            company.RevenueGrowth = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.revenueGrowth.raw"), CultureInfo.InvariantCulture);
            company.OperatingMargins = double.Parse(Helper.GetValueFromJson(companyInfo, "financialData.operatingMargins.raw"), CultureInfo.InvariantCulture);

            if (company.Id > 0)
                _context.Companies.Update(company);
            else
                _context.Companies.Add(company);

            await _context.SaveChangesAsync();
        }
    }
}
