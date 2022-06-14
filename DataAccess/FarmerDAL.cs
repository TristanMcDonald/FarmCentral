using FarmCentral.Data;
using FarmCentral.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FarmCentral.DataAccess
{
    public class FarmerDAL
    {
        private FarmCentralContext _context;

        public FarmerDAL()
        {
            _context = new FarmCentralContext();
        }

        //Creating an object of the Farmer class to assign values to the username and password properties.
        public static Farmer thisFarmer = new Farmer();

        //Creating an object of this class to access the context.
        public static FarmerDAL farmerDAL = new FarmerDAL();

        //Method to Register a new Farmer by adding their information entered to the database (context).
        public void RegisterNewFarmer(Farmer farmer)
        {
            //Saving the new farmers entered password into the password model property.
            farmer.Password = HashPassword(farmer.Password);

            //Adding the new Farmer to the database Farmers (Andrew Troelsen and Philip Japikse, 2017).
            farmerDAL._context.Farmers.Add(farmer);
            //Save the changes made to the database context.
            _context.SaveChanges();
        }

        //Method for use in the LoginPage to check if the farmer username entered already exists in the database.
        public bool CheckIfFarmerExists(Farmer farmer)
        {
            bool found = false;

            //Checking if the username which the farmer has entered exists in the database (Lujan, 2016) & (Andrew Troelsen and Philip Japikse, 2017).
            foreach (var user in farmerDAL._context.Farmers)
            {
                found = user.Username.Equals(farmer.Username);
                //If the username is found exit the foreach loop (Lujan, 2016).
                if (found == true)
                {
                    break;
                }
            }
            //Return true if a user with that username was found or return false if it was not.
            return found;
        }

        //Method for use in the LoginPage to check if the password entered matches the username for the farmer in the database.
        public bool CheckFarmerCredentials(Farmer farmer)
        {
            bool authorized = false;

            //LINQ query to fetch the farmer that matches the username entered (Wagner, 2017) & (Andrew Troelsen and Philip Japikse, 2017).
            List<Farmer> farmerQuery = new List<Farmer>
                (
                    from farm in farmerDAL._context.Farmers
                    where farm.Username.Equals(farmer.Username)
                    select farm
                );

            //Checking if the password which the farmer has entered exists in the database (Lujan, 2016) & (Andrew Troelsen and Philip Japikse, 2017).
            foreach (var farm in farmerQuery)
            {
                authorized = VerifyHashedPassword(farm.Password, farmer.Password);
                //If the password matches exit the foreach loop (Lujan, 2016).
                if (authorized == true)
                {
                    break;
                }

            }
            //Return true if a user with matching password was found or return false if it was not.
            return authorized;
        }

        //Method to log in the Farmer.
        public void LoginFarmer(Farmer farmer)
        {
            //Setting the username property in the Farmer class to the username of the farmer logging in
            //for use in other view models to only show the logged in farmers relevant data and not that of others.
            thisFarmer.Username = farmer.Username;
        }

        //Method to retrieve the current logged in Farmer.
        public string GetLoggedInFarmer()
        {
            return thisFarmer.Username;
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
