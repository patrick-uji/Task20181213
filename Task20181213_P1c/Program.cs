using System;
using Task20181213.Common;
using Task20181213.Common.DB;
namespace Task20181213.P1c
{
    class Program
    {
        private static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            using (var currencyMarket = new CurrencyMarketContext())
            {
                foreach (var currExchangeRate in Fixer.GetAllExchangeRates())
                {
                    Currency sourceCurrency = currencyMarket.FindOrCreateCurrency(currExchangeRate.SourceCurrency);
                    Currency targetCurrency = currencyMarket.FindOrCreateCurrency(currExchangeRate.TargetCurrency);
                    currencyMarket.ExchangeRates.Add(new ExchangeRate() {
                        SourceCurrency = sourceCurrency,
                        TargetCurrency = targetCurrency,
                        Rate = currExchangeRate.Rate,
                        Date = now
                    });
                }
                currencyMarket.SaveChanges();
            }
        }
    }
}
