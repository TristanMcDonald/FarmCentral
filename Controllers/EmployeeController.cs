using FarmCentral.DataAccess;
using FarmCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmCentral.Controllers
{
    public class EmployeeController : Controller
    {
        //Creating an object of the EmployeeDAL
        EmployeeDAL employeeDAL = new EmployeeDAL();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind] Employee employeeObj)
        {
            if (ModelState.IsValid)
            {
                //Using the DAL to check if the Employee Username exists in the database
                if (employeeDAL.CheckIfEmployeeExists(employeeObj).Equals(true))
                {
                    //Using the employeeDAL to check if the password that is asociated with the Employees Username is valid.
                    if (employeeDAL.CheckEmployeeCredentials(employeeObj).Equals(true))
                    {
                        //Logging in the Employee after all checks have been made.
                        employeeDAL.LoginEmployee(employeeObj);
                        //Redirecting to the Index page once signed in.
                        return RedirectToAction("AllFarmers", "Farmer_Product");
                    }
                }
                return RedirectToAction("Login");
            }
            return View(employeeObj);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind] Employee employeeObj)
        {
            if (ModelState.IsValid)
            {
                //Using the employeeDAL to check if the employee's Username exists in the database
                bool found = employeeDAL.CheckIfEmployeeExists(employeeObj);

                if (found.Equals(false))
                {
                    //Registering the new employee using the DAL method.
                    employeeDAL.RegisterNewEmployee(employeeObj);
                    //Redirecting to the Login page once Registered.
                    return RedirectToAction("Login");
                }
                else if (found.Equals(true))
                {
                    return RedirectToAction("UsernameExistsError");
                }
            }
            return View(employeeObj);
        }
        //Returns the relevant page when the relevant error occurrs.
        public IActionResult UsernameExistsError()
        {
            return View();
        }
    }
}
