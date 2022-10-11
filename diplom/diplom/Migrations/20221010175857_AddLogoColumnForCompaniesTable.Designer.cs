﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using diplom.Data;

#nullable disable

namespace diplom.Migrations
{
    [DbContext(typeof(diplomContext))]
    [Migration("20221010175857_AddLogoColumnForCompaniesTable")]
    partial class AddLogoColumnForCompaniesTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("diplom.Models.Candle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<double?>("Close")
                        .HasColumnType("double");

                    b.Property<double?>("High")
                        .HasColumnType("double");

                    b.Property<double?>("Low")
                        .HasColumnType("double");

                    b.Property<double?>("Open")
                        .HasColumnType("double");

                    b.Property<int?>("ShareId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)");

                    b.Property<long?>("Volume")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ShareId");

                    b.ToTable("Candles");
                });

            modelBuilder.Entity("diplom.Models.CandlesByDay", b =>
                {
                    b.Property<double?>("Close")
                        .HasColumnType("double")
                        .HasColumnName("Close");

                    b.Property<string>("Date")
                        .HasColumnType("longtext")
                        .HasColumnName("Date");

                    b.Property<double?>("High")
                        .HasColumnType("double")
                        .HasColumnName("High");

                    b.Property<double?>("Low")
                        .HasColumnType("double")
                        .HasColumnName("Low");

                    b.Property<double?>("Open")
                        .HasColumnType("double")
                        .HasColumnName("Open");

                    b.Property<long?>("ShareId")
                        .HasColumnType("bigint")
                        .HasColumnName("ShareId");

                    b.Property<long?>("Volume")
                        .HasColumnType("bigint")
                        .HasColumnName("Volume");

                    b.ToView("View_CandlesByDay");
                });

            modelBuilder.Entity("diplom.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double?>("BookValuePerShare")
                        .HasColumnType("double");

                    b.Property<string>("BrandInfo")
                        .HasColumnType("longtext");

                    b.Property<double?>("CurrentRatio")
                        .HasColumnType("double");

                    b.Property<double?>("DebtToEquity")
                        .HasColumnType("double");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<long?>("Ebitda")
                        .HasColumnType("bigint");

                    b.Property<double?>("EnterpriseToEbitda")
                        .HasColumnType("double");

                    b.Property<double?>("EnterpriseToRevenue")
                        .HasColumnType("double");

                    b.Property<long?>("EnterpriseValue")
                        .HasColumnType("bigint");

                    b.Property<long?>("FloatShares")
                        .HasColumnType("bigint");

                    b.Property<double?>("ForwardPE")
                        .HasColumnType("double");

                    b.Property<long?>("FreeCashflow")
                        .HasColumnType("bigint");

                    b.Property<int?>("FullTimeEmployees")
                        .HasColumnType("int");

                    b.Property<long?>("GrossProfits")
                        .HasColumnType("bigint");

                    b.Property<string>("Logo")
                        .HasColumnType("longtext");

                    b.Property<long?>("NetIncomeToCommon")
                        .HasColumnType("bigint");

                    b.Property<long?>("OperatingCashflow")
                        .HasColumnType("bigint");

                    b.Property<double?>("OperatingMargins")
                        .HasColumnType("double");

                    b.Property<double?>("PriceToBook")
                        .HasColumnType("double");

                    b.Property<double?>("ProfitMargins")
                        .HasColumnType("double");

                    b.Property<double?>("ReturnOnAssets")
                        .HasColumnType("double");

                    b.Property<double?>("ReturnOnEquity")
                        .HasColumnType("double");

                    b.Property<long?>("Revenue")
                        .HasColumnType("bigint");

                    b.Property<double?>("RevenueGrowth")
                        .HasColumnType("double");

                    b.Property<double?>("RevenuePerShare")
                        .HasColumnType("double");

                    b.Property<double?>("SandP52WeekChange")
                        .HasColumnType("double");

                    b.Property<long?>("SharesOutstanding")
                        .HasColumnType("bigint");

                    b.Property<long?>("SharesShort")
                        .HasColumnType("bigint");

                    b.Property<long?>("SharesShortPriorMonth")
                        .HasColumnType("bigint");

                    b.Property<double?>("ShortPercentOfFloat")
                        .HasColumnType("double");

                    b.Property<double?>("ShortRatio")
                        .HasColumnType("double");

                    b.Property<long?>("TotalCash")
                        .HasColumnType("bigint");

                    b.Property<double?>("TotalCashPerShare")
                        .HasColumnType("double");

                    b.Property<long?>("TotalDebt")
                        .HasColumnType("bigint");

                    b.Property<double?>("TrailingEps")
                        .HasColumnType("double");

                    b.Property<string>("Website")
                        .HasColumnType("longtext");

                    b.Property<double?>("Week52Change")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("diplom.Models.CompanyEvents", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Company_Events");
                });

            modelBuilder.Entity("diplom.Models.CompanyFilings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Company_Filings");
                });

            modelBuilder.Entity("diplom.Models.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("diplom.Models.Exchange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Exchanges");
                });

            modelBuilder.Entity("diplom.Models.NewsQuotesImpact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<double>("Influence")
                        .HasColumnType("double");

                    b.Property<int>("WorldNewsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("WorldNewsId");

                    b.ToTable("Company_World_News");
                });

            modelBuilder.Entity("diplom.Models.Sector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("diplom.Models.Share", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClassCode")
                        .HasColumnType("longtext");

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Currency")
                        .HasColumnType("longtext");

                    b.Property<int?>("ExchangeId")
                        .HasColumnType("int");

                    b.Property<string>("Figi")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("IpoDate")
                        .HasColumnType("datetime(6)");

                    b.Property<long?>("IssuePlanSize")
                        .HasColumnType("bigint");

                    b.Property<long?>("IssueSize")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("SectorId")
                        .HasColumnType("int");

                    b.Property<int?>("ShareType")
                        .HasColumnType("int");

                    b.Property<string>("Ticker")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CountryId");

                    b.HasIndex("ExchangeId");

                    b.HasIndex("SectorId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("diplom.Models.WorldNews", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("World_News");
                });

            modelBuilder.Entity("diplom.Models.Candle", b =>
                {
                    b.HasOne("diplom.Models.Share", "Share")
                        .WithMany("Candles")
                        .HasForeignKey("ShareId");

                    b.Navigation("Share");
                });

            modelBuilder.Entity("diplom.Models.CompanyEvents", b =>
                {
                    b.HasOne("diplom.Models.Company", null)
                        .WithMany("Events")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("diplom.Models.CompanyFilings", b =>
                {
                    b.HasOne("diplom.Models.Company", null)
                        .WithMany("Filings")
                        .HasForeignKey("CompanyId");
                });

            modelBuilder.Entity("diplom.Models.NewsQuotesImpact", b =>
                {
                    b.HasOne("diplom.Models.Company", "Company")
                        .WithMany("NewsQuotesImpacts")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("diplom.Models.WorldNews", "WorldNews")
                        .WithMany("NewsQuotesImpacts")
                        .HasForeignKey("WorldNewsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("WorldNews");
                });

            modelBuilder.Entity("diplom.Models.Share", b =>
                {
                    b.HasOne("diplom.Models.Company", "Company")
                        .WithMany("Shares")
                        .HasForeignKey("CompanyId");

                    b.HasOne("diplom.Models.Country", "Country")
                        .WithMany("Shares")
                        .HasForeignKey("CountryId");

                    b.HasOne("diplom.Models.Exchange", "Exchange")
                        .WithMany("Shares")
                        .HasForeignKey("ExchangeId");

                    b.HasOne("diplom.Models.Sector", "Sector")
                        .WithMany("Shares")
                        .HasForeignKey("SectorId");

                    b.Navigation("Company");

                    b.Navigation("Country");

                    b.Navigation("Exchange");

                    b.Navigation("Sector");
                });

            modelBuilder.Entity("diplom.Models.Company", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Filings");

                    b.Navigation("NewsQuotesImpacts");

                    b.Navigation("Shares");
                });

            modelBuilder.Entity("diplom.Models.Country", b =>
                {
                    b.Navigation("Shares");
                });

            modelBuilder.Entity("diplom.Models.Exchange", b =>
                {
                    b.Navigation("Shares");
                });

            modelBuilder.Entity("diplom.Models.Sector", b =>
                {
                    b.Navigation("Shares");
                });

            modelBuilder.Entity("diplom.Models.Share", b =>
                {
                    b.Navigation("Candles");
                });

            modelBuilder.Entity("diplom.Models.WorldNews", b =>
                {
                    b.Navigation("NewsQuotesImpacts");
                });
#pragma warning restore 612, 618
        }
    }
}
