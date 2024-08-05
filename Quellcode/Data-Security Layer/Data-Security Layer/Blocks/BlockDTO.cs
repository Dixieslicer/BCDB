using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data_Security_Layer.Blocks
{
    internal class BlockDTO
    {
        public string? ChainHash {  get; set; }
        public string? Hash {  get; set; }
        public string? MerkleRoot {  get; set; }
        public string? PreviousHash { get; set; }
        public string? Time {  get; set; }
        public List<string>? Transactions { get; set; }
    }

    
}
