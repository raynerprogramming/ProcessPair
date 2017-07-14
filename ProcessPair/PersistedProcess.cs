using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class PersistedProcess
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public PersistedProcess(string name)
        {
            Name = name;
            Alias = name;
        }
        public PersistedProcess()
        {

        }
        public PersistedProcess(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

    }
}

