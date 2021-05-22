
namespace SzachyAI
{
    partial class HelpModeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpModeForm));
            this.scanButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpButton = new System.Windows.Forms.Button();
            this.commandLabel = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.recognisedBoardPcBox = new System.Windows.Forms.PictureBox();
            this.boardLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.recognisedBoardPcBox)).BeginInit();
            this.SuspendLayout();
            // 
            // scanButton
            // 
            resources.ApplyResources(this.scanButton, "scanButton");
            this.scanButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.scanButton.Name = "scanButton";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(134)))), ((int)(((byte)(39)))));
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // helpButton
            // 
            resources.ApplyResources(this.helpButton, "helpButton");
            this.helpButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.helpButton.Name = "helpButton";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // commandLabel
            // 
            resources.ApplyResources(this.commandLabel, "commandLabel");
            this.commandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.commandLabel.Name = "commandLabel";
            // 
            // notifyIcon1
            // 
            resources.ApplyResources(this.notifyIcon1, "notifyIcon1");
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // recognisedBoardPcBox
            // 
            this.recognisedBoardPcBox.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.recognisedBoardPcBox, "recognisedBoardPcBox");
            this.recognisedBoardPcBox.Name = "recognisedBoardPcBox";
            this.recognisedBoardPcBox.TabStop = false;
            // 
            // boardLabel
            // 
            resources.ApplyResources(this.boardLabel, "boardLabel");
            this.boardLabel.Name = "boardLabel";
            // 
            // HelpModeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.Controls.Add(this.boardLabel);
            this.Controls.Add(this.recognisedBoardPcBox);
            this.Controls.Add(this.commandLabel);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.scanButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HelpModeForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelpModeForm_FormClosing);
            this.Load += new System.EventHandler(this.HelpModeForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseUp);
            this.Resize += new System.EventHandler(this.HelpModeForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.recognisedBoardPcBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label commandLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.PictureBox recognisedBoardPcBox;
        private System.Windows.Forms.Label boardLabel;
    }
}