using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using irm_wpf.Entity;
using Microsoft.EntityFrameworkCore;
using OxyPlot.Series;

namespace irm_wpf.EFCore
{
    public class MyDbContext : DbContext
    {
        public DbSet<DataList> DataLists { get; set; }

        public DbSet<DataDetail> DataDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source="+DatabaseInitializer.databasePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataList>(b=>{
                b.HasKey(p=>p.ID);
                b.HasIndex(p=>p.FileId).IsUnique();
                b.HasIndex(p => p.FileName);
            });
                
            modelBuilder.Entity<DataDetail>(b=>{
                b.HasKey(p=>p.ID);
                b.HasIndex(p => p.FileId);
            });
        }

    }
}