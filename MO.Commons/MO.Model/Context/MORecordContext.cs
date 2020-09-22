using Microsoft.EntityFrameworkCore;
using MO.Model.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace MO.Model.Context
{
    public class MORecordContext : DbContext
    {
        public DbSet<LoginRecord> LoginRecords { get; set; }

        public MORecordContext(DbContextOptions<MORecordContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LoginRecord>().ToTable("LoginRecord");
        }
    }
}
