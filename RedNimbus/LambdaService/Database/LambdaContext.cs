using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RedNimbus.Domain;
using RedNimbus.LambdaService.Database;
using RedNimbus.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedNimbus.LambdaService.Database
{
    public class LambdaContext : DbContext
    {
        public DbSet<LambdaDB> Lambdas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = "LambdaConfig.json";

            using(StreamReader sr = new StreamReader(path))
            {
                string json = sr.ReadToEnd();
                var values = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
                var server = values["server"];
                var database = values["database"];
                var user = values["user"];
                var password = values["password"];

                string connStr = "server=" + server + ";database=" + database + ";user=" + user + ";password=" + password;

                optionsBuilder.UseMySQL(connStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LambdaDB>().Property(e => e.Trigger).HasConversion(
            t => t.ToString(),
            t => (LambdaMessage.Types.TriggerType)Enum.Parse(typeof(LambdaMessage.Types.TriggerType), t));

            modelBuilder.Entity<LambdaDB>().Property(e => e.Runtime).HasConversion(
            r => r.ToString(),
            r => (LambdaMessage.Types.RuntimeType)Enum.Parse(typeof(LambdaMessage.Types.RuntimeType), r));

        }
    }
}
