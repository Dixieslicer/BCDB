using Presentation_Layer.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation_Layer.DataBaseEvents;
using Presentation_Layer.WorkerChain;
using System.Security.Cryptography;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace Presentation_Layer
{
    internal class DataPresentationLayer
    {
        //Instance as part of the Singleton-Pattern
        private static DataPresentationLayer? _instance;
        //DataBaseAcces through DataBaseNaming
        private Dictionary<string, IDatabase>? _databases;
        //Path of the DataBase System
        private string? _path;
        //Data-Security-Layer Connector
        private DSLConnector? _connector;
        //WorkerChain
        private WorkerChain.WorkerChain? newCommands;
        //DataBaseLoader for Loading from File
        private DataBaseLoader? _loader;
        //Events for the Database
        internal event NewDataBaseEvent? CreateNewDataBase;
        internal event CreateDataEvent? CreateNewData;
        internal event UpdateDataEvent? UpdateData;
        internal event RemoveDataEvent? RemoveData;

        //private Constructor for the Singleton-Pattern
        private DataPresentationLayer()
        {
            

        }

        //Setting the path of the DataBasesystem
        public void activateDatabase(string path, string DataBaseSystemName, string secretkey)
        {
            _path = path;
            _connector = new DSLConnector(_path, DataBaseSystemName, secretkey);
            newCommands = new WorkerChain.WorkerChain();
            CreateNewDataBase += newCommands.execute;
            CreateNewData += newCommands.execute;
            UpdateData += newCommands.execute;
            RemoveData += newCommands.execute;
            newCommands.AddWorkers(new DataBaseCommandHandler());
            newCommands.AddWorkers(new DataCommandHandler ());
            newCommands.AddWorkers(_connector);
            _databases = new Dictionary<string, IDatabase>();
            _loader = new DataBaseLoader(newCommands);
            try
            {
                loadDatabase("PasswordDB");
            }catch
            {
                List<string> Fields = new List<string>()
                {
                    "DBname", "SecretKey"
                };
                List<string> optionalFields = new List<string>();
                DataBasetype DBType = DataBasetype.KVDB;
                NewDataBase("PasswordDB", "Passwords", DBType, Fields, optionalFields);
            }
        }

        //Delegates for the DataBaseEvents
        public delegate void NewDataBaseEvent(object sender, NewDataBaseEventArgs e);
        public delegate void CreateDataEvent(object sender, CreateDataEventArgs e);
        public delegate void UpdateDataEvent(object sender, UpdateDataEventArgs e);
        public delegate void RemoveDataEvent(object sender, RemoveDataEventArgs e);


        //Property for the Singleton-Pattern
        public static DataPresentationLayer Instance
        {
            get 
            {
                if(_instance == null)
                {
                    _instance = new DataPresentationLayer();
                    return _instance;
                } 
                return _instance;
            } 
        }

        //returns a Dictionary with:
        //1. A List of the non optional Fields in the Database
        //2. A Blanko Dictionary for all the Fields in the Database
        public Dictionary<string, object> GetDataPattern(string DatabaseName)
        {
            if(DatabaseName != null)
            {
                return _databases[DatabaseName].GetDataPattern();
            }
            else
            {
                throw new InvalidOperationException("The Databasesystem musst be activated to use this function");
            }
        }


        //Enum with the DataBaseTypes
        public enum DataBasetype
        {
            KVDB,
            Accounting
        }

        //Function to build a new DataBase
        public void NewDataBase(string DataBaseName, string secretKey, DataBasetype dataBaseType, List<string>fields, List<string> optionalFields)
        {
            NewDataBaseEventArgs e = new NewDataBaseEventArgs(DataBaseName, secretKey, dataBaseType, fields, optionalFields, false);
            if (CreateNewDataBase != null)
            {
                if (DataBaseName != "PasswordDB")
                {
                    //Checking if the name of the new Database is already in use
                    List<Dictionary<string, string>> PasswordDBData = _databases["PasswordDB"].ReadAll();
                    foreach (Dictionary<string, string> dataset in PasswordDBData)
                    {
                        if (dataset["DBName"] == DataBaseName)
                        {
                            throw new ArgumentException("There is already a Database with this name");
                        }
                    }
                    //Adding new Database to PasswordDB
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("DBName", DataBaseName);
                    data.Add("Passwords", secretKey);
                    Create("PasswordDB", data);
                }
                CreateNewDataBase(this, e);
            }
            else 
            {
                throw new InvalidOperationException("The Databasesystem musst be activated to use this function");
            }

        }
        
        //Function to Save new Data into the DataBase
        public void Create(string DataBaseName, Dictionary<string, string> data)
        {
            if (_databases != null & CreateNewData != null)
            {
                string key;
                try
                {
                    key = _databases[DataBaseName].GetActualKey();
                }catch
                {
                    loadDatabase(DataBaseName);
                    key = _databases[DataBaseName].GetActualKey();
                }
                
                CreateDataEventArgs e = new CreateDataEventArgs(data, key, _databases[DataBaseName], false);
                CreateNewData(this, e);
            }
            else
            {
                throw new InvalidOperationException("The Databasesystem musst be activated to use this function");
            }
            
        }

        //Functions to Read Data from the DataBase
        public Dictionary<string,string> ReadKey(string DataBaseName, string key)
        {
            IDatabase database = _databases[DataBaseName];
            return database.ReadKeyBased(key);
        }

        public List<Dictionary<string, string>>ReadFullDataBase(string DataBaseName)
        {
            IDatabase database = _databases[DataBaseName];
            return database.ReadAll();
        }

        //Function to read an Account out of a AccountingDB
        public Dictionary<string, List<Dictionary<string, string>>> ReadAccount(string DataBaseName, string account)
        {
            IDatabase database = _databases[DataBaseName];
            if (database.GetType() == typeof(AccountingDB))
            {
                AccountingDB Database = (AccountingDB)database;
                return Database.ReadAccount(account);
            }
            else
            {
                throw new InvalidOperationException("The choosen Database is not an AccountingDB");
            }
            
        }

        //Function to Update Data from the DataBase
        public void Update(string DataBaseName, Dictionary<string, string>newData, string key)
        {
            UpdateDataEventArgs e = new UpdateDataEventArgs(newData, key, _databases[DataBaseName], false);
            UpdateData(this, e);
        }

        //Function to Delete Data from a DataBase
        public void Delete(string DataBaseName, string key)
        {
            RemoveDataEventArgs e = new RemoveDataEventArgs(key,_databases[DataBaseName], false);
            RemoveData(this, e);
        }

        //getDatabase out of _Databases
        internal IDatabase getDataBase(string DataBaseName)
        {
            if (_databases != null)
            {
                return _databases[DataBaseName];
            }
            else
            {
                throw new InvalidOperationException("The databasesystem musst be activated before this function can be used");
            }

        }

        //add Database to _Databases
        internal void addDatabase(IDatabase database)
        {
            if(_databases != null)
            {
                _databases.Add(database.Name, database);
            }
        }


        //Loading a Database from File with the loader
        public void loadDatabase(string DataBaseName)
        {
            _loader.ReloadDataBase(_connector.loadData(DataBaseName));
        }

        //get the Key from the PasswordDB
        internal string GetKeyFromPasswordDB(string DataBaseName)
        {
            List<Dictionary<string, string>> PasswordDBData = _databases["PasswordDB"].ReadAll();
            foreach(Dictionary<string, string> data in PasswordDBData)
            {
                if (data["DBName"] == DataBaseName)
                {
                    return data["Passwords"];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The database isn't in the PasswordDB");
                }
            }
            throw new ArgumentOutOfRangeException("The database isn't in the PasswordDB");
        }
    }
}
