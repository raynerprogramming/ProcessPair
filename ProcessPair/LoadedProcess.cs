using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class LoadedProcess
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        [JsonIgnoreAttribute]
        public bool? Running { get; set; }

        public LoadedProcess() {
            Running = false;
        }
        public LoadedProcess(string name) :this() {
            Name = name;
        }

        public LoadedProcess(string name,string alias) :this(name)
        {
            Alias = alias;
        }

        public LoadedProcess(string name, string alias, bool running) :this (name, alias)
        {
            Running = running;
        }

    }
}
