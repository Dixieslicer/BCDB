using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.Databases
{
    internal class Account
    {
        //List with entrys on the debit side
        private List<string> _debitEntrys = new List<string>();
        //List with entrys on the credit side
        private List<string> _creditEntrys = new List<string>();
        //Database the Account is a part of, needed to get the journalentrys, when the Account is readed
        private AccountingDB _db;

        //Constructor
        internal Account(AccountingDB db)
        {
            _db = db;
        }

        //function to add entry on the credit side
        internal void addCreditEntry(string key)
        {
            _creditEntrys.Add(key);
        }

        //function to add entry on the debit side
        internal void addDebitEntry(string key)
        {
            _debitEntrys.Add(key);
        }

        //function to remoce entry on the credit side
        internal void removeCreditEntry(string key)
        {
            _creditEntrys.Remove(key);
        }

        //function to remoce entry on the debit side
        internal void removeDebitEntry(string key)
        {
            _debitEntrys.Remove(key);
        }

        //function to read the account. Returns a Dictionary with two Lists, which are representing the debit and the credit side
        internal Dictionary<string, List<Dictionary<string, string>>> ReadAccount()
        {
            //getting a List with the journal entrys on the Credit side
            List<Dictionary<string, string>> CreditEntrys = new List<Dictionary<string, string>>();
            foreach (string key in _creditEntrys)
            {
                CreditEntrys.Add(_db.ReadKeyBased(key));
            }
            //getting a List with the journal entrys on the Debit side
            List<Dictionary<string, string>> DebitEntrys = new List<Dictionary<string, string>>();
            foreach (string key in _debitEntrys)
            {
                DebitEntrys.Add(_db.ReadKeyBased(key));
            }
            //building the Dicitionary with both Lists
            return new Dictionary<string, List<Dictionary<string, string>>>()
            {
                { "CreditEntrys", CreditEntrys },
                {"DebitEntrys", DebitEntrys }
            };
        }
    }
}
