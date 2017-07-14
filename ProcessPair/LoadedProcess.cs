using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class LoadedProcess : PersistedProcess
    {
        public bool? Running { get; set; }

        public LoadedProcess() { }
        public LoadedProcess(string name) :base(name) {
            Running = false;
        }

        public LoadedProcess(string name,string alias) :base(name, alias)
        {
            Running = false;
        }

        public LoadedProcess(string name, string alias, bool Running) : base(name, alias)
        {
            Running = false;
        }

    }
}
