using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Presentation_Layer;
using Presentation_Layer.Databases;

namespace Presentation_Layer.DataBaseEvents
{
    internal class NewDataBaseEventArgs : EventArgs
    {
        //Eventtype for determing which event has to be created when loading from file
        private string _eventtype = "NewDataBase";
        //Name of the new Database
        private string _databaseName;
        //Password of the new Database
        private string _secretKey;
        //type of the new Database
        DataPresentationLayer.DataBasetype _databasetype;
        //fields for the new Database, additional fields when it is an AccountingDB
        private List<string> _fields;
        //list with fields, that are optional
        private List<string> _optionalFields;
        //The Database, when ist is created
        private IDatabase? _createdDatabase;
        //marker if the event is newly created or laoded from file
        private bool _isreloaded;

        //Properties for JSON-Serialization
        public string Eventtype
        {
            get
            {
                return _eventtype;
            }
        }
        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
        }
        public string SecretKey
        {
            get
            {
                return _secretKey;
            }
        }
        public string Databasetype
        {
            get
            {
                return _databasetype.ToString();
            }
        }

        public List<string> Fields
        {
            get
            {
                return _fields;
            }
        }

        public List<string> OptionalFields
        {
            get
            {
                return _optionalFields;
            }
        }

        //internal Properties
        internal IDatabase CreatedDatabase
        {
            get
            {
                if(_createdDatabase == null)
                {
                    throw new ArgumentNullException("Es wurde keine Datenbank erstellt");
                }
                return _createdDatabase;
            }
        }

        internal DataPresentationLayer.DataBasetype DatabaseType
        {
            get
            {
                return _databasetype;
            }
        }

        internal bool Isreloaded
        {
            get
            {
                return _isreloaded;
            }
        }

        //Constructor for Event
        public NewDataBaseEventArgs(string name, string secretKey, DataPresentationLayer.DataBasetype dataBasetype, List<string> fields, List<string> optionalFields, bool isreloaded)
        {
            _databaseName = name;
            _secretKey = secretKey;
            _databasetype = dataBasetype;
            _fields = fields;
            _optionalFields = optionalFields;
            _isreloaded = isreloaded;
        }

        //function to give the Database to the next Levels in the WorkerChain
        internal void setDataBase(IDatabase createdDatabase)
        {
            _createdDatabase = createdDatabase;
        }
    }
}
