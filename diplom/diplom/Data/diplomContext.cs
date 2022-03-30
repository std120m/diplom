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
    }
}
