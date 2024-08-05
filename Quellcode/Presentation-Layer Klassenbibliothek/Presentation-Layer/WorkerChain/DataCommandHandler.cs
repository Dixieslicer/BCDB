using Presentation_Layer.DataBaseEvents;
using Presentation_Layer.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.WorkerChain
{
    internal class DataCommandHandler:IWorker
    {
        public void execute(EventArgs e)
        {
            if (e == null)
            {
                return;
            }
            //Create new Data in the Database
            else if (e.GetType() == typeof(CreateDataEventArgs))
            {
                CreateDataEventArgs CreateData = (CreateDataEventArgs)e;
                CreateData.Database.AddData(CreateData.Key, CreateData.Data);
                return;
            }
            //Update existing Data in the Database
            else if (e.GetType() == typeof(UpdateDataEventArgs))
            {
                UpdateDataEventArgs UpdateData = (UpdateDataEventArgs)e;
                UpdateData.Database.UpdateData(UpdateData.Key, UpdateData.Data);
                return;
            }
            //Remove Data from the Database
            else if (e.GetType() == typeof(RemoveDataEventArgs))
            {
                RemoveDataEventArgs RemoveData = (RemoveDataEventArgs)e;
                RemoveData.Database.DeleteData(RemoveData.Key);
                return;
            }
        }
    }
}
