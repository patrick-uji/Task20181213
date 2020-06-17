using System;
using System.Net;
using Newtonsoft.Json.Linq;
namespace Task20181213_P1
{
    public static class Fixer
    {
        const string BASE_URL = "http://data.fixer.io/api";
        const string API_KEY = "..."; //Ideally keep this in an environment variable or secrets file
        const string ACCESS_KEY_SUFFIX = "?access_key=" + API_KEY;
        const string LATEST_URL = BASE_URL + "/latest" + ACCESS_KEY_SUFFIX;
        public static decimal GetLatestExchangeRate(string sourceCurrency, string targetCurrency)
        {
            using (WebClient webClient = new WebClient())
            {
                string url = LATEST_URL + "&base=" + sourceCurrency + "&symbols=" + targetCurrency;
                JObject jsonResponse = JObject.Parse(webClient.DownloadString(url));
                ErrorCodeCheck(jsonResponse);
                return jsonResponse.Value<JObject>("rates").Value<decimal>(targetCurrency);
            }
        }
        private static void ErrorCodeCheck(JObject jsonResponse)
        {
            if (!jsonResponse.Value<bool>("success"))
            {
                JObject errorNode = jsonResponse.Value<JObject>("error");
                int errorCode = errorNode.Value<int>("code");
                switch (errorCode)
                {
                    case 101: throw new FixerException("API key not supplied. (Check Fixer.cs)", errorCode);
                    case 105: throw new FixerException("API key has restricted access.", errorCode);
                    case 202: throw new FixerException("Invalid currency code supplied.", errorCode);
                    default: throw new FixerException("Unknown Fixer.io error code (" + errorCode + "): " + errorNode.Value<string>("info"), errorCode);
                }
            }
        }
    }
}
