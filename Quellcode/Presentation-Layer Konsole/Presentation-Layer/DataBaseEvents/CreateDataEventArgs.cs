using Presentation_Layer.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.DataBaseEvents
{
    internal class CreateDataEventArgs:EventArgs
    {
        //Eventtype for determing which event has to be created when loading from file
        private string _eventtype = "CreateData";
        //Database to which the Data should be added
        private IDatabase _database;
        //Data to add
        private Dictionary<string, string> _data;
        //Key for the new Data
        private string _key;
        //marker if the Command is created or reloaded
        private bool _isreloaded;
        
        //Public Properties for the JSONSerializer
        public string Eventtype
        {
            get
            {
                return _eventtype;
            }
        }
        public string Key
        {
            get 
            { 
                return _key; 
            }
        }

        public Dictionary<string, string> Data
        {
            get
            {
                return _data;
            }
        }

        public string DataBaseName
        {
            get
            {
                return _database.Name;
            }
        }

        //internal Properties for the Project
        internal IDatabase Database
        {
            get
            {
                return _database;
            }
        }

        internal bool Isreloaded
        {
            get
            {
                return _isreloaded;
            }
        }

        //Constructor for the EventArgs
        public CreateDataEventArgs(Dictionary<string, string> data, string key, IDatabase database, bool isreloaded)
        {
            _data = data;
            _key = key;
            _database = database;
            _isreloaded = isreloaded;
        }
    }
}
