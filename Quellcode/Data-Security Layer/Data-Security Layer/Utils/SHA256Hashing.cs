using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;

namespace Data_Security_Layer.Utils
{
    internal static class SHA256Hashing
    {
        //Function for unifying the hashingprocess in the project
        internal static string getHash(string Input)
        {
            using(SHA256 mySHA256 = SHA256.Create())
            {
                byte[] hashValue = Encoding.UTF8.GetBytes(Input);
                byte[] hash = mySHA256.ComputeHash(hashValue);
                string HashString = string.Empty;
                foreach (byte b in hash)
                {
                    HashString += String.Format("{0:x2}", b);
                }
                return HashString;
            }
            
        }
    }
}
