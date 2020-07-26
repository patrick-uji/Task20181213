using System;
using System.Net;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
namespace Task20181213.Common
{
    public static class Fixer
    {
        const string API_KEY = "..."; //Ideally keep this in an environment variable or secrets file
        const string API_URL = "http://data.fixer.io/api/";
        const string ACCESS_KEY_SUFFIX = "?access_key=" + API_KEY;
        const string LATEST_URL = API_URL + "latest" + ACCESS_KEY_SUFFIX;
        public static decimal GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            return QueryExchangeRate(LATEST_URL, sourceCurrency, targetCurrency);
        }
        public static decimal GetExchangeRate(string sourceCurrency, string targetCurrency, DateTime date)
        {
            return QueryExchangeRate(BuildDateQueryURL(date), sourceCurrency, targetCurrency);
        }
        private static decimal QueryExchangeRate(string baseURL, string sourceCurrency, string targetCurrency)
        {
            string url = baseURL + "&base=" + sourceCurrency + "&symbols=" + targetCurrency;
            JObject exchangeRates = QueryExchangeRates(url);
            return exchangeRates.Value<decimal>(targetCurrency.ToUpper());
        }
        public static IEnumerable<FixerExchangeRate> GetAllExchangeRates()
        {
            return QueryAllExchangeRates(GetAllExchangeRates);
        }
        public static IEnumerable<FixerExchangeRate> GetAllExchangeRates(DateTime date)
        {
            return QueryAllExchangeRates(currencyCode => GetAllExchangeRates(currencyCode, date));
        }
        private static IEnumerable<FixerExchangeRate> QueryAllExchangeRates(Func<string, IEnumerable<FixerExchangeRate>> getExchangeRatesFunc)
        {
            List<string> currencyCodes = new List<string>();
            foreach (var currExchangeRate in getExchangeRatesFunc("EUR"))
            {
                yield return currExchangeRate;
                currencyCodes.Add(currExchangeRate.TargetCurrency);
            }
            currencyCodes.Remove("EUR");
            for (int currCurrencyIndex = 0; currCurrencyIndex < currencyCodes.Count; currCurrencyIndex++)
            {
                Debug.Print("QueryAllExchangeRates() [" + currCurrencyIndex + "/" + currencyCodes.Count + "]");
                foreach (var currExchangeRate in getExchangeRatesFunc(currencyCodes[currCurrencyIndex]))
                {
                    yield return currExchangeRate;
                }
            }
            Debug.Print("QueryAllExchangeRates() DONE!");
        }
        public static IEnumerable<FixerExchangeRate> GetAllExchangeRates(string sourceCurrency)
        {
            return QueryAllExchangeRates(LATEST_URL, sourceCurrency);
        }
        public static IEnumerable<FixerExchangeRate> GetAllExchangeRates(string sourceCurrency, DateTime date)
        {
            return QueryAllExchangeRates(BuildDateQueryURL(date), sourceCurrency);
        }
        private static IEnumerable<FixerExchangeRate> QueryAllExchangeRates(string baseURL, string sourceCurrency)
        {
            JObject exchangeRates = QueryExchangeRates(baseURL + "&base=" + sourceCurrency);
            foreach (var currExchangeRate in exchangeRates.Properties())
            {
                yield return new FixerExchangeRate(sourceCurrency, currExchangeRate.Name, (decimal)currExchangeRate.Value);
            }
        }
        private static JObject QueryExchangeRates(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                JObject jsonResponse = JObject.Parse(webClient.DownloadString(url));
                ErrorCodeCheck(jsonResponse);
                return jsonResponse.Value<JObject>("rates");
            }
        }
        private static void ErrorCodeCheck(JObject jsonResponse)
        {
            if (!jsonResponse.Value<bool>("success"))
            {
                JObject errorNode = jsonResponse.Value<JObject>("error");
                FixerErrorCode errorCode = (FixerErrorCode)errorNode.Value<int>("code");
                switch (errorCode)
                {
                    case FixerErrorCode.NoAPIKey: throw new FixerException("API key not supplied. (Check Fixer.cs)", errorCode);
                    case FixerErrorCode.InvalidCurrency: throw new FixerException("Invalid currency code supplied.", errorCode);
                    case FixerErrorCode.RestrictedAPIKey: throw new FixerException("API key has restricted access.", errorCode);
                    default: throw new FixerException("Unknown Fixer.io error code (" + errorCode + "): " + errorNode.Value<string>("info"), errorCode);
                }
            }
        }
        private static string BuildDateQueryURL(DateTime date)
        {
            return API_URL + date.ToString("yyyy-MM-dd") + ACCESS_KEY_SUFFIX;
        }
    }
}
