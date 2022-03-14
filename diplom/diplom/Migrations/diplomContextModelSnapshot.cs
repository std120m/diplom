﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using diplom.Data;

#nullable disable

namespace diplom.Migrations
{
    [DbContext(typeof(diplomContext))]
    partial class diplomContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
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

            modelBuilder.Entity("diplom.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Symbol")
                        .HasColumnType("longtext");

                    b.Property<string>("longName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Company");
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

                    b.Property<int>("ShareType")
                        .HasColumnType("int");

                    b.Property<string>("Ticker")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("ExchangeId");

                    b.HasIndex("SectorId");

                    b.ToTable("Shares");
                });

            modelBuilder.Entity("diplom.Models.Candle", b =>
                {
                    b.HasOne("diplom.Models.Share", "Share")
                        .WithMany("Candles")
                        .HasForeignKey("ShareId");

                    b.Navigation("Share");
                });

            modelBuilder.Entity("diplom.Models.Share", b =>
                {
                    b.HasOne("diplom.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("diplom.Models.Exchange", "Exchange")
                        .WithMany()
                        .HasForeignKey("ExchangeId");

                    b.HasOne("diplom.Models.Sector", "Sector")
                        .WithMany()
                        .HasForeignKey("SectorId");

                    b.Navigation("Country");

                    b.Navigation("Exchange");

                    b.Navigation("Sector");
                });

            modelBuilder.Entity("diplom.Models.Share", b =>
                {
                    b.Navigation("Candles");
                });
#pragma warning restore 612, 618
        }
    }
}
