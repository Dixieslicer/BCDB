using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Security_Layer.Blocks
{
    internal class ChainDTO
    {
        public string? Hash {  get; set; }
        public string? HighestHash {  get; set; }
        public string? ChainName { get; set; }
    }
}
