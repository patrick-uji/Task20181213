using System;
using Task20181213.Common;
using System.Globalization;
using Task20181213.Common.DB;
using Microsoft.AspNetCore.Mvc;
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
                return BadRequest("Invalid date format supplied in 'date' parameter. Expected format: yyyy-MM-dd");
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
        private DateTime ParseISODate(string date)
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
