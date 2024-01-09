using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using irm_wpf.Entity;
using Microsoft.EntityFrameworkCore;

namespace irm_wpf.EFCore {
    public class MyDbContext : DbContext {
        public DbSet<DataList> DataLists { get; set; }

        public DbSet<DataDetail> DataDetails { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite ("Data Source=/data/yourDatabase.db");
        }

    }
}