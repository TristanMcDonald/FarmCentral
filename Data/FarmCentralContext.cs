using FarmCentral.Models;
using System.Data.Entity;

namespace FarmCentral.Data
{
    public class FarmCentralContext : DbContext
    {
        public FarmCentralContext()
        {
        }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Farmer_Product> Farmer_Products { get; set; }
    }
}
