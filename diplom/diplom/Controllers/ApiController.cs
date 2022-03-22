using diplom.Data;
using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;
using ApiShare = Tinkoff.InvestApi.V1.Share;
using ApiCandle = Tinkoff.InvestApi.V1.HistoricCandle;
using Share = diplom.Models.Share;
using Candle = diplom.Models.Candle;
using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;
using diplom.Models;
using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.Collections;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diplom.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly diplomContext _context;
        private readonly InvestApiClient _investApi;
        private readonly IConfiguration _configuration;

        public ApiController(diplomContext context, InvestApiClient investApi, IConfiguration configuration) : base()
        {
            _context = context;
            _investApi = investApi;
            _configuration = configuration;
        }

        // GET: api/company
        [HttpGet("company/{id?}")]
        public async void UpdateCompanyInfo(int? id = null)
        {
            IQueryable<Company> companies = from c in _context.Companies select c;
            Company? company = companies.First(company => company.Id == id);
            if (company == null || company.Share == null)
                return;

            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("https://query1.finance.yahoo.com/v10/finance/quoteSummary/" + company.Share.Ticker + "?modules=" + string.Join(',', Company.ApiModulesParams));
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;
            JsonElement companyInfo = root.GetProperty("quoteSummary").GetProperty("result")[0];
            
            client.Dispose();
        }

        // GET: api/shares/update
        [HttpGet("shares/update/{id?}")]
        public async void UpdateShares(int? id = null)
        {
            ICollection<ApiShare> apiShares = new RepeatedField<ApiShare>();
            if (id == null)
            {
                SharesResponse sharesResponse = _investApi.Instruments.Shares();
                apiShares = sharesResponse.Instruments;
            }
            else
            {
                IQueryable<Share> shares = _context.Shares.Include(share => share.Candles);
                Share? share = shares.Any() ? shares.FirstOrDefault(shares => shares.Id == id) : null;
                if (share == null)
                    return;
                InstrumentRequest request = new InstrumentRequest();
                request.IdType = InstrumentIdType.Figi;
                request.Id = share.Figi;
                ShareResponse shareResponse = _investApi.Instruments.ShareBy(request);
                apiShares.Add(shareResponse.Instrument);
            }
            foreach (ApiShare apiShare in apiShares)
            {
                IQueryable<Exchange> exchanges = _context.Exchanges.Include(exchange => exchange.Shares);
                Exchange? exchange = exchanges.Any() ? exchanges.FirstOrDefault(exchange => exchange.Name == apiShare.Exchange) : null;
                if (exchange == null)
                {
                    exchange = new Exchange(apiShare.Exchange);
                    _context.Exchanges.Add(exchange);
                }

                IQueryable<Country> countries = _context.Countries.Include(country => country.Shares);
                Country? country = countries.Any() ? countries.FirstOrDefault(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk) : null;
                if (country == null)
                {
                    country = new Country(apiShare.CountryOfRiskName, apiShare.CountryOfRisk);
                    _context.Countries.Add(country);
                }

                IQueryable<Sector> sectors = _context.Sectors.Include(sector => sector.Shares);
                Sector? sector = sectors.Any() ? sectors.FirstOrDefault(sector => sector.Name == apiShare.Sector) : null;
                if (sector == null)
                {
                    sector = new Sector(apiShare.Sector);
                    _context.Sectors.Add(sector);
                }

                IQueryable<Share> shares = _context.Shares.Include(share => share.Candles);
                Share? share = shares.Any() ? shares.FirstOrDefault(shares => shares.Figi == apiShare.Figi) : null;
                if (share != null)
                {
                    share.Update(apiShare, exchange, country, sector);
                    new CandlesController(_context, _investApi, _configuration).GetCandles(share);
                    _context.Shares.Update(share);
                }
                else
                {
                    share = new Share(apiShare, exchange, country, sector);
                    new CandlesController(_context, _investApi, _configuration).GetCandles(share);
                    _context.Shares.Add(share);
                }

                IQueryable<Company> companies = _context.Companies;
                Company? company = companies.Any() ? companies.FirstOrDefault(company => company.Share != null && company.Share.Figi == share.Figi) : null;
                if (company != null)
                {
                    company.Update();
                    _context.Companies.Update(company);
                }
                else
                {
                    company = new Company(share);
                    _context.Companies.Add(company);
                }
            }

            await _context.SaveChangesAsync();
        }

        // GET: api/company/{id}/events
        [HttpGet("company/{id}/events")]
        public void UpdateCompanyEvents(int id)
        {

        }

        // GET: api
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
