using System;
using System.Collections.Generic;
namespace Task20181213_P2.DTOs
{
    public class ExchangeRateDateGroupDTO
    {
        public string Date { get; set; }
        public IDictionary<string, decimal> Rates { get; set; }
        public ExchangeRateDateGroupDTO(string date, IDictionary<string, decimal> rates)
        {
            this.Date = date;
            this.Rates = rates;
        }
    }
}
