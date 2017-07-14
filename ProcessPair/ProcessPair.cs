using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class ProcessPair
    {
        public LoadedProcess Dependent { get; set; }
        public LoadedProcess Independent { get; set; }

        public ProcessPair(LoadedProcess dependent, LoadedProcess independent)
        {
            Dependent = dependent;
            Independent = independent;
        }
    }
}
