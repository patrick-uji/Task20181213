using System;
using System.Net;
using System.Globalization;
namespace Task20181213_P1
{
    class Program
    {
        private static void Main(string[] args)
        {
            bool retry = true;
            while (retry)
            {
                string sourceCurrency = PromptForCode("Source currency code: ");
                string targetCurrency = PromptForCode("Target currency code: ");
                decimal amount = PromptForAmount("Currency amount: ");
                try
                {
                    decimal exchangeRate;
                    Console.WriteLine();
                    if (QuestionPrompt("Would you like to check for a particular date? "))
                    {
                        exchangeRate = Fixer.GetExchangeRate(sourceCurrency, targetCurrency, PromptForDate("Date: "));
                    }
                    else
                    {
                        exchangeRate = Fixer.GetExchangeRate(sourceCurrency, targetCurrency);
                    }
                    Console.WriteLine("\n" + amount + " " + sourceCurrency + " = " + (amount * exchangeRate) + " " + targetCurrency);
                    retry = false;
                }
                catch (FixerException ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message + "\n");
                    retry = QuestionPrompt("Would you like to try again? ");
                }
                catch (WebException ex)
                {
                    Console.WriteLine("ERROR: Internet connection problem. (" + ex.Message + ")\n");
                    retry = QuestionPrompt("Would you like to try again? ");
                }
            }
        }
        private static string Prompt(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        private static string PromptForCode(string message)
        {
            while (true)
            {
                string code = Prompt(message).Trim();
                if (code.Length == 3 && ContainsOnlyLetters(code)) //Assuming ISO_4217 codes...
                {
                    return code;
                }
                Console.WriteLine("Please enter a valid currency code.\n");
            }
        }
        private static bool ContainsOnlyLetters(string text)
        {
            foreach (var currChar in text)
            {
                if (!char.IsLetter(currChar))
                {
                    return false;
                }
            }
            return true;
        }
        private static decimal PromptForAmount(string message)
        {
            while (true)
            {
                string amountString = Prompt(message).Trim();
                if (decimal.TryParse(amountString, out decimal amount))
                {
                    return amount;
                }
                Console.WriteLine("Please enter a valid amount.\n");
            }
        }
        private static DateTime PromptForDate(string message)
        {
            while (true)
            {
                string dateString = Prompt(message).Trim();
                try
                {
                    return DateTime.ParseExact(dateString, "d/M/yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please enter a valid date.\n");
                }
            }
        }
        private static bool QuestionPrompt(string message)
        {
            while (true)
            {
                string answer = Prompt(message).Trim();
                switch (answer.ToLower())
                {
                    case "y":
                    case "yes": return true;
                    case "n":
                    case "no": return false;
                }
                Console.WriteLine("Please answer with just Y(es) or N(o).\n");
            }
        }
    }
}
