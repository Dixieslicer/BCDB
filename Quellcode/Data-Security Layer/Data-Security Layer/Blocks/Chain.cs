using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Data_Security_Layer.Utils;

namespace Data_Security_Layer.Blocks
{
    internal class Chain
    {
        public event EventHandler DataAdded;
        private List<Block> _Blocks;
        private string _HashOfHighestBlockATM;
        private string _chainHash;
        private string _chainName;
        private Block _actualBlock;

        //Constructor to Build a new Chain
        public Chain(string ChainHash, string ChainName)
        {
            _Blocks = new List<Block>();
            _HashOfHighestBlockATM = string.Empty;
            _chainHash = ChainHash;
            _chainName = ChainName;
            DataAdded += BlockEventHandler.OnDataAdded;
            Directory.CreateDirectory(Path.Combine(DataSecurityLayer.instance.GetChainsPath(), _chainHash));
            _Blocks.Add(firstBlock());
            _actualBlock = _Blocks[0];
            _HashOfHighestBlockATM = _actualBlock.Hash;
            DataAdded?.Invoke(this, EventArgs.Empty);
        }

        //Property for use in the Class own functions
        private Block ActualBlock
        {
            set
            {
                _actualBlock = value;
            }
        }

        //Private Constructor for loading Chains from file
        private Chain(ChainDTO chainDTO)
        {
            _Blocks = new List<Block>();
            _HashOfHighestBlockATM = chainDTO.HighestHash??"";
            _chainHash = chainDTO.Hash??"";
            _chainName = chainDTO.ChainName??"";
            DataAdded += BlockEventHandler.OnDataAdded;
            _actualBlock = Block.loadBlockFromFile(_chainHash, _HashOfHighestBlockATM, NewBlock);
        }

        //Properties for the JSON-Serializer
        public string Hash
        {
            get
            {
                return _chainHash;
            }
        }

        public string HighestHash
        {
            get
            {
                return _HashOfHighestBlockATM;
            }
        }

        public string ChainName
        {
            get
            {
                return _chainName;
            }
        }

        //Function to load a Chain from File
        public static Chain loadChainFromFile(string Hash)
        {
            string path = Path.Combine(DataSecurityLayer.instance.GetChainsPath(), Hash);
            path = Path.ChangeExtension(path, "bcdb");
            FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(File);
            string JSON = Reader.ReadToEnd();
            File.Close();
            Reader.Close();
            Chain? chain = new Chain(JsonSerializer.Deserialize<ChainDTO>(JSON) ?? throw new ArgumentNullException(nameof(ChainDTO)));
            return chain;
        }

        //Function to add new Data to the last Block of the Chain
        internal void newData(string data)
        {
            _actualBlock.addData(data);
        }

        //Function to build the first Block of the chain
        private Block firstBlock()
        {
            Block block = new Block(_chainHash, SHA256Hashing.getHash(Path.Combine(DataSecurityLayer.instance.GetChainsPath(), _chainHash)),_chainHash, NewBlock);
            _HashOfHighestBlockATM = block.Hash;
            return block;
        }

        //Event-Handler to build a new Block when a Block is filled
        private void NewBlock(object? sender, EventArgs e)
        {
            if(sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }
            else
            {
                Block senderBlock = sender as Block ?? throw new ArgumentNullException(nameof(sender));
                Block block = new Block(senderBlock.ChainHash, SHA256Hashing.getHash(senderBlock.MerkleRoot + senderBlock.Hash), senderBlock.Hash, NewBlock);
                _actualBlock = block;
                _HashOfHighestBlockATM = block.Hash;
                DataAdded?.Invoke(this, EventArgs.Empty);
            }
            
        }

        //Function to receive the whole Data saved in the Blocks of the Chain
        public List<string> getData()
        {
            Block tmpBlock = Block.loadBlockFromFile(_chainHash, SHA256Hashing.getHash(Path.Combine(DataSecurityLayer.instance.GetChainsPath(), _chainHash)), NewBlock);
            List<string> data = new List<string>();
            bool firstRun = true;
            do
            {
                if (!firstRun)
                {
                    tmpBlock = Block.loadBlockFromFile(_chainHash, SHA256Hashing.getHash(tmpBlock.MerkleRoot + tmpBlock.Hash), NewBlock);
                }
                firstRun = false;
                data.AddRange(tmpBlock.getData());
            } while (tmpBlock.Hash != _HashOfHighestBlockATM);
            return data;
        }

        //Function to test if there was a change in the Data
        internal bool BackwardsConsistencyTest()
        {
            string ParentHash = string.Empty;
            bool result = true;
            Block testBlock = Block.loadBlockFromFile(_chainHash, HighestHash, BlockEventHandler.OnDataAdded);
            bool FirstRun = true;
            do
            {
                if (!FirstRun)
                {
                    testBlock = Block.loadBlockFromFile(_chainHash, ParentHash, BlockEventHandler.OnDataAdded);
                }
                FirstRun = false;
                ParentHash = testBlock.PreviousHash;
                if(testBlock.MerkleRoot != testBlock.recalculateMerkleroot())
                {
                    result = false; 
                    break;
                }
            } while (ParentHash != _chainHash && result == true);
            return result;
        }

        //Function to test if there was a change in the Data
        internal bool ForwardsConsistencyTest()
        {
            bool result = true;
            Block testBlock = Block.loadBlockFromFile(_chainHash, HighestHash, BlockEventHandler.OnDataAdded);
            do
            {
                if (testBlock.MerkleRoot != testBlock.recalculateMerkleroot())
                {
                    result = false;
                    break;
                }
                try
                {
                    testBlock = Block.loadBlockFromFile(_chainHash, SHA256Hashing.getHash(testBlock.MerkleRoot + testBlock.Hash), NewBlock);
                }
                catch
                {
                    result |= false; 
                    break;
                }
                
            } while (testBlock.Hash != _HashOfHighestBlockATM && result == true);
            return result;

        }
    }
}
