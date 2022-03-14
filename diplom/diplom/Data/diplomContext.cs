﻿#nullable disable
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

        public DbSet<diplom.Models.Company> Company { get; set; }

        public DbSet<diplom.Models.Country> Countries { get; set; }

        public DbSet<diplom.Models.Exchange> Exchanges { get; set; }

        public DbSet<diplom.Models.Sector> Sectors { get; set; }

        public DbSet<diplom.Models.Share> Shares { get; set; }

        public DbSet<diplom.Models.Candle> Candles { get; set; }
    }
}
