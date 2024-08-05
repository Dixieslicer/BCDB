using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Security_Layer.Blocks
{
    internal class GenesisBlockDTO
    {
        public string Hash { get; set; }
        public string Version { get; set; }
        public string Merkleroot { get; set; }
        public string Time { get; set; }
        public List<string> Transactions { get; set; }
        public List<string>Chains{ get; set; }
    }
}
