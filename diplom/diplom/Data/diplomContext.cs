#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using diplom.Models;

namespace diplom.Data
{
    public class diplomContext : DbContext
    {

        public diplomContext (DbContextOptions<diplomContext> options)
            : base(options)
        {

        }

        public DbSet<diplom.Models.Company> Companies { get; set; }

        public DbSet<diplom.Models.Country> Countries { get; set; }

        public DbSet<diplom.Models.Exchange> Exchanges { get; set; }

        public DbSet<diplom.Models.Sector> Sectors { get; set; }

        public DbSet<diplom.Models.Share> Shares { get; set; }

        public DbSet<diplom.Models.Candle> Candles { get; set; }

        public DbSet<diplom.Models.CompanyEvents> CompanyEvents { get; set; }

        public DbSet<diplom.Models.CompanyFilings> CompanyFilings { get; set; }

        public DbSet<diplom.Models.WorldNews> WorldNews { get; set; }
        public DbSet<diplom.Models.NewsQuotesImpact> NewsQuotesImpacts { get; set; }
        public DbSet<diplom.Models.Keyword> Keywords { get; set; }
        public DbSet<diplom.Models.WorldNewsKeyword> WorldNewsKeywords { get; set; }


        public DbSet<CandlesByDay> CandlesByDay { get; set; }
        public DbSet<SectorsStats> SectorsStats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CandlesByDay>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("View_CandlesByDay");
                        eb.Property(v => v.Volume).HasColumnName("Volume");
                        eb.Property(v => v.Open).HasColumnName("Open");
                        eb.Property(v => v.Close).HasColumnName("Close");
                        eb.Property(v => v.Low).HasColumnName("Low");
                        eb.Property(v => v.High).HasColumnName("High");
                        eb.Property(v => v.Date).HasColumnName("Date");
                        eb.Property(v => v.ShareId).HasColumnName("ShareId");
                    });

            modelBuilder
                .Entity<SectorsStats>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("View_SectorsStats");
                        eb.Property(v => v.It).HasColumnName("It");
                        eb.Property(v => v.Consumer).HasColumnName("Consumer");
                        eb.Property(v => v.HealthCare).HasColumnName("HealthCare");
                        eb.Property(v => v.Financial).HasColumnName("Financial");
                        eb.Property(v => v.Industrials).HasColumnName("Industrials");
                        eb.Property(v => v.Energy).HasColumnName("Energy");
                        eb.Property(v => v.Telecom).HasColumnName("Telecom");
                        eb.Property(v => v.Other).HasColumnName("Other");
                        eb.Property(v => v.Materials).HasColumnName("Materials");
                        eb.Property(v => v.Date).HasColumnName("Date");
                    });
        }
    }
}
