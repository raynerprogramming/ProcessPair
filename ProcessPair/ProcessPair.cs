using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class ProcessPair
    {
        public LoadedProcess Dependant { get; set; }
        public LoadedProcess Independant { get; set; }

        public ProcessPair(LoadedProcess dependant, LoadedProcess independant)
        {
            Dependant = dependant;
            Independant = independant;
        }
    }
}
