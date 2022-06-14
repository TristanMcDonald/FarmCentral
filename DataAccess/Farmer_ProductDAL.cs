using FarmCentral.Data;
using FarmCentral.Models;
using System.Collections.Generic;
using System.Linq;

namespace FarmCentral.DataAccess
{
    public class Farmer_ProductDAL
    {
        private FarmCentralContext _context;

        public Farmer_ProductDAL()
        {
            _context = new FarmCentralContext();
        }

        //Fetching all farmers from the database.
        public IEnumerable<Farmer> GetAllFarmers()
        {
            //initializing the farmersProducts list.
            List<Farmer> farmers = new List<Farmer>();

            foreach (var Farmer in _context.Farmers)
            {
                farmers = new List<Farmer>
                    (from f in _context.Farmers
                     select f
                    );
            }
            return farmers;
        }

        //Fetching all products assigned to all farmers.
        public IEnumerable<Farmer_Product> GetSpecificFarmersProducts(string Username)
        {

            //initializing the farmersProducts list.
            List<Farmer_Product> allFarmersProducts = new List<Farmer_Product>();

            foreach (var FarmerProduct in _context.Farmer_Products)
            {
                allFarmersProducts = new List<Farmer_Product>
                    (from fp in _context.Farmer_Products
                     join f in _context.Farmers
                     on fp.FarmerUsername equals f.Username
                     join p in _context.Products
                     on fp.ProductName equals p.Name
                     where fp.FarmerUsername.Equals(Username)
                     select fp
                    );
            }
            return allFarmersProducts;
        }
    }
}
