﻿using diplom.Data;
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
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.Collections;
using System.Text;
using System.Text.RegularExpressions;
using diplom.Helpers;
using Microsoft.ML;
using static Diplom.MLModel;
using diplom.Models.SentimentPrediction;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace diplom.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : Controller
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

        // GET: api/share/candles
        [HttpGet("share/candles")]
        public JsonResult GetCandles()
        {
            Microsoft.Extensions.Primitives.StringValues period, shareId;
            if (!Request.Query.TryGetValue("period", out period))
                return Json(null);
            if (!Request.Query.TryGetValue("share", out shareId))
                return Json("Share not found");

            Models.Share? share = _context.Shares.Find(int.Parse(shareId.ToString()));
            if (share == null)
                return Json("Share not found");

            StockRecommendation recommendation = FundamentalAnalysis.GetSummaryRecommendation(share);
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return Json(share.GetCandlesByDay(_context), options);
        }

        // GET: api/news/update
        [HttpGet("news/update")]
        public async Task UpdateWorldNews()
        {
            await new WorldNewsController(_context, _configuration).UpdateWorldNews();
        }

        // GET: api/news/
        [HttpGet("news")]
        public JsonResult GetWorldNews()
        {
            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");

            string date = HttpContext.Request.Query["date"].ToString();
            DateTime? dateTime = null;
            if (date != null)
            {
                string[] dateParts = date.Split('-');
                dateTime = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]));
            }
            List<WorldNews> worldNews = new WorldNewsController(_context, _configuration).GetWorldNews(dateTime);
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };
            return Json(worldNews.ToArray(), options);
        }

        // GET: api/shares/update
        [HttpGet("shares/update/{id?}")]
        public async Task UpdateShares(int? id = null)
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
                await _context.SaveChangesAsync();

                IQueryable<Country> countries = _context.Countries.Include(country => country.Shares);
                Country? country = countries.Any() ? countries.FirstOrDefault(country => country.Name == apiShare.CountryOfRiskName && country.Code == apiShare.CountryOfRisk) : null;
                if (country == null)
                {
                    country = new Country(apiShare.CountryOfRiskName, apiShare.CountryOfRisk);
                    _context.Countries.Add(country);
                }
                await _context.SaveChangesAsync();

                IQueryable<Sector> sectors = _context.Sectors.Include(sector => sector.Shares);
                Sector? sector = sectors.Any() ? sectors.FirstOrDefault(sector => sector.Name == apiShare.Sector) : null;
                if (sector == null)
                {
                    sector = new Sector(apiShare.Sector);
                    _context.Sectors.Add(sector);
                }
                await _context.SaveChangesAsync();

                IQueryable<Share> shares = _context.Shares.Include(share => share.Candles);
                Share? share = shares.Any() ? shares.FirstOrDefault(shares => shares.Figi == apiShare.Figi) : null;
                if (share != null)
                {
                    share.Update(apiShare, exchange, country, sector);
                    _context.Shares.Update(share);
                }
                else
                {
                    share = new Share(apiShare, exchange, country, sector);
                    _context.Shares.Add(share);
                }
                await _context.SaveChangesAsync();
                await new CandlesController(_context, _investApi, _configuration).UpdateCandles(share);

                IQueryable<Company> companies = _context.Companies;
                Company? company = companies.Any() ? companies.FirstOrDefault(company => company.Shares.Count() > 0 && company.Shares.FirstOrDefault(companyShare => companyShare.Figi == share.Figi) != null) : null;
                if (company == null)
                    company = new Company();
                await new CompaniesController(_context).UpdateCompany(share, company);
            }
        }

        // GET: api/candles/prediction
        //[HttpGet("candles/prediction")]
        //public JsonResult GetCandlesPrediction([Bind("Ticker")] Share share)
        //{
        //    return Json(TechnicalAnalysis.GetPrediction(share));
        //}

        // GET: api/candles/forecast
        [HttpGet("candles/forecast")]
        public void GetForecasting()
        {
            new ForecastingModel().GetForecast(_configuration, _context);
        }

        // GET: api/news/sentiment
        [HttpGet("news/sentiment")]
        public void GetSentimentPrediction()
        {
            SentimentPredictionModel model = new SentimentPredictionModel();
            var prediction = model.Predict(@"В среду, 5 октября, «Белуга» опубликует операционные результаты за 3 квартал 2022 года. В 3 квартале 2021 года продажи собственных брендов компании снизились на 3,4% г/г, составив 3,3 млн декалитров.
                            Еще завтра последний день покупки для получения дивидендов «НОВАТЭКа». За 1 полугодие 2022 года полагается выплата 45 рублей на одну акцию.
                            Также в среду пройдет внеочередное общее собрание акционеров ""Ренессанс Страхования"".Закрытие реестра состоялось 13 августа.На ВОСА будут избраны новые члены совета директоров компании.
                            Кроме того, заседания советов директоров проведут «Газпром», «Селигдар» и «Россети Северный Кавказ».");
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
