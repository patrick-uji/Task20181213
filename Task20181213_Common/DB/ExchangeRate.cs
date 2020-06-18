using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace Task20181213.Common.DB
{
    public class ExchangeRate
    {
        public int ID { get; set; }
        public Currency SourceCurrency { get; set; }
        public Currency TargetCurrency { get; set; }
        public decimal Rate { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public static void ConfigureModel(ModelBuilder modelBuilder)
        {
            var modelEntity = modelBuilder.Entity<ExchangeRate>();
            modelEntity.HasOne(exchangeRate => exchangeRate.SourceCurrency)
                       .WithMany(currency => currency.SourceExchanges)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired();

            modelEntity.HasOne(exchangeRate => exchangeRate.TargetCurrency)
                       .WithMany(currency => currency.TargetExchanges)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired();
        }
    }
}
