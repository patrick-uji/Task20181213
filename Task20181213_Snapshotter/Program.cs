using System;
using Task20181213.Common;
using System.Globalization;
using Task20181213.Common.DB;
namespace Task20181213_Snapshotter
{
    class Program
    {
        private static void Main(string[] args)
        {
            bool invalidDates = true;
            while (invalidDates)
            {
                DateTime fromDate = PromptForDate("From date: ");
                DateTime untilDate = PromptForDate("Until date: ", fromDate);
                DateTime now = DateTime.Now;
                if (fromDate > untilDate)
                {
                    Console.WriteLine("ERROR: The 'From' date cannot be higher than the 'Until' date.\n");
                }
                else if (fromDate > now)
                {
                    Console.WriteLine("ERROR: The 'From' date cannot be after today.\n");
                }
                else if (untilDate > now)
                {
                    Console.WriteLine("ERROR: The 'Until' date cannot be after today.\n");
                }
                else
                {
                    Console.WriteLine();
                    invalidDates = false;
                    SnapshotExchangeRates(fromDate, untilDate);
                }
            }
        }
        private static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        private static DateTime PromptForDate(string message)
        {
            return PromptForDate(message, DateTime.Now);
        }
        private static DateTime PromptForDate(string message, DateTime defaultDate)
        {
            while (true)
            {
                string dateString = Prompt(message).Trim();
                if (dateString == String.Empty)
                {
                    return defaultDate;
                }
                try
                {
                    return DateTime.ParseExact(dateString, "d/M/yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid date. (Format: Day/Month/Year)\n");
                }
            }
        }
        private static void SnapshotExchangeRates(DateTime fromDate, DateTime untilDate)
        {
            using (var currencyMarket = new CurrencyMarketContext())
            {
                DateTime currDate = fromDate;
                while (currDate <= untilDate)
                {
                    Console.WriteLine("Snapshotting exchange rates in: " + currDate.ToString("dd/MM/yyyy"));
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
                    currDate = currDate.AddDays(1);
                    currencyMarket.SaveChanges();
                }
            }
        }
    }
}
