using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.MVVM.Models;

namespace OrderManagementSystem.Dataa
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "maui.db");

            optionsBuilder.UseSqlite($"Filename = {dbPath}");
        }
    }
}
