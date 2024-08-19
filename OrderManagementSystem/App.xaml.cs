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

            MainPage = new AppShell();
        }
    }
}
