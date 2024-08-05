using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Data_Security_Layer;
using Presentation_Layer.DataBaseEvents;
using Presentation_Layer.Databases;

namespace Presentation_Layer.WorkerChain
{
    internal class DSLConnector: IWorker
    {
        //Instance of the Databasesystem
        private DataSecurityLayer DSL;
        
        //Constructor
        internal DSLConnector(string DatabasesystemPath, string DatabaseSystemName, string SecretKey)
        {
            DSL = DataSecurityLayer.getDatabaseSystem(DatabasesystemPath, DatabaseSystemName, SecretKey);
        }

        //Executing the given Command
        public void execute(EventArgs e)
        {
            //filter out  Nulls
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            //Handling new Databases
            else if (e.GetType() == typeof(NewDataBaseEventArgs))
            {
                NewDataBaseEventArgs newDataBase = (NewDataBaseEventArgs)e;
                if (newDataBase.Isreloaded)
                {
                    DataPresentationLayer.Instance.addDatabase(newDataBase.CreatedDatabase);
                    return;
                }
                DSL.newDatabase(newDataBase.DatabaseName, newDataBase.SecretKey);
                DataPresentationLayer.Instance.addDatabase(newDataBase.CreatedDatabase);
                string JSON = JsonSerializer.Serialize(newDataBase);
                DSL.AddData(newDataBase.DatabaseName, newDataBase.SecretKey, JSON);
            }
            //Handling new Data
            else if (e.GetType() == typeof(CreateDataEventArgs))
            {
                CreateDataEventArgs CreateData = (CreateDataEventArgs)e;
                if (CreateData.Isreloaded)
                {
                    return;
                }
                string JSON = JsonSerializer.Serialize(CreateData);
                string secretKey;
                if (CreateData.DataBaseName == "PasswordDB")
                {
                    secretKey = "Passwords";
                }
                else
                {
                    secretKey = DataPresentationLayer.Instance.GetKeyFromPasswordDB(CreateData.DataBaseName);
                }
                DSL.AddData(CreateData.Database.Name, secretKey, JSON);
            }
            //Handling Update Data
            else if(e.GetType() == typeof(UpdateDataEventArgs))
            {
                UpdateDataEventArgs UpdateData = (UpdateDataEventArgs)e;
                if (UpdateData.Isreloaded)
                {
                    return;
                }
                string JSON = JsonSerializer.Serialize(UpdateData);
                string secretKey;
                if (UpdateData.DataBaseName == "PasswordDB")
                {
                    secretKey = "Passwords";
                }
                else
                {
                    secretKey = DataPresentationLayer.Instance.GetKeyFromPasswordDB(UpdateData.DataBaseName);
                }
                DSL.AddData(UpdateData.Database.Name, secretKey, JSON);
            }
            //Handling Remove Commands
            else if (e.GetType() == typeof(RemoveDataEventArgs))
            {
                RemoveDataEventArgs RemoveData = (RemoveDataEventArgs)e;
                if (RemoveData.Isreloaded)
                {
                    return;
                }
                string JSON = JsonSerializer.Serialize(RemoveData);
                string secretKey;
                if (RemoveData.DataBaseName == "PasswordDB")
                {
                    secretKey = "Passwords";
                }
                else
                {
                    secretKey = DataPresentationLayer.Instance.GetKeyFromPasswordDB(RemoveData.DataBaseName);
                }
                DSL.AddData(RemoveData.Database.Name, secretKey, JSON);
            }
        }

        //Loading data for the DataBaseLoader
        internal List<string> loadData(string dataBaseName)
        {
            string secretKey;
            if (dataBaseName == "PasswordDB")
            {
                secretKey = "Passwords";
            }
            else
            {
                secretKey = DataPresentationLayer.Instance.GetKeyFromPasswordDB(dataBaseName);
            }
            return DSL.getData(dataBaseName, secretKey);
        }
    }
}
