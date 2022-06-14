using FarmCentral.Data;
using FarmCentral.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace FarmCentral.DataAccess
{
    public class EmployeeDAL
    {
        private FarmCentralContext _context;

        public EmployeeDAL()
        {
            _context = new FarmCentralContext();
        }

        //Creating an object of the Employee class to assign values to the username and password properties.
        public static Employee thisEmp = new Employee();

        //Creating an object of this class to access the context.
        public static EmployeeDAL empDAL = new EmployeeDAL();

        //Method to Register a new employee by adding their information entered to the database (context).
        public void RegisterNewEmployee(Employee employee)
        {
            //Saving the new employees entered password into the password model property.
            employee.Password = HashPassword(employee.Password);

            //Adding the new Employee to the database employees (Andrew Troelsen and Philip Japikse, 2017).
            empDAL._context.Employees.Add(employee);
            //Save the changes made to the database context.
            _context.SaveChanges();
        }

        //Method for use in the LoginPage to check if the employee username entered already exists in the database.
        public bool CheckIfEmployeeExists(Employee employee)
        {
            bool found = false;

            //Checking if the username which the employee has entered exists in the database (Lujan, 2016) & (Andrew Troelsen and Philip Japikse, 2017).
            foreach (var user in empDAL._context.Employees)
            {
                found = user.Username.Equals(employee.Username);
                //If the username is found exit the foreach loop (Lujan, 2016).
                if (found == true)
                {
                    break;
                }
            }
            //Return true if a user with that username was found or return false if it was not.
            return found;
        }

        //Method for use in the LoginPage to check if the password entered matches the username for the employee in the database.
        public bool CheckEmployeeCredentials(Employee employee)
        {
            bool authorized = false;

            //LINQ query to fetch the employee that matches the username entered (Wagner, 2017) & (Andrew Troelsen and Philip Japikse, 2017).
            List<Employee> empQuery = new List<Employee>
                (
                    from emp in empDAL._context.Employees
                    where emp.Username.Equals(employee.Username)
                    select emp
                );

            //Checking if the password which the employee has entered exists in the database (Lujan, 2016) & (Andrew Troelsen and Philip Japikse, 2017).
            foreach (var emp in empQuery)
            {
                authorized = VerifyHashedPassword(emp.Password, employee.Password);
                //If the password matches exit the foreach loop (Lujan, 2016).
                if (authorized == true)
                {
                    break;
                }

            }
            //Return true if a user with matching password was found or return false if it was not.
            return authorized;
        }

        //Method to log in the Employee.
        public void LoginEmployee(Employee employee)
        {
            //Setting the username property in the Employee class to the username of the employee logging in
            //for use in other view models to only show the logged in employees relevant data and not that of others.
            thisEmp.Username = employee.Username;
        }

        //Method to retrieve the current logged in Employee.
        public string GetLoggedInEmployee()
        {
            return thisEmp.Username;
        }

        //The following method was taken from stackoverflow:
        //Author : Michael
        //Link : https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

        public string HashPassword(string password)
        {
            var prf = KeyDerivationPrf.HMACSHA256;
            var rng = RandomNumberGenerator.Create();
            const int iterCount = 10000;
            const int saltSize = 128 / 8;
            const int numBytesRequested = 256 / 8;

            // Produce a version 3 (see comment above) text hash.
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, iterCount);
            WriteNetworkByteOrder(outputBytes, 9, saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return Convert.ToBase64String(outputBytes);
        }

        //The following method was taken from stackoverflow:
        //Author : Michael
        //Link : https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // If Wrong version
            if (decodedHashedPassword[0] != 0x01)
                return false;

            // Read header information
            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
            var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
            var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }
            var salt = new byte[saltLength];
            Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }

        //The following method was taken from stackoverflow:
        //Author : Michael
        //Link : https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        //The following method was taken from stackoverflow:
        //Author : Michael
        //Link : https://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }
    }
}
