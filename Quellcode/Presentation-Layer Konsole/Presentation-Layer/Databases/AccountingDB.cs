using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation_Layer.Databases;

namespace Presentation_Layer.Databases
{
    internal class AccountingDB:IDatabase
    {
        //additional fields to the set fields for accountingDBs
        private List<string> _additionalFields;
        //List with the fields which are optional
        private List<string> _optionalFields;
        //name of the DB
        private string _name;
        //journal with all entrys to the DB
        private Dictionary<string, Dictionary<string, string>> _journal;
        //Datapattern for the user
        private Dictionary<string, string> _dataPattern;
        //actual key
        private int _key = 0;
        //List with the fields, that must be used in all AccountingDBs
        private static List<string> AccountingDBFields = new List<string>()
        {
            "Amount",
            "DebitAccount",
            "CreditAccount",
            "Date"
        };
        //Dictionary for the Account class
        Dictionary<string, Account> _accounts;
        
        //Constructor
        public AccountingDB(string name, List<string> additionalFields, List<string> optionalFields)
        {
            _name = name;
            _additionalFields = additionalFields;
            _optionalFields = optionalFields;
            _dataPattern = new Dictionary<string, string>();
            List<string> Fields = AccountingDBFields;
            Fields.AddRange(_additionalFields);
            foreach (string field in Fields)
            {
                _dataPattern.Add(field, "");
            }          
            _journal = new Dictionary<string, Dictionary<string, string>>();
            _accounts = new Dictionary<string, Account>();
        }

        //Public Property for implementing the Interface IDatabase
        public string Name
        {
            get
            {
                return _name;
            }
        }

        //Function to Add data to the Database
        public void AddData(string key, Dictionary<string, string> value)
        {
            _journal.Add(key, value);
            _key++;
            //checking if the Account is already existing
            if (_accounts.ContainsKey(value["CreditAccount"]))
            {
                _accounts[value["CreditAccount"]].addCreditEntry(key);
            }
            else
            {
                _accounts[value["CreditAccount"]] = new Account(this);
                _accounts[value["CreditAccount"]].addCreditEntry(key);
            }
            //checking if the Account is already existing
            if (_accounts.ContainsKey(value["DebitAccount"]))
            {
                _accounts[value["DebitAccount"]].addDebitEntry(key);
            }
            else
            {
                _accounts[value["DebitAccount"]] = new Account(this);
                _accounts[value["DebitAccount"]].addDebitEntry(key);
            }
        }

        //Function to update data in the Database
        public void UpdateData(string key, Dictionary<string, string> value)
        {
            //checking if the key is valid
            if (_journal.ContainsKey(key))
            {
                _journal[key] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("There is no such key in the Database");
            }
        }

        //Function to Delete data in the Database
        public void DeleteData(string key)
        {
            //Fetching data to get the Accounts
            Dictionary<string, string> data = _journal[key];
            //deleting from Accounts
            Account CreditAccount = _accounts[data["CreditAccount"]];
            Account DebitAccount = _accounts[data["DebitAccount"]];
            CreditAccount.removeCreditEntry(key);
            DebitAccount.removeDebitEntry(key);
            //deleting from journal
            _journal.Remove(key);
        }

        //Function to get the dataPattern of the Database
        public Dictionary<string, object> GetDataPattern()
        {
            return new Dictionary<string, object>()
            {
                {"optionalFields", _optionalFields},
                {"dataDictionary", _dataPattern }
            };
        }

        //Read the JournalData from one Key
        public Dictionary<string, string> ReadKeyBased(string Key)
        {
            return _journal[Key];
        }

        //read the full journal
        public List<Dictionary<string, string>> ReadAll()
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (string key in _journal.Keys)
            {
                result.Add(_journal[key]);
            }
            return result;
        }

        //Read the Data from one Account
        internal Dictionary<string, List<Dictionary<string,string>>>ReadAccount(string Account)
        {
            return _accounts[Account].ReadAccount();
        }

        //returns the actual Key
        public string GetActualKey()
        {
            return _key.ToString();
        }
    }
}
