using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            String path = "DBConfig.json";
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var values = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
                var server      = values["server"];
                var database    = values["database"];
                var user        = values["user"];
                var password    = values["password"];
                optionsBuilder.UseMySQL("server=" + server + ";database=" + database + ";user=" + user + ";password=" + password);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var converter = new BoolToZeroOneConverter<Int16>();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDB>().HasAlternateKey(e => e.Email);

            modelBuilder.Entity<UserDB>().Property(e => e.ActiveAccount).HasConversion(converter);

        }
    }
}
