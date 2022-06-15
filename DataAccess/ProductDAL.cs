using FarmCentral.Data;
using FarmCentral.Models;
using System.Collections.Generic;
using System.Linq;

namespace FarmCentral.DataAccess
{
    public class ProductDAL
    {
        private FarmCentralContext _context;

        public ProductDAL()
        {
            _context = new FarmCentralContext();
        }

        //Method to be called when the user clicks the Add product button to add a new product to their profile.
        //This method will retrieve the values entered by the user and will assign them to the relevant properties in the relevant classes (Andrew Troelsen and Philip Japikse, 2017).
        public void AddNewProduct(Product productObj)
        {
            //Creating an object of the FarmerDAL class to access its properties
            FarmerDAL farmerDAL = new FarmerDAL();

            //Creating an object of the Farmer_Product class to access the properties.
            Farmer_Product farmer_product = new Farmer_Product();
            farmer_product.FarmerUsername = farmerDAL.GetLoggedInFarmer();
            farmer_product.ProductName = productObj.Name;
            farmer_product.Quantity = productObj.Quantity;

            //Adding the relationship between a specific farmer and their products to the bridging entity (Farmer_Product) table in the database.
            //This will ensure that every Farmer will only see their own data and never that of others.
            _context.Farmer_Products.Add(farmer_product);

            //Getting the result of the CheckIfProductExists method.
            bool ProductExists = CheckIfProductExists(productObj.Name);

            //Checking if the product that is being entered exists in the database and if not the product will be added.
            if (ProductExists.Equals(false))
            {
                //Adding the product to the database Products(Andrew Troelsen and Philip Japikse, 2017).
                _context.Products.Add(productObj);
            }

            //Saving the changes made to the database.
            _context.SaveChanges();
        }

        //Method to check if a product already exists in the database.
        public bool CheckIfProductExists(string ProductName)
        {
            bool ProductExists = false;

            //Checking if the product which the user has entered exists in the database (Lujan, 2016) & (Andrew Troelsen and Philip Japikse, 2017).
            foreach (var product in _context.Products)
            {
                ProductExists = product.Name.ToLower().Equals(ProductName.ToLower());
                //If the product is found exit the foreach loop (Lujan, 2016).
                if (ProductExists == true)
                {
                    break;
                }
            }
            return ProductExists;
        }


        //Fetching the products assigned to the farmer that is logged in.
        public IEnumerable<Product> GetFarmerProducts()
        {
            //Creating an object of the Farmer_Product class to access the Farmers Products.
            Farmer_Product farmer_Product = new Farmer_Product();

            //Creating an object of the FarmerDAL class to access the Farmer logged in.
            FarmerDAL farmerDAL = new FarmerDAL();

            //initializing the farmersProducts list.
            List<Product> farmersProducts = new List<Product>();

            //Getting the logged in farmer to use in the below Linq query.
            string loggedInFarmer = farmerDAL.GetLoggedInFarmer();

            foreach (var FarmerProduct in _context.Farmer_Products)
            {
                farmersProducts = new List<Product>
                    (from fp in _context.Farmer_Products
                     join f in _context.Farmers
                     on fp.FarmerUsername equals f.Username
                     join p in _context.Products
                     on fp.ProductName equals p.Name
                     where fp.FarmerUsername.Equals(loggedInFarmer)
                     select fp.Product
                    );
            }
            return farmersProducts;
        }
    }
}
