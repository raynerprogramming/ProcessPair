using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ProcessPair
{
    public partial class ProcessForm : Form
    {
        List<ProcessPair> ProcessList;
        string filepath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "data", "processlist.json");
        public ProcessForm()
        {
            ProcessPair PP = new ProcessPair(new LoadedProcess("TslGame.exe"), new LoadedProcess("obs64.exe"));
            LoadProceessFromFile();            
            //ProcessList = new List<ProcessPair>() { PP };
            WaitForProcess(ProcessList);
            InitializeComponent();
            BindTable();
        }
        void WaitForProcess(List<ProcessPair> processList)
        {
            foreach (var process in processList)
            {
                WatchForProcessStart(process);
                WatchForProcessEnd(process);
            }
        }
        private void WatchForProcessStart(ProcessPair pair)
        {
            WatchForProcessStart(pair.Dependant);
            WatchForProcessStart(pair.Independant);
            if (pair.Independant.Running == false && pair.Dependant.Running==true)
            {
                showBalloon($"Start {pair.Dependant.Alias ?? pair.Dependant.Name} dummy", $"{pair.Dependant.Alias ?? pair.Dependant.Name } is running without {pair.Independant.Name ?? pair.Independant.Name}");
            }
        }
        private void WatchForProcessEnd(ProcessPair pair)
        {
            WatchForProcessEnd(pair.Dependant);
            WatchForProcessEnd(pair.Independant);
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
            ProcessList.Where(x => x.Dependant.Name == processName).All(p => { p.Dependant.Running = false; return true; });
            ProcessList.Where(x => x.Independant.Name == processName).All(p => { p.Independant.Running = false; return true; });
            Console.WriteLine(String.Format("{0} process ended", processName));
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            var startedPair = ProcessList.Where(x => x.Dependant.Name == processName);
            foreach (var started in startedPair)
            {
                if (started.Independant.Running == false)
                {
                    showBalloon($"Start {started.Dependant.Alias ?? started.Dependant.Name} dummy", $"{started.Dependant.Alias ?? started.Dependant.Name } is running without {started.Independant.Name ?? started.Independant.Name}");
                }
            }
            ProcessList.Where(x => x.Dependant.Name == processName).All(p => { p.Dependant.Running = true; return true; });
            ProcessList.Where(x => x.Independant.Name == processName).All(p => { p.Independant.Running = true; return true; });
            Console.WriteLine(String.Format("{0} process started", processName));
        }
        private void showBalloon(string title, string body)
        {
            using (NotifyIcon notifyIcon = new NotifyIcon())
            {
                notifyIcon.Visible = true;

                if (title != null)
                {
                    notifyIcon.BalloonTipTitle = title;
                }

                if (body != null)
                {
                    notifyIcon.BalloonTipText = body;
                }
                notifyIcon.Icon = SystemIcons.Application;
                notifyIcon.ShowBalloonTip(30000);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(TextIsValid(txtDependant.Text) && TextIsValid(txtIndependant.Text))
            {
                if(!Valid())
                {
                    return;
                }
                var newPair = new ProcessPair(new LoadedProcess(txtDependant.Text), new LoadedProcess(txtIndependant.Text));
                ProcessList.Add(newPair);
                SaveProcessToFile();
                WatchForProcessStart(newPair);
                WatchForProcessEnd(newPair);
                BindTable();
            }
        }

        private bool TextIsValid(string text)
        {
            return true;
        }

        private void SaveProcessToFile()
        {
            if (!Directory.Exists(Path.GetDirectoryName(filepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
            }
            File.WriteAllText(filepath, JsonConvert.SerializeObject(ProcessList));
        }
        private void LoadProceessFromFile()
        {
            if (File.Exists(filepath))
                ProcessList = JsonConvert.DeserializeObject<List<ProcessPair>>(File.ReadAllText(filepath));
        }
        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Dependant", typeof(string)).ReadOnly=true;
            dt.Columns.Add("Dependant Running", typeof(bool)).ReadOnly = true;
            dt.Columns.Add("Independant", typeof(string)).ReadOnly = true;            
            dt.Columns.Add("Independant Running", typeof(bool)).ReadOnly = true;
            foreach(var process in ProcessList)
            {
                var row = dt.NewRow();
                row[0] = process.Dependant.Name;
                row[1] = process.Dependant.Running;
                row[2] = process.Independant.Name;
                row[3] = process.Independant.Running;
                dt.Rows.Add(row);
            }
            
            return dt;
        }
        private void BindTable()
        {
            gridProcess.DataSource = GetDataTable();            
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int selectedIndex = gridProcess.SelectedCells[0].RowIndex;
            var row = gridProcess.Rows[selectedIndex];
            ProcessList.RemoveAll(x => x.Dependant.Name == (string)row.Cells["Dependant"].Value && x.Independant.Name == (string)row.Cells["Independant"].Value);
            SaveProcessToFile();
            BindTable();
        }

        private bool Valid()
        {
            if (ProcessList.Any(x => x.Dependant.Name== txtDependant.Text && x.Independant.Name== txtIndependant.Text))
            {
                lblError.Text = "Process pair already monitored";
                return false;
            }
            return true;
        }

    }
}

