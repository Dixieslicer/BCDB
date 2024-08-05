using Presentation_Layer.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.DataBaseEvents
{
    internal class RemoveDataEventArgs:EventArgs
    {
        //Eventtype for determing which event has to be created when loading from file
        private string _eventtype = "RemoveData";
        //Database the data should be removed from
        private IDatabase _database;
        //Key that should be deleted in the database
        private string _key;
        //marker if the event is newly created or laoded from file
        private bool _isreloaded;
        
        //public Properties for JSONSerializer
        public string Eventtype
        {
            get
            {
                return _eventtype;
            }
        }
        public string DataBaseName
        {
            get
            {
                return _database.Name;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        //Constructor
        public RemoveDataEventArgs(string key, IDatabase database, bool isreloaded)
        {
            _database = database;
            _key = key;
            _isreloaded = isreloaded;
        }

        //internal Properties
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
    }
}
