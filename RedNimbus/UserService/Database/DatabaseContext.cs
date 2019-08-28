using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.DatabaseModel;

namespace UserService.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserDB> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Read connection parameters from config file
            optionsBuilder.UseMySQL("server=localhost;database=rednimbususerdb;user=root;password=root");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This is Fluent API, Alternate keys can be set using data annotations
            modelBuilder.Entity<UserDB>().HasAlternateKey(e => e.Email);

        }
    }
}
