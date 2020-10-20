using Microsoft.EntityFrameworkCore;
using MO.Model.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Model.Context
{
    public class MODataContext : DbContext
    {
        public DbSet<GameUser> GameUsers { get; set; }
        public DbSet<ServerConfig> ServerConfigs { get; set; }

        public MODataContext(DbContextOptions<MODataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameUser>().ToTable("GameUser");
            modelBuilder.Entity<PhoneCode>().ToTable("PhoneCode");
            modelBuilder.Entity<ServerConfig>().ToTable("ServerConfig");
            modelBuilder.Entity<ServerConfig>().Property(m => m.ApiIP).HasDefaultValue("127.0.0.1");
            modelBuilder.Entity<ServerConfig>().Property(m => m.ApiPort).HasDefaultValue(8000);
            modelBuilder.Entity<ServerConfig>().Property(m => m.GateIP).HasDefaultValue("127.0.0.1");
            modelBuilder.Entity<ServerConfig>().Property(m => m.GatePort).HasDefaultValue(9001);
            modelBuilder.Entity<ServerConfig>().Property(m => m.LoginIP).HasDefaultValue("127.0.0.1");
            modelBuilder.Entity<ServerConfig>().Property(m => m.LoginPort).HasDefaultValue(8001);
        }
    }
}
