using System;
namespace Task20181213.Common
{
    public class FixerExchangeRate
    {
        public decimal Rate { get; private set; }
        public string SourceCurrency { get; private set; }
        public string TargetCurrency { get; private set; }
        public FixerExchangeRate(string sourceCurrency, string targetCurrency, decimal rate)
        {
            this.Rate = rate;
            this.SourceCurrency = sourceCurrency;
            this.TargetCurrency = targetCurrency;
        }
    }
}
