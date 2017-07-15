﻿using System;
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
            ProcessList.Where(p => p.Dependent != null && p.Independent != null).ToList().ForEach(p =>
            {
                WatchForProcessStart(p);
                WatchForProcessEnd(p);
            });
        }
        private void WatchForProcessStart(ProcessPair pair)
        {
            WatchForProcessStart(pair.Dependent);
            WatchForProcessStart(pair.Independent);
            if (pair.Independent.Running == false && pair.Dependent.Running == true)
            {
                showBalloon($"Start {pair.Dependent.Alias ?? pair.Dependent.Name} dummy", $"{pair.Dependent.Alias ?? pair.Dependent.Name } is running without {pair.Independent.Name ?? pair.Independent.Name}");
            }
        }
        private void WatchForProcessEnd(ProcessPair pair)
        {
            WatchForProcessEnd(pair.Dependent);
            WatchForProcessEnd(pair.Independent);
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
            ProcessList.Where(x => x.Dependent.Name == processName).All(p => { p.Dependent.Running = false; return true; });
            ProcessList.Where(x => x.Independent.Name == processName).All(p => { p.Independent.Running = false; return true; });
            Console.WriteLine(String.Format("{0} process ended", processName));
        }

        private void ProcessStarted(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            var startedPair = ProcessList.Where(x => x.Dependent.Name == processName);
            foreach (var started in startedPair)
            {
                if (started.Independent.Running == false)
                {
                    showBalloon($"Start {started.Dependent.Alias ?? started.Dependent.Name} dummy", $"{started.Dependent.Alias ?? started.Dependent.Name } is running without {started.Independent.Name ?? started.Independent.Name}");
                }
            }
            ProcessList.Where(x => x.Dependent.Name == processName).All(p => { p.Dependent.Running = true; return true; });
            ProcessList.Where(x => x.Independent.Name == processName).All(p => { p.Independent.Running = true; return true; });
            Console.WriteLine(String.Format("{0} process started", processName));
        }
        private void showBalloon(string title, string body)
        {
            notifyIcon1.Visible = true;

            if (title != null)
            {
                notifyIcon1.BalloonTipTitle = title;
            }

            if (body != null)
            {
                notifyIcon1.BalloonTipText = body;
            }

            notifyIcon1.ShowBalloonTip(30000);
        }
        private void ProcessForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(200);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (TextIsValid(txtDependant.Text) && TextIsValid(txtIndependant.Text))
            {
                if (!Valid())
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
            {
                ProcessList = JsonConvert.DeserializeObject<List<ProcessPair>>(File.ReadAllText(filepath)) ?? new List<ProcessPair>();
            }
            else
            {
                ProcessList = new List<ProcessPair>();
            }
        }
        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Dependant", typeof(string)).ReadOnly = true;
            dt.Columns.Add("Dependant Running", typeof(bool)).ReadOnly = true;
            dt.Columns.Add("Independant", typeof(string)).ReadOnly = true;
            dt.Columns.Add("Independant Running", typeof(bool)).ReadOnly = true;
            ProcessList.Where(p => p.Dependent != null && p.Independent != null).ToList().ForEach(p => addRow(dt, p));
            return dt;
        }
        private void addRow(DataTable dt, ProcessPair p)
        {
            var row = dt.NewRow();
            row[0] = p.Dependent.Name;
            row[1] = p.Dependent.Running;
            row[2] = p.Independent.Name;
            row[3] = p.Independent.Running;
            dt.Rows.Add(row);
        }
        private void BindTable()
        {
            gridProcess.DataSource = GetDataTable();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int selectedIndex = gridProcess.SelectedCells[0].RowIndex;
            var row = gridProcess.Rows[selectedIndex];
            ProcessList.RemoveAll(x => x.Dependent.Name == (string)row.Cells["Dependant"].Value && x.Independent.Name == (string)row.Cells["Independant"].Value);
            SaveProcessToFile();
            BindTable();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }
        private void Quit_Menu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool Valid()
        {
            if (ProcessList.Any(x => x.Dependent.Name == txtDependant.Text && x.Independent.Name == txtIndependant.Text))
            {
                lblError.Text = "Process pair already monitored";
                return false;
            }
            return true;
        }

    }
}

