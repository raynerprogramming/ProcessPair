using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;

namespace ProcessPair
{
    class LoadedProcess
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string ExePath { get; set; }
        [JsonIgnoreAttribute]
        public bool? Running { get; set; }

        public LoadedProcess()
        {
            Running = false;
        }
        public LoadedProcess(string path) : this()
        {
            Name = Path.GetFileName(path);
            ExePath = path;
        }

        public LoadedProcess(string path, string alias) : this(path)
        {
            Alias = alias;
        }

        public LoadedProcess(string path, string alias, bool running) : this(path, alias)
        {
            Running = running;
        }
        public void Start()
        {
            if (!Running.GetValueOrDefault())
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.WorkingDirectory = Path.GetDirectoryName(ExePath);
                psi.FileName = ExePath;
                Process.Start(psi);
            }
        }
        public void KillFirst()
        {
            if (Running.GetValueOrDefault())
            {
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Name)).First().Kill();
            }
        }
        public void KillAll()
        {
            if (Running.GetValueOrDefault())
            {
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Name)).All(p => { p.Kill(); return true; });
            }
        }
        public ManagementEventWatcher WatchForEnd(EventArrivedEventHandler ProcessEnded)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceDeletionEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + Name + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";

            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessEnded;
            watcher.Start();
            return watcher;
        }
        public ManagementEventWatcher WatchForStart(EventArrivedEventHandler ProcessStarted)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceCreationEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + Name + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";
            //initialize already running processes
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Name)).Any())
            {
                Running = true;
            }
            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessStarted;
            watcher.Start();
            return watcher;
        }
    }
}
