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
                decimal exchangeRate = date != null ? Fixer.GetExchangeRate(from, to, ParseISODate(date))
                                                    : Fixer.GetExchangeRate(from, to);
                return Ok(amount * exchangeRate);
            }
            catch (FormatException)
            {
                return BadDateRequest("date");
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
                return BadDateRequest("from");
            }
            if (!DefaultOrTryParseDate(to, out toDate, now))
            {
                return BadDateRequest("to");
            }
            if (fromDate > toDate)
            {
                return BadRequest("The 'from' parameter cannot have a later date than the 'to' parameter");
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
        private BadRequestObjectResult BadDateRequest(string dateParamName)
        {
            return BadRequest("Invalid date format supplied in '" + dateParamName + "' parameter. Expected format: yyyy-MM-dd");
        }
    }
}
