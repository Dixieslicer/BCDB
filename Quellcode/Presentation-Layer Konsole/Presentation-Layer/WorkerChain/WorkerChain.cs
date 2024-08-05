using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.WorkerChain
{
    internal class WorkerChain
    {
        //List of all registered Workers
        private List<IWorker>? Workers;

        //Execution Method
        internal void execute(object sender, EventArgs e)
        {
            if(Workers == null)
            {
                throw new InvalidOperationException("There aren't Workers set");
            }else if(Workers.Count == 0)
            {
                throw new InvalidOperationException("There aren't Workers set");
            }
            foreach(IWorker worker in Workers)
            {
                worker.execute(e);
            }
        }

        //Contructor with a List of Workers
        internal WorkerChain(List<IWorker> workers)
        {
            Workers = workers;
        }
        //Constructor for use without Workers
        internal WorkerChain()
        {
            Workers = new List<IWorker>();
        }

        //Function to Add Workers
        internal void AddWorkers(IWorker worker)
        {
            Workers.Add(worker);
        }
    }
}
