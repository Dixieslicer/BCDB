using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data_Security_Layer.Blocks
{
    internal static class BlockEventHandler
    {
        public static void OnDataAdded(object? sender, EventArgs e)
        {
            //test if the sender is valid
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }
            else
            {
                //Calculating the string for saving the sender
                string path = string.Empty;
                //Calculation if the sender is a GenesisBlock
                if (sender.GetType() == typeof(GenesisBlock))
                {

                    GenesisBlock genesisBlock = sender as GenesisBlock?? throw new ArgumentNullException(nameof(sender));
                    path = Path.Combine(DataSecurityLayer._BasePath??throw new ArgumentNullException(nameof(DataSecurityLayer)), genesisBlock.Hash);
                    path = Path.ChangeExtension(path, ".bcdb");
                }
                //Calculation if the sender is a Block
                else if (sender.GetType() == typeof(Block))
                {
                    Block? block = sender as Block ?? throw new ArgumentNullException(nameof(sender));
                    path = Path.Combine(DataSecurityLayer.instance.GetChainsPath(), block.ChainHash, block.Hash);
                    path = Path.ChangeExtension(path, ".bcdb");
                }
                //Calculation if the sender is a Chain
                else if (sender.GetType() == typeof(Chain))
                {
                    Chain? chain = sender as Chain ?? throw new ArgumentNullException(nameof(sender));
                    path = Path.Combine(DataSecurityLayer.instance.GetChainsPath(), chain.Hash);
                    path = Path.ChangeExtension(path, ".bcdb");
                }
                //Abbort if the sender is not a Element to be saved
                else
                {
                    return;
                }
                //Saving the Data of the sender
                string JSON = JsonSerializer.Serialize(sender);
                FileStream File = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter Writer = new StreamWriter(File, Encoding.UTF8);
                Writer.Write(JSON);
                Writer.Flush();
                File.Flush();
                File.Close();
            }
            
        }
    }
}
