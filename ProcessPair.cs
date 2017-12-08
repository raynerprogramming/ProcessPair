using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ProcessPair
{
    class ProcessPair
    {
        public LoadedProcess Dependent { get; set; }
        public LoadedProcess Independent { get; set; }
        public bool StopProcess { get; set; }
        public bool ReLaunch { get; set; }
        public Action bindUI;
        public Action<ProcessPair,string> notify;
        public ProcessPair(LoadedProcess dependent, LoadedProcess independent)
        {
            Dependent = dependent;
            Independent = independent;
            StopProcess = false;
            ReLaunch = false;

        }
        public void WatchForStart()
        {
            WatchForProcessStart(Dependent);
            WatchForProcessStart(Independent);            
        }
        public void WatchForEnd()
        {
            WatchForProcessEnd(Dependent);
            WatchForProcessEnd(Independent);
        }
        private ManagementEventWatcher WatchForProcessStart(LoadedProcess process)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceCreationEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + process.Name + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";
            //initialize already running processes
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(process.Name)).Any())
            {
                process.Running = true;
            }
            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessStarted;
            watcher.Start();
            return watcher;
        }
        private ManagementEventWatcher WatchForProcessEnd(LoadedProcess process)
        {
            string queryString =
                "SELECT TargetInstance" +
                "  FROM __InstanceDeletionEvent " +
                "WITHIN  10 " +
                " WHERE TargetInstance ISA 'Win32_Process' " +
                "   AND TargetInstance.Name = '" + process.Name + "'";

            // The dot in the scope means use the current machine
            string scope = @"\\.\root\CIMV2";

            // Create a watcher and listen for events
            ManagementEventWatcher watcher = new ManagementEventWatcher(scope, queryString);
            watcher.EventArrived += ProcessEnded;
            watcher.Start();
            return watcher;
        }
        private void ProcessEnded(object sender, EventArrivedEventArgs e)
        {
        ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            if (Dependent.Name == processName)
            {
                Dependent.Running = false;
            }
            if (Independent.Name == processName)
            {
                Independent.Running = false;
            }
            if (StopProcess && ReLaunch && !Independent.Running.GetValueOrDefault())
            {
                notify(this, $"Relaunching {Dependent.Name} with {Independent.Name} ending");
                Dependent.Start();
                Dependent.Running = true;
            }
            
            Console.WriteLine(String.Format("{0} process ended", processName));
            bindUI();
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            if (StopProcess && Independent.Name==processName)
            {
                notify(this, $"Killing {Dependent.Name} with {Independent.Name} start");
                Dependent.KillFirst();
            }
            else
            {
                notify(this, $"Starting {Dependent.Name} with {Independent.Name} start");
                Dependent.Start();
            }

            if (Dependent.Name == processName)
            {
                Dependent.Running = true;
            }
            if (Independent.Name == processName)
            {
                Independent.Running = true;
            }
            Console.WriteLine(String.Format("{0} process started", processName));
            bindUI();
        }
    }
}
