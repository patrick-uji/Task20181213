using System;
using System.Linq;
using Task20181213.Common;
using Task20181213_P2.DTOs;
using System.Globalization;
using Task20181213.Common.DB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
namespace Task20181213_P2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        readonly CurrencyMarketContext currencyMarket;
        readonly ILogger<ExchangeRatesController> logger;
        public ExchangeRatesController(ILogger<ExchangeRatesController> logger, CurrencyMarketContext currencyMarket)
        {
            this.logger = logger;
            this.currencyMarket = currencyMarket;
        }
        [HttpGet("exchange")]
        public ActionResult<decimal> Exchange([FromQuery, Required] string from,
                                              [FromQuery, Required] string to,
                                              [FromQuery, Required] decimal amount,
                                              [FromQuery] string date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                decimal exchangeRate;
                if (date != null)
                {
                    DateTime parsedDate = ParseISODate(date);
                    if (parsedDate > DateTime.Now)
                    {
                        return DateAfterTodayError("date");
                    }
                    exchangeRate = Fixer.GetExchangeRate(from, to, parsedDate);
                }
                else
                {
                    exchangeRate = Fixer.GetExchangeRate(from, to);
                }
                return Ok(amount * exchangeRate);
            }
            catch (FormatException)
            {
                return DateFormatError("date");
            }
            catch (FixerException ex)
            {
                if (ex.ErrorCode == FixerErrorCode.InvalidCurrency)
                {
                    return BadRequest(ex.Message);
                }
                logger.LogError(ex, "{Controller}.Exchange({From}, {To}, {Amount}, {Date})", nameof(ExchangeRatesController), from, to, amount, date);
                return StatusCode(502, "Something went wrong.");
            }
        }
        [HttpGet("{code}")]
        public ActionResult<IEnumerable<ExchangeRateDateGroupDTO>> GetAllExchangeRatesFor([FromRoute] string code,
                                                                                          [FromQuery] string from,
                                                                                          [FromQuery] string to)
        {
            DateTime toDate;
            DateTime fromDate;
            DateTime now = DateTime.Now;
            if (to != null && from == null)
            {
                return BadRequest("You must also supply a 'from' parameter if a 'to' parameter is supplied.");
            }
            if (!DefaultOrTryParseDate(from, out fromDate, now))
            {
                return DateFormatError("from");
            }
            if (!DefaultOrTryParseDate(to, out toDate, now))
            {
                return DateFormatError("to");
            }
            if (fromDate > toDate)
            {
                return DateOrderError("from", "to");
            }
            var result = currencyMarket.ExchangeRates.Where(exchangeRate => exchangeRate.SourceCurrency.Code == code &&
                                                                            exchangeRate.Date >= fromDate && exchangeRate.Date <= toDate)
                                                     .Select(exchangeRate => new { exchangeRate.TargetCurrency.Code, exchangeRate.Rate, exchangeRate.Date }) //Allows us to extract the target currency code!
                                                     .AsEnumerable()
                                                     .GroupBy(exchangeRate => exchangeRate.Date,
                                                              (key, group) => new ExchangeRateDateGroupDTO( key.ToString("yyyy-MM-dd"),
                                                                                                            group.ToDictionary(exchangeRate => exchangeRate.Code,
                                                                                                                               exchangeRate => exchangeRate.Rate) )
                                                             );
            return Ok(result);
        }
        [HttpGet("snapshot")]
        public ActionResult<string> Snapshot([FromQuery] string from,
                                             [FromQuery] string until)
        {
            string response;
            DateTime fromDate;
            DateTime untilDate;
            DateTime now = DateTime.Now;
            ERSnapshotsTracker snapshotsTracker;
            if (!DefaultOrTryParseDate(from, out fromDate, now))
            {
                return DateFormatError("from");
            }
            if (!DefaultOrTryParseDate(until, out untilDate, now))
            {
                return DateFormatError("until");
            }
            if (fromDate > untilDate)
            {
                return DateOrderError("from", "until");
            }
            if (fromDate > now)
            {
                return DateAfterTodayError("from");
            }
            if (untilDate > now)
            {
                return DateAfterTodayError("until");
            }
            snapshotsTracker = SnapshotExchangeRates(fromDate, untilDate);
            response = "Snapshot " + snapshotsTracker.Dates + " dates (Total exchange rates: " + snapshotsTracker.ExchangeRates + ").";
            logger.LogInformation(response);
            return Ok(response);
        }
        private ERSnapshotsTracker SnapshotExchangeRates(DateTime fromDate, DateTime untilDate)
        {
            ERSnapshotsTracker tracker = new ERSnapshotsTracker();
            DateTime currDate = fromDate;
            while (currDate <= untilDate)
            {
                if (!currencyMarket.ContainsExchangeRatesIn(currDate))
                {
                    logger.LogInformation("Snapshotting exchange rates in: " + currDate.ToString("dd/MM/yyyy"));
                    foreach (var currExchangeRate in Fixer.GetAllExchangeRates(currDate))
                    {
                        Currency sourceCurrency = currencyMarket.FindOrCreateCurrency(currExchangeRate.SourceCurrency);
                        Currency targetCurrency = currencyMarket.FindOrCreateCurrency(currExchangeRate.TargetCurrency);
                        currencyMarket.ExchangeRates.Add(new ExchangeRate()
                        {
                            SourceCurrency = sourceCurrency,
                            TargetCurrency = targetCurrency,
                            Rate = currExchangeRate.Rate,
                            Date = currDate
                        });
                    }
                    tracker.ExchangeRates += currencyMarket.ExchangeRates.Local.Count;
                    currencyMarket.SaveChanges();
                    tracker.Dates++;
                }
                else
                {
                    logger.LogInformation("Already contained the exchange rates in: " + currDate.ToString("dd/MM/yyyy") + " [Skipping]");
                }
                currDate = currDate.AddDays(1);
            }
            return tracker;
        }
        private bool DefaultOrTryParseDate(string date, out DateTime result, DateTime defaultValue)
        {
            if (date == null)
            {
                result = defaultValue;
                return true;
            }
            return TryParseISODate(date, out result);
        }
        private bool TryParseISODate(string date, out DateTime result)
        {
            try
            {
                result = ParseISODate(date);
                return true;
            }
            catch (FormatException)
            {
                result = DateTime.MinValue;
                return false;
            }
        }
        private DateTime ParseISODate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        private BadRequestObjectResult DateFormatError(string dateParamName)
        {
            return BadRequest("Invalid date format supplied in '" + dateParamName + "' parameter. Expected format: yyyy-MM-dd");
        }
        private BadRequestObjectResult DateOrderError(string fromDateParamName, string toDateParamName)
        {
            return BadRequest("The '" + fromDateParamName + "' parameter cannot have a later date than the '" + toDateParamName + "' parameter.");
        }
        private BadRequestObjectResult DateAfterTodayError(string dateParamName)
        {
            return BadRequest("The '" + dateParamName + "' date cannot be after today.");
        }
    }
}
