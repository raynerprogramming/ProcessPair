using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace ProcessPair
{
    class LoadedProcess
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ExePath { get; set; }

        [JsonIgnoreAttribute]
        public bool? Running { get; set; }

        public LoadedProcess() {
            Running = false;
        }
        public LoadedProcess(string path) :this() {
            Name = Path.GetFileName(path);
            ExePath = path;
        }

        public LoadedProcess(string path, string alias) :this(path)
        {
            Alias = alias;
        }

        public LoadedProcess(string path, string alias, bool running) :this (path, alias)
        {
            Running = running;
        }

    }
}
