using System;
using Task20181213.Common.DB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
namespace Task20181213_P2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        readonly CurrencyMarketContext currencyMarket;
        public CurrenciesController(CurrencyMarketContext currencyMarket)
        {
            this.currencyMarket = currencyMarket;
        }
        [HttpGet]
        public IEnumerable<Currency> GetAll()
        {
            return currencyMarket.Currencies;
        }
    }
}
