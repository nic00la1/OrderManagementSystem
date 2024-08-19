using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Dataa;
using OrderManagementSystem.MVVM.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace OrderManagementSystem
{
    public partial class App : Application
    {
        private readonly DataContext _context;

        public App(DataContext context)
        {
            InitializeComponent();

            _context = context;
            context.Database.Migrate();
            SeedDatabase();

            MainPage = new AppShell();
        }

        private void SeedDatabase()
        {
            if(_context.Products.Any())
                return; // DB has been seeded

            Product product = new Product(
                id: 1,
                name: "Sample Product",
                price: 19.99m,
                category: "Electronics",
                quantity: 100,
                distribution: "Online",
                invoiceNumber: "INV123456",
                purchaseDate: DateTime.Now,
                comment: "This is a sample product."
            );
            _context.Products.Add(product);
            _context.SaveChanges();
        }
    }
}
