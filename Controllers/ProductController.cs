using FarmCentral.DataAccess;
using FarmCentral.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FarmCentral.Controllers
{
    public class ProductController : Controller
    {
        //GET: ProductDAL
        //Creating an object of the ProductDAL
        public ProductDAL productDAL = new ProductDAL();

        public ViewResult Index()
        {
            //Get all Farmer Products
            var productList = new List<Product>();
            //Calling the DAL method which parses a list with all the farmers products.
            productList = productDAL.GetFarmerProducts().ToList();
            //Return the list of products for the logged in farmer to the Products view.
            return View(productList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add([Bind] Product productObj)
        {
            if (ModelState.IsValid)
            {
                //Calling the DAL method to add a new product for the farmer.
                productDAL.AddNewProduct(productObj);
                //once the product is added redirect to the Index view.
                return RedirectToAction("Index");
            }
            return View(productObj);
        }
    }
}
