using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.Databases
{
    internal interface IDatabase
    {
        public string Name { get; }

        public void AddData(string key, Dictionary<string,string> value);
        public void UpdateData(string key, Dictionary<string, string> value);
        public void DeleteData(string key);
        public Dictionary<string, object> GetDataPattern();
        public Dictionary<string,string> ReadKeyBased(string key);
        public List<Dictionary<string, string>> ReadAll();
        public string GetActualKey();
    }
}
