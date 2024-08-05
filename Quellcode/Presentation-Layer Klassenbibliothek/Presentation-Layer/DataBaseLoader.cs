using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Presentation_Layer.DataBaseEvents;
using Presentation_Layer.Databases;
using Presentation_Layer.WorkerChain;

namespace Presentation_Layer
{
    internal class DataBaseLoader
    {
        //properties
        private WorkerChain.WorkerChain _workers;
        //Events for the Database
        internal event NewDataBaseEvent? CreateNewDataBase;
        internal event CreateDataEvent? CreateNewData;
        internal event UpdateDataEvent? UpdateData;
        internal event RemoveDataEvent? RemoveData;

        //Constructor
        public DataBaseLoader(WorkerChain.WorkerChain workers)
        {
        _workers = workers;
        CreateNewDataBase += _workers.execute;
        CreateNewData += _workers.execute;
        UpdateData += _workers.execute;
        RemoveData += _workers.execute;
        }

        //Delegates for the DataBaseEvents
        public delegate void NewDataBaseEvent(object sender, NewDataBaseEventArgs e);
        public delegate void CreateDataEvent(object sender, CreateDataEventArgs e);
        public delegate void UpdateDataEvent(object sender, UpdateDataEventArgs e);
        public delegate void RemoveDataEvent(object sender, RemoveDataEventArgs e);

        //Sorting the loaded Events
        internal void ReloadDataBase(List<string> eventlist)
        {
            foreach(string EventArgsJson in eventlist)
            {
                var JSONDom = JsonSerializer.Deserialize<JsonObject>(EventArgsJson);
                string eventType = (string)JSONDom["Eventtype"];
                switch (eventType)
                {
                    case "CreateData":
                        ReloadNewData(JSONDom);
                        break;
                    case "RemoveData":
                        ReloadRemoveData(JSONDom);
                        break;
                    case "NewDataBase":
                        ReloadDataBase(JSONDom);
                        break;
                    case "UpdateData":
                        ReloadUpdateData(JSONDom);
                        break;
                    default:
                        throw new Exception("this is not an valid eventtype");
                }
            }
        }

        //rebuild the NewDataBase Command
        private void ReloadDataBase(JsonObject JSON)
        {
            string? Databasename = (string?)JSON["DatabaseName"];
            string secretKey;
            if (Databasename == "PasswordDB")
            {
                secretKey = "Passwords";
            }
            else
            {
                secretKey = DataPresentationLayer.Instance.GetKeyFromPasswordDB(Databasename);
            }
            DataPresentationLayer.DataBasetype? DatabaseType = null;

            if ((string)JSON["Databasetype"] == "KVDB")
            {
                DatabaseType = DataPresentationLayer.DataBasetype.KVDB;
            }else if ((string)JSON["Databasetype"] == "Accounting")
            {
                DatabaseType = DataPresentationLayer.DataBasetype.Accounting;
            }
            List<string> fields = JsonSerializer.Deserialize<List<string>>(JSON["Fields"]);
            List<string> optionalFields = JsonSerializer.Deserialize<List<string>>(JSON["OptionalFields"]);
            bool isreloaded = true;
            NewDataBaseEventArgs e = new NewDataBaseEventArgs(Databasename, secretKey, (DataPresentationLayer.DataBasetype)DatabaseType, fields, optionalFields, isreloaded);
            CreateNewDataBase(this, e);
        }

        //rebuild the CreateData Command
        private void ReloadNewData(JsonObject JSON)
        {
            Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(JSON["Data"]);
            IDatabase Database = DataPresentationLayer.Instance.getDataBase((string)JSON["DataBaseName"]);
            bool isreloaded = true;
            string key = Database.GetActualKey();
            CreateDataEventArgs e = new CreateDataEventArgs(data, key, Database, isreloaded);
            CreateNewData(this, e);
        }

        //rebuild the UpdateData Command
        private void ReloadUpdateData(JsonObject JSON)
        {
            Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(JSON["Data"]);
            string key = (string)JSON["Key"];
            IDatabase database = DataPresentationLayer.Instance.getDataBase((string)JSON["DataBaseName"]);
            bool isreloaded = true;
            UpdateDataEventArgs e = new UpdateDataEventArgs(data, key, database, isreloaded);
            UpdateData(this, e);
        }

        //rebuild the RemoveData Command
        private void ReloadRemoveData(JsonObject JSON)
        {
            string key = (string)JSON["Key"];
            IDatabase database = DataPresentationLayer.Instance.getDataBase((string)JSON["DataBaseName"]);
            bool isreloaded = true;
            RemoveDataEventArgs e = new RemoveDataEventArgs(key, database, isreloaded);
            RemoveData(this, e);
        }
    }
}
