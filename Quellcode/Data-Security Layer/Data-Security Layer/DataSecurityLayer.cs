using Data_Security_Layer.Blocks;
using Data_Security_Layer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace Data_Security_Layer
{
    public class DataSecurityLayer
    {
        //Path of the Database
        internal static string? _BasePath;
        private GenesisBlock _GenesisBlock;

        //instance used to build a Singleton
        internal static DataSecurityLayer instance;
        
        internal GenesisBlock GenesisBlock
        {
            get
            {
                return _GenesisBlock;
            }
        }


        // internal Constructor for use in the singleton-pattern
        private DataSecurityLayer(string path, string name, string secretKey)
        {
            _BasePath = path;
            //Calcution of the Path with the name of the Databasesystem and the secret key
            string hash = SHA256Hashing.getHash(name+secretKey);
            string GenesisPath = Path.Combine(path, hash);
            //Checking if the Databasesystem is already existing and loading/creating the Genesisblock
            if (File.Exists(Path.ChangeExtension(GenesisPath, "bcdb")) == false)
            {
                Directory.CreateDirectory(GenesisPath);
                _GenesisBlock = new GenesisBlock(hash);
            }
            else
            {
                _GenesisBlock = GenesisBlock.loadGenesisBlockFromFile(GenesisPath);
            }
        }


        //Public function for getting the instance of the Databasesystem, part of the singleton-Pattern
        public static DataSecurityLayer getDatabaseSystem(string path, string name, string secretKey)
        {
            if (instance == null)
            {
                instance = new DataSecurityLayer(path, name, secretKey);
            }  
            //ConsistencyTest when loading the Databasesystem
            if(!(instance.ConsistencyTest(ConsistencyTestmode.Forwards) & instance.ConsistencyTest(ConsistencyTestmode.Backwards)))
            {
                throw new Exception("Es sind Inkonsistenzen in der Datenbank aufgetreten");
            }
            return instance;
        }

        //Function for getting the Path up to the Chains, used as an Shortcut for Calculating the Blockpaths
        internal string GetChainsPath()
        {
            return Path.Combine(_BasePath??"", _GenesisBlock.Hash);
        }

        //Function for adding a new Database to the Databasesystem
        public void newDatabase(string Name, string SecretKey)
        {
            try
            {
                this._GenesisBlock.addChain(Name, SecretKey);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //Function for getting the whole Data out of a Database
        public List<string> getData(string DatabaseName, string SecretKey)
        {
            Chain chain = GenesisBlock.getDataBase(DatabaseName, SecretKey);
            return chain.getData();
        }

        //Function for adding Data to a Database
        public void AddData(string DataBaseName, string SecretKey, string Data)
        {
            Chain chain = GenesisBlock.getDataBase(DataBaseName, SecretKey);
            chain.newData(Data);
        }

        //Testmodes for the ConsistencyTest
        public enum ConsistencyTestmode
        {
            Backwards = 0, //Testing from the Chain to the last Block
            Forwards = 1 //Testing from the Last Block back to the Chain
        }


        //Function for Testing the Consistency in the Blockchain Datastructure
        public bool ConsistencyTest(ConsistencyTestmode testmode)
        {
            //result as bool True = All Databases are consistent
            bool result = true;
            List<string> ChainHashes = _GenesisBlock.getDataBases();
            if (ChainHashes.Count == 0)
            {
                return true;
            }
            foreach (string ChainHash in ChainHashes)
            {
                Chain testChain = Chain.loadChainFromFile(ChainHash);
                switch (testmode)
                {
                    case ConsistencyTestmode.Backwards:
                        result = testChain.BackwardsConsistencyTest();
                        break;
                    case ConsistencyTestmode.Forwards:
                        result = testChain.ForwardsConsistencyTest();
                        break;
                }
                if (result == false)
                {
                    return result;
                }
            }
            return result;
        }
    }
}
 