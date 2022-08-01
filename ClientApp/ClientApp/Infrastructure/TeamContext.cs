using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Infrastructure
{
    public class TeamContext : DbContext
    {
        public TeamContext(DbContextOptions options)
         : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<Team> Teams { get; set; }

    }
}
