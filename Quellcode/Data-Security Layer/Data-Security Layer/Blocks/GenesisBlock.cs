using Data_Security_Layer.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Data_Security_Layer.Blocks
{
    internal class GenesisBlock
    {
        public event EventHandler DataAdded;
        private string _hash;
        private string _version = "1";
        private string _merkleroot;
        private DateTime _time;
        private List<string> _transactions;
        private List<string> _chains;
        private Dictionary<string, Chain> _loadedChains;

        //public Properties for the JSONSerializer
        public string Hash
        {
            get
            {
                return _hash;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
        }

        public string Merkleroot
        {
            get
            {
                return _merkleroot;
            }
        }

        public string Time
        {
            get
            {
                return _time.ToString();
            }
        }

        public List<string> Chains
        {
            get
            {
                return _chains;
            }
        }

        public List<string> Transactions
        {
            get
            {
                return _transactions;
            }
        }

        //Public Constructor
        public GenesisBlock(string hash)
        {
            _hash = hash;
            _merkleroot = string.Empty;
            _chains = new List<string>();
            _time = DateTime.Now;
            Directory.CreateDirectory(DataSecurityLayer._BasePath + "\\" + _hash);
            DataAdded += BlockEventHandler.OnDataAdded;
            DataAdded.Invoke(this, EventArgs.Empty);
            _transactions = new List<string>();
            _loadedChains = new Dictionary<string, Chain>();
        }

        //Constructor for Loading the Block from File
        private GenesisBlock(GenesisBlockDTO DTO)
        {
            _hash = DTO.Hash;
            _version = DTO.Version;
            _merkleroot = DTO.Merkleroot;
            _time = DateTime.Parse(DTO.Time);
            _transactions = DTO.Transactions?? new List<string>();
            _chains = DTO.Chains?? new List<string>();
            DataAdded += BlockEventHandler.OnDataAdded;
            _loadedChains = new Dictionary<string, Chain>();
        }

        //Adding a new DataBase
        public void addChain(string Name, string secretKey)
        {
            string searchstring = "NewChain:" + Name;
            
            //Checking if the Database is already existing
            if (_transactions.Contains(searchstring))
            {
                throw new ArgumentException("The Database is already existing");
            }
            else
            {
                //Adding the new Chain
                _transactions.Add("NewChain:" + Name);
                _merkleroot = MerkleRootCalculator.calculateMerkleRoot(_transactions);
                string chainhash = SHA256Hashing.getHash(_merkleroot + secretKey);
                Chain chain = new Chain(chainhash, Name);
                _chains.Add(chainhash);
                _loadedChains.Add(Name, chain);
                DataAdded?.Invoke(this, EventArgs.Empty);
            }
            
        }

        //Static Function for loading the Genesisblock from JSON
        internal static GenesisBlock loadGenesisBlockFromFile(string path)
        {
            path = Path.ChangeExtension(path, "bcdb");
            FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(File);
            string JSON = Reader.ReadToEnd();
            File.Close();
            Reader.Close();
            GenesisBlock? genesisBlock = new GenesisBlock(JsonSerializer.Deserialize<GenesisBlockDTO>(JSON)??throw new ArgumentNullException(nameof(JsonSerializer)));
            return genesisBlock;
        }

        //Function for getting the acces to a DataBase
        internal Chain getDataBase(string DataBaseName, string SecretKey)
        {
            string searchstring = "NewChain:" + DataBaseName;
            if (_transactions.Contains(searchstring))
            {
                int index = _transactions.IndexOf(searchstring);
                List<string> transactionsForMerkleRootCalculation = new List<string>();
                transactionsForMerkleRootCalculation = _transactions.Slice(0,index+1);
                string chainHash = SHA256Hashing.getHash(MerkleRootCalculator.calculateMerkleRoot(transactionsForMerkleRootCalculation) + SecretKey);
                if (_loadedChains.ContainsKey(chainHash))
                {
                    return _loadedChains[chainHash];
                }
                else
                {
                    _loadedChains.Add(chainHash, Chain.loadChainFromFile(chainHash));
                    return _loadedChains[chainHash];
                }
            }
            else
            {
                throw new ArgumentException("The called Database is not existend");
            }
        }

        //getting the Database over the DataBaseHash instead of using the name and key
        internal Chain getDataBase(string DataBaseHash)
        {
            if (_loadedChains.ContainsKey(DataBaseHash))
            {
                return _loadedChains[DataBaseHash];
            }
            else
            {
                _loadedChains.Add(DataBaseHash, Chain.loadChainFromFile(DataBaseHash));
                return _loadedChains[DataBaseHash];
            }
        }

        //Function for getting the Hashes of all Databases in this Databasesystem
        internal List<string> getDataBases()
        {
            return Chains;
        }
    }
}
