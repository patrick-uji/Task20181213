using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Task20181213.Common.DB
{
    public class Currency
    {
        public int ID { get; set; }
        public string Code { get; set; }

        public ICollection<ExchangeRate> SourceExchanges { get; set; }
        public ICollection<ExchangeRate> TargetExchanges { get; set; }

        public static void ConfigureModel(ModelBuilder modelBuilder)
        {
            var modelEntity = modelBuilder.Entity<Currency>();
            modelEntity.HasIndex(currency => currency.Code).IsUnique();
            modelEntity.Property(currency => currency.Code).IsRequired();
        }
    }
}
