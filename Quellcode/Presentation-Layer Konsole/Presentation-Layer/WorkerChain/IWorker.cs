using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.WorkerChain
{
    internal interface IWorker
    {
        internal void execute(EventArgs e);
    }
}
