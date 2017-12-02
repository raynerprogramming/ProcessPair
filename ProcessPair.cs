using System;
using System.Collections.Generic;
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

        public ProcessPair(LoadedProcess dependent, LoadedProcess independent)
        {
            Dependent = dependent;
            Independent = independent;
            StopProcess = false;
            ReLaunch = false;

        }
        public void WatchForStart()
        {
            if (StopProcess)
            {

            }
            else
            {

            }
        }
        public void WatchForEnd()
        {
            if (StopProcess)
            {

            }
            else
            {

            }
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
            if (StopProcess && ReLaunch)
            {
                Dependent.Start();
            }
            Console.WriteLine(String.Format("{0} process ended", processName));
           // BindTable();
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            if (StopProcess)
            {
               
                Dependent.KillFirst();
            }
            else
            {
                //showBalloon($"Process Pair", $"Starting {started.Dependent.Alias ?? started.Dependent.Name } with {started.Independent.Name ?? started.Independent.Name}");
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
             


            foreach (var started in startedPair)
            {
                if (started.Dependent.Running == false)
                {

                    started.Dependent.Start();
                }
            }
       //     ProcessList.Where(x => x.Dependent.Name == processName).All(p => { p.Dependent.Running = true; return true; });
       //     ProcessList.Where(x => x.Independent.Name == processName).All(p => { p.Independent.Running = true; return true; });
            Console.WriteLine(String.Format("{0} process started", processName));
          //  BindTable();
        }
    }
}
