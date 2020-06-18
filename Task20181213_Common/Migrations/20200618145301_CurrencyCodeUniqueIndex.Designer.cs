﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Task20181213.Common.DB;

namespace Task20181213_Common.Migrations
{
    [DbContext(typeof(CurrencyMarketContext))]
    [Migration("20200618145301_CurrencyCodeUniqueIndex")]
    partial class CurrencyCodeUniqueIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Task20181213.Common.DB.Currency", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Task20181213.Common.DB.ExchangeRate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SourceCurrencyID")
                        .HasColumnType("int");

                    b.Property<int>("TargetCurrencyID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("SourceCurrencyID");

                    b.HasIndex("TargetCurrencyID");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("Task20181213.Common.DB.ExchangeRate", b =>
                {
                    b.HasOne("Task20181213.Common.DB.Currency", "SourceCurrency")
                        .WithMany("SourceExchanges")
                        .HasForeignKey("SourceCurrencyID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Task20181213.Common.DB.Currency", "TargetCurrency")
                        .WithMany("TargetExchanges")
                        .HasForeignKey("TargetCurrencyID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
