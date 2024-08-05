using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation_Layer.DataBaseEvents;
using Presentation_Layer.Databases;

namespace Presentation_Layer.WorkerChain
{
    internal class DataBaseCommandHandler:IWorker
    {
        public void execute(EventArgs e)
        {
            if(e.GetType() == typeof(NewDataBaseEventArgs))
            {
                NewDataBaseEventArgs newDataBase = e as NewDataBaseEventArgs?? throw new ArgumentNullException("The given Event is null:" + nameof(e));
                //Building the new Database depending on the Databasetype
                switch (newDataBase.DatabaseType)
                {
                    case DataPresentationLayer.DataBasetype.KVDB:
                        KVDB Kdb = new KVDB(newDataBase.DatabaseName, newDataBase.Fields, newDataBase.OptionalFields);
                        newDataBase.setDataBase(Kdb);
                        break;
                    case DataPresentationLayer.DataBasetype.Accounting:
                        AccountingDB Adb = new AccountingDB(newDataBase.DatabaseName, newDataBase.Fields, newDataBase.OptionalFields);
                        newDataBase.setDataBase(Adb);
                        break;
                }
            }

        }
    }
}
