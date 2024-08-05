using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.Databases
{
    internal class KVDB:IDatabase
    {
        //list with the datafields in the DB
        private List<string> _fields;
        //list with the fields which are optional
        private List<string> _opionalfields;
        //name of the db
        private string _name;
        //Data of the DB
        private Dictionary<string, Dictionary<string, string>> _data;
        //Datapattern of the DB
        private Dictionary<string, string> _dataPattern;
        //actual key of the Database
        private int _key = 0;

        //Constructor
        public KVDB(string name, List<string> fields, List<string> opionalfields)
        {
            _name = name;
            _fields = fields;
            _opionalfields = opionalfields;
            _dataPattern = new Dictionary<string, string>();
            foreach(string field in _fields)
            {
                _dataPattern.Add(field, "");
            }
            _data = new Dictionary<string, Dictionary<string, string>>();
        }

        //Public Property for implementing the Interface IDatabase
        public string Name
        {
            get { return _name; }
        }

        //Function to add Data to the DB
        public void AddData(string key, Dictionary<string, string> value)
        {
            _data.Add(key, value);
            _key++;
        } 

        //Function to Update existing data in the DB
        public void UpdateData(string key, Dictionary<string, string> value)
        {
            //Checking if the key is in the Database
            if(_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("There is no such key in the Database");
            }
            
        }

        //Function for deleting data in the DB
        public void DeleteData(string key)
        {
            _data.Remove(key);
        }

        //Function to get the dataPattern of the Database
        public Dictionary<string, object> GetDataPattern()
        {
            return new Dictionary<string, object>()
            {
                { "optionalFields", _opionalfields },
                {"dataDictionary", _dataPattern }
            };

        }

        //Read the data from one Key
        public Dictionary<string, string> ReadKeyBased(string key)
        {
            return _data[key];
        }

        //Read the full data
        public List<Dictionary<string, string>> ReadAll()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>> ();
            foreach (string key in _data.Keys)
            {
                result.Add(_data[key]);
            }
            return result;
        }

        //returns the actual Key
        public string GetActualKey()
        {
            return _key.ToString();
        }
    }
}
