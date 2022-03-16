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
        [HttpGet("company/{figi?}")]
        public async void GetCompanyInfo(string figi)
        {
            IQueryable<Share> shares = from s in _context.Shares select s;
            Share? share = shares.First(shares => shares.Figi == figi);
            
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync("https://query1.finance.yahoo.com/v10/finance/quoteSummary/" + share.Ticker + "?modules=" + string.Join(',', Company.ApiModulesParams));
            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement root = doc.RootElement;
            client.Dispose();
        }

        // GET: api/shares/update
        [HttpGet("shares/update/{id?}")]
        public void UpdateShares(int? id = null)
        {
            if (id == null)
            {
                SharesResponse sharesResponse = _investApi.Instruments.Shares();
                foreach (ApiShare apiShare in sharesResponse.Instruments)
                {
                    IQueryable<Exchange> exchanges = from e in _context.Exchanges select e;
                    Exchange? exchange = null;
                    if (exchanges.Count() > 0 && exchanges.First(exchange => exchange.Name == apiShare.Exchange) != null)
                    {
                        exchange = exchanges.First(exchange => exchange.Name == apiShare.Exchange);
                    }
                    else
                    {
                        exchange = new Exchange(apiShare.Exchange);
                        _context.Exchanges.Add(exchange);
                    }

                    IQueryable<Country> countries = from c in _context.Countries select c;
                    Country? country = null;
                    if (countries.Count() > 0 && countries.First(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk) != null)
                    {
                        country = countries.First(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk);
                    }
                    else
                    {
                        country = new Country(apiShare.CountryOfRiskName, apiShare.CountryOfRisk);
                        _context.Countries.Add(country);
                    }

                    IQueryable<Sector> sectors = from s in _context.Sectors select s;
                    Sector? sector = null;
                    if (sectors.Count() > 0 && sectors.First(sector => sector.Name == apiShare.Sector) != null)
                    {
                        sector = sectors.First(sector => sector.Name == apiShare.Sector);
                    }
                    else
                    {
                        sector = new Sector(apiShare.Sector);
                        _context.Sectors.Add(sector);
                    }

                    IQueryable<Share> shares = from s in _context.Shares select s;
                    Share? share = null;
                    if (shares.Count() > 0 && shares.First(shares => shares.Figi == apiShare.Figi) != null)
                    {
                        share = shares.First(shares => shares.Figi == apiShare.Figi);
                        share.Update(apiShare, exchange, country, sector);
                        share.Candles = new CandlesController(_context, _investApi, _configuration).GetCandles(share);
                        _context.Shares.Update(share);
                    }
                    else
                    {
                        share = new Share(apiShare, exchange, country, sector);
                        share.Candles = new CandlesController(_context, _investApi, _configuration).GetCandles(share);
                        _context.Shares.Add(share);
                    }
                }

                _context.SaveChangesAsync();
            }
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

        // GET api/test
        [HttpGet("test/1")]
        public string Test(int id)
        {
            return "test";
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
