using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Task20181213.Common.DB
{
    public class CurrencyMarketContext : DbContext
    {
        Dictionary<string, Currency> currenciesCache;
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public CurrencyMarketContext()
        {
            this.currenciesCache = new Dictionary<string, Currency>();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=CurrencyMarket;Integrated Security=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Currency.ConfigureModel(modelBuilder);
            ExchangeRate.ConfigureModel(modelBuilder);
        }
        public Currency FindOrCreateCurrency(string code)
        {
            if (!currenciesCache.TryGetValue(code, out Currency currency))
            {
                currency = FindCurrencyIn(this.Currencies, code) ??
                           FindCurrencyIn(this.Currencies.Local, code); //Reminder: By using ".Local", we can query objects that have not been saved to the database yet!
                if (currency == null)
                {
                    currency = new Currency();
                    currency.Code = code;
                    this.Currencies.Add(currency);
                }
                currenciesCache.Add(code, currency);
            }
            return currency;
        }
        private Currency FindCurrencyIn(IEnumerable<Currency> currencies, string code)
        {
            return currencies.Where(currency => currency.Code == code).FirstOrDefault();
        }
    }
}
