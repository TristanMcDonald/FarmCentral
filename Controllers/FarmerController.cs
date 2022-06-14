using FarmCentral.DataAccess;
using FarmCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmCentral.Controllers
{
    public class FarmerController : Controller
    {
        //Creating an object of the FarmerDAL
        FarmerDAL farmerDAL = new FarmerDAL();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind] Farmer farmerObj)
        {
            if (ModelState.IsValid)
            {
                //Using the DAL to check if the Farmers Username exists in the database
                if (farmerDAL.CheckIfFarmerExists(farmerObj).Equals(true))
                {
                    //Using the FarmerDAL to check if the password that is asociated with the Farmers Username is valid.
                    if (farmerDAL.CheckFarmerCredentials(farmerObj).Equals(true))
                    {
                        //Logging in the Farmer after all checks have been made.
                        farmerDAL.LoginFarmer(farmerObj);
                        //Redirecting to the Index page once signed in.
                        return RedirectToAction("Index", "Semester");
                    }
                }
                return RedirectToAction("Login");
            }
            return View(farmerObj);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind] Farmer farmerObj)
        {
            if (ModelState.IsValid)
            {
                //Using the farmerDAL to check if the farmers Username exists in the database
                bool found = farmerDAL.CheckIfFarmerExists(farmerObj);

                if (found.Equals(false))
                {
                    //Registering the new farmer using the DAL method.
                    farmerDAL.RegisterNewFarmer(farmerObj);
                    //Redirecting to the Login page once Registered.
                    return RedirectToAction("Login");
                }
                else if (found.Equals(true))
                {
                    return RedirectToAction("UsernameExistsError");
                }
            }
            return View(farmerObj);
        }
        //Returns the relevant page when the relevant error occurrs.
        public IActionResult UsernameExistsError()
        {
            return View();
        }

    }
}
