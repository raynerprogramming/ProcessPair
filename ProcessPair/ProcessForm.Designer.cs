using System.Drawing;

namespace ProcessPair
{
    partial class ProcessForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessForm));
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gridProcess = new System.Windows.Forms.DataGridView();
            this.lblError = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startupBox = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnDependant = new System.Windows.Forms.Button();
            this.btnIndependant = new System.Windows.Forms.Button();
            this.txtDependant = new System.Windows.Forms.TextBox();
            this.txtIndependant = new System.Windows.Forms.TextBox();
            this.dlgDependant = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(447, 68);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(447, 283);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Dependant Process:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Independant Process:";
            // 
            // gridProcess
            // 
            this.gridProcess.AllowUserToAddRows = false;
            this.gridProcess.AllowUserToDeleteRows = false;
            this.gridProcess.AllowUserToResizeColumns = false;
            this.gridProcess.AllowUserToResizeRows = false;
            this.gridProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridProcess.Location = new System.Drawing.Point(15, 97);
            this.gridProcess.MultiSelect = false;
            this.gridProcess.Name = "gridProcess";
            this.gridProcess.ReadOnly = true;
            this.gridProcess.Size = new System.Drawing.Size(507, 180);
            this.gridProcess.TabIndex = 6;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(333, 14);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 7;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "Docking ProcessPair";
            this.notifyIcon1.BalloonTipTitle = "ProcessPair";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "ProcessPair";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(98, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(97, 22);
            this.toolStripMenuItem1.Text = "Quit";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.Quit_Menu_Click);
            // 
            // startupBox
            // 
            this.startupBox.AutoSize = true;
            this.startupBox.Location = new System.Drawing.Point(28, 74);
            this.startupBox.Name = "startupBox";
            this.startupBox.Size = new System.Drawing.Size(120, 17);
            this.startupBox.TabIndex = 8;
            this.startupBox.Text = "Start with windows?";
            this.startupBox.UseVisualStyleBackColor = true;
            this.startupBox.CheckedChanged += new System.EventHandler(this.startupBox_CheckedChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnDependant
            // 
            this.btnDependant.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDependant.Location = new System.Drawing.Point(141, 12);
            this.btnDependant.Name = "btnDependant";
            this.btnDependant.Size = new System.Drawing.Size(111, 23);
            this.btnDependant.TabIndex = 9;
            this.btnDependant.Text = "Select Dependant";
            this.btnDependant.UseVisualStyleBackColor = true;
            this.btnDependant.Click += new System.EventHandler(this.btnDependant_Click);
            // 
            // btnIndependant
            // 
            this.btnIndependant.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIndependant.Location = new System.Drawing.Point(141, 42);
            this.btnIndependant.Name = "btnIndependant";
            this.btnIndependant.Size = new System.Drawing.Size(111, 24);
            this.btnIndependant.TabIndex = 10;
            this.btnIndependant.Text = "Select Independant";
            this.btnIndependant.UseVisualStyleBackColor = true;
            this.btnIndependant.Click += new System.EventHandler(this.btnIndependant_Click);
            // 
            // txtDependant
            // 
            this.txtDependant.Location = new System.Drawing.Point(259, 14);
            this.txtDependant.Name = "txtDependant";
            this.txtDependant.Size = new System.Drawing.Size(263, 20);
            this.txtDependant.TabIndex = 11;
            // 
            // txtIndependant
            // 
            this.txtIndependant.Location = new System.Drawing.Point(259, 46);
            this.txtIndependant.Name = "txtIndependant";
            this.txtIndependant.Size = new System.Drawing.Size(263, 20);
            this.txtIndependant.TabIndex = 12;
            // 
            // ProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 355);
            this.Controls.Add(this.txtIndependant);
            this.Controls.Add(this.txtDependant);
            this.Controls.Add(this.btnIndependant);
            this.Controls.Add(this.btnDependant);
            this.Controls.Add(this.startupBox);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.gridProcess);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Name = "ProcessForm";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.ProcessForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridProcess)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gridProcess;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.CheckBox startupBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnDependant;
        private System.Windows.Forms.Button btnIndependant;
        private System.Windows.Forms.TextBox txtDependant;
        private System.Windows.Forms.TextBox txtIndependant;
        private System.Windows.Forms.OpenFileDialog dlgDependant;
    }
}

