using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Transactions;

namespace Data_Security_Layer.Utils
{
    internal static class MerkleRootCalculator
    {
        //Geting the strings to Hashvalues for the first level of nodes
        public static string calculateMerkleRoot(List<String> transactions)
        {
            List<String> HashList = new List<string>();
            using (SHA256 mySHA256 = SHA256.Create())
            {
                if (transactions.Count == 0)
                {
                    return "";
                }

                foreach (string transaction in transactions)
                {
                    byte[] hashValue = Encoding.UTF8.GetBytes(transaction);
                    byte[] hash = mySHA256.ComputeHash(hashValue);
                    string HashString = string.Empty;
                    foreach (byte b in hash)
                    {
                        HashString += String.Format("{0:x2}", b);
                    }
                    HashList.Add(HashString);
                }
            }

            return calculateMerkleRoot(HashList, true);
        }

        //condensing the list of Hashvalues to one Hashvalue
        public static string calculateMerkleRoot(List<String> transactions, bool alreadyHashed)
        {
            List<string> HashList = new List<string>();
            for (int i = 0; i < transactions.Count; i++)
            {
                string tmpHashValueString = string.Empty;
//Diasabling the warning, that the ArgumentOutOfRangeException variable e is never used
#pragma warning disable CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
                try
                {
                    tmpHashValueString = transactions[i] + transactions[++i];

                }
                catch (ArgumentOutOfRangeException e)
                {

                    tmpHashValueString = transactions[--i] + transactions[i];
                }
#pragma warning restore CS0168 // Variable ist deklariert, wird jedoch niemals verwendet
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    byte[] hashValue = Encoding.UTF8.GetBytes(tmpHashValueString);
                    byte[] hash = mySHA256.ComputeHash(hashValue);
                    string HashString = string.Empty;
                    foreach (byte b in hash)
                    {
                        HashString += String.Format("{0:x2}", b);
                    }
                    HashList.Add(HashString);
                }
            }
            if (HashList.Count == 1)
            {
                return HashList[0];
            }
            else
            {
                return calculateMerkleRoot(HashList, true);
            }
        }
    }
}
