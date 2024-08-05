using Data_Security_Layer.Utils;
using System.Text.Json;

namespace Data_Security_Layer.Blocks
{
    internal class Block
    {
        private string _chainHash;
        private string _hash;
        private string _previousHash;
        private string _version = "1";
        private string _merkleRoot;
        private DateTime _time;
        private List<string> _transactions = new List<string>();

        //Event-Handler 
        public event EventHandler? DataAdded;
        public event EventHandler? BlockFilled;

        //Standard Contructor for building new Blocks
        public Block(string ChainHash, string Hash,string previousHash, EventHandler onBlockFilled)
        {
            _chainHash = ChainHash;
            _hash = Hash;
            _time = DateTime.Now;
            _merkleRoot = string.Empty;
            _previousHash = previousHash;
            DataAdded += BlockEventHandler.OnDataAdded;
            DataAdded?.Invoke(this, EventArgs.Empty);
            BlockFilled += onBlockFilled;
        }

        //Constructor for Loading Blocks from File
        private Block(BlockDTO blockDTO, EventHandler onBlockFilled)
        {
            _chainHash = blockDTO.ChainHash??"";
            _hash = blockDTO.Hash??"";
            _merkleRoot= blockDTO.MerkleRoot??"";
            _previousHash = blockDTO.PreviousHash??"";
            _time = DateTime.Parse(blockDTO.Time??"");
            _transactions = blockDTO.Transactions??[];
            DataAdded += BlockEventHandler.OnDataAdded;
            BlockFilled += onBlockFilled;
        }

        //Public Properties for the JSON-Serializer
        public string ChainHash { get { return _chainHash; } }
        public string Hash { get { return _hash; } }
        public string PreviousHash {  get { return _previousHash; } }
        public string MerkleRoot {  get { return _merkleRoot; } }
        public string Time { get { return _time.ToString(); } }
        public List<string> Transactions { get { return _transactions; } }
        
        //Function to Load Blocks from File
        public static Block loadBlockFromFile(string chainhash, string hash, EventHandler onBlockfilled)
        {
            string path = Path.Combine(DataSecurityLayer.instance.GetChainsPath(), chainhash, hash);
            path = Path.ChangeExtension(path, "bcdb");
            FileStream File = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader Reader = new StreamReader(File);
            string JSON = Reader.ReadToEnd();
            File.Close();
            Reader.Close();
            Block? block = new Block(JsonSerializer.Deserialize<BlockDTO>(JSON) ?? throw new ArgumentNullException(nameof(BlockDTO)), onBlockfilled);
            return block;
        }

        //Function for adding new Data to the Block
        public void addData(string Data)
        {
            this._transactions.Add(Data);
            _merkleRoot = MerkleRootCalculator.calculateMerkleRoot(_transactions);
            DataAdded?.Invoke(this, EventArgs.Empty);
            if (this._transactions.Count == 64)
            {
                BlockFilled?.Invoke(this, EventArgs.Empty);
            }
        }

        //Function to get the Data from the Block
        public List<string> getData()
        {
            return _transactions;
        }

        //Function for the Consistencytest for testing if the MerkleRoot is Still working out
        internal string recalculateMerkleroot()
        {
            return MerkleRootCalculator.calculateMerkleRoot(this._transactions);
        }
    }
}
