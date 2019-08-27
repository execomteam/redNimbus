using Microsoft.EntityFrameworkCore;
using RedNimbus.UserService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Database
{
    public class UserDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Read connection parameters from config file
            optionsBuilder.UseMySQL("server=localhost;database=rednimbususerdb;user=root;password=root");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This is Fluent API and this can be also solved using data annotations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id); // Primary key
                entity.HasAlternateKey(e => e.Email); // Alternate key a.k.a unique value
                entity.Property(e => e.FirstName);
                entity.Property(e => e.LastName);
                entity.Property(e => e.Password);
                entity.Property(e => e.PhoneNumber);
            });
        }
    }
}
