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

        public DbSet<diplom.Models.Company> Company { get; set; }
    }
}
