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


        public DbSet<CandlesByDay> CandlesByDay { get; set; }
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
                        eb.Property(v => v.Day).HasColumnName("Day");
                        eb.Property(v => v.ShareId).HasColumnName("ShareId");
                    });
        }
    }
}
