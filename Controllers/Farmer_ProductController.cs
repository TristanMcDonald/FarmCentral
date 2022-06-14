using FarmCentral.DataAccess;
using FarmCentral.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FarmCentral.Controllers
{
    public class Farmer_ProductController : Controller
    {
        //GET: Farmer_ProductDAL
        //Creating an object of the ProductDAL
        public Farmer_ProductDAL farmer_ProductDAL = new Farmer_ProductDAL();

        public IActionResult Index()
        {
            return View();
        }

        public ViewResult AllFarmers()
        {
            //Get all Farmers
            var farmerList = new List<Farmer>();
            //Calling the DAL method which parses a list with all farmers
            farmerList = farmer_ProductDAL.GetAllFarmers().ToList();
            //Return the list of farmers.
            return View(farmerList);
        }

        //Get Farmers Products
        [Route("Farmer_Product/Products/{Username}")]
        [HttpGet]
        public IActionResult FarmersProducts(string Username)
        {
            //Get all Products that belong to the selected Farmer
            var farmersProductList = new List<Farmer_Product>();
            //Calling the DAL method which parses a list with all the farmers products
            farmersProductList = farmer_ProductDAL.GetSpecificFarmersProducts(Username).ToList();
            //Return the list of farmers products.
            return View(farmersProductList);
        }
    }
}
