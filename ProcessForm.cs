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
using Microsoft.Win32;

namespace ProcessPair
{
    public partial class ProcessForm : Form
    {
        //Startup registry key and value
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "ProcessPair";
        private static List<object> installedApplications;
        List<ProcessPair> ProcessList;
       
        string filepath = Path.Combine(Path.GetTempPath(),"ProcessPair", "data", "processlist.json");
        public ProcessForm()
        {
            InitializeComponent();
            LoadProceessFromFile();
            WaitForProcess(ProcessList);            
            BindTable();
            startupBox.Checked = BootOnWindows();
            
        }
        void WaitForProcess(List<ProcessPair> processList)
        {
            ProcessList.ForEach(p =>
            {
                p.notify = showBalloon;
                p.bindUI = BindTable;
                p.WatchForStart();
                p.WatchForEnd();
            });


            //ProcessList.Where(p => p.Dependent != null && p.Independent != null && !p.Dependent.StopProcess).ToList().ForEach(p =>
            //{
            //    p.Dependent.WatchForEnd(ProcessEnded);
            //    p.Independent.WatchForEnd(ProcessEnded);
            //});
            //ProcessList.Where(p => p.Dependent != null && p.Independent != null && p.Dependent.StopProcess).ToList().ForEach(p =>
            //{
            //    p.Independent.WatchForStart(ProcessStarted);
            //    p.Dependent.WatchForStart(ProcessStarted);
            //    if (p.Independent.Running == true && p.Dependent.Running == false)
            //    {
            //        showBalloon($"Start {p.Dependent.Alias ?? p.Dependent.Name}", $"{p.Dependent.Alias ?? p.Dependent.Name } is running without {p.Independent.Name ?? p.Independent.Name}");
            //    }
            //});
        }

        private void showBalloon(ProcessPair pair, string body)
        {
            notifyIcon1.Visible = true;

            notifyIcon1.BalloonTipTitle = "ProcessPair";            

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
                newPair.ReLaunch = relaunchBox.Checked;
                newPair.StopProcess = stopProcessBox.Checked;
                ProcessList.Add(newPair);
                SaveProcessToFile();
                newPair.WatchForStart();
                newPair.WatchForEnd();
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
            using (DataTable Dt = new DataTable())
            {
                Dt.Columns.Add("Dependant", typeof(string)).ReadOnly = true;
                Dt.Columns.Add("Dependant Running", typeof(bool)).ReadOnly = true;
                Dt.Columns.Add("Independant", typeof(string)).ReadOnly = true;
                Dt.Columns.Add("Independant Running", typeof(bool)).ReadOnly = true;
                Dt.Columns.Add("Dependant Kill Instead of Launch?", typeof(bool)).ReadOnly = true;
                Dt.Columns.Add("Relaunch if killed?", typeof(bool)).ReadOnly = true;
                ProcessList.ForEach(p => Console.WriteLine($"Dependent: {p.Dependent.Name} {p.Dependent.Running}"));
                ProcessList.ForEach(p => Console.WriteLine($"Independent: {p.Independent.Name} {p.Independent.Running}"));
                ProcessList.Where(p => p.Dependent != null && p.Independent != null).ToList().ForEach(p => addRow(Dt, p));
                return Dt;
            }
        }
        private void addRow(DataTable dt, ProcessPair p)
        {
            var row = dt.NewRow();
            row[0] = p.Dependent.Name;
            row[1] = p.Dependent.Running;
            row[2] = p.Independent.Name;
            row[3] = p.Independent.Running;
            row[4] = p.StopProcess;
            row[5] = p.ReLaunch;
            dt.Rows.Add(row);
        }
        private void BindTable()
        {
            if (gridProcess.DataSource != null)
            {
                gridProcess.Invoke(new Action(() => gridProcess.DataSource = GetDataTable()));
            }
            else
            {
                gridProcess.DataSource = GetDataTable();
            }
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
        private static void SetStartup(bool add)
        {
            if (add)
            {
                //Set the application to run at startup
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
                key.SetValue(StartupValue, Application.ExecutablePath.ToString());
            }
            else
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
                key.DeleteValue(StartupValue);
            }
        }
        private static bool BootOnWindows()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true))
            {
                return key.GetValue(StartupValue) == null ? false : true;
            }
        }

        private void startupBox_CheckedChanged(object sender, EventArgs e)
        {
            if (startupBox.Checked)
                SetStartup(true);
            else
                SetStartup(false);
        }

        private void btnDependant_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if(file.ShowDialog() == DialogResult.OK)
            {
                txtDependant.Text = file.FileName;
            }
        }

        private void btnIndependant_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                txtIndependant.Text = file.FileName;
            }
        }
    }
}

