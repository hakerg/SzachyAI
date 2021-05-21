
namespace SzachyAI
{
    partial class BotModeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BotModeForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.scanButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.commandLabel = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.recognisedBoardPcBox = new System.Windows.Forms.PictureBox();
            this.boardLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.recognisedBoardPcBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(8)))), ((int)(((byte)(73)))));
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // scanButton
            // 
            resources.ApplyResources(this.scanButton, "scanButton");
            this.scanButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.scanButton.Name = "scanButton";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // startButton
            // 
            resources.ApplyResources(this.startButton, "startButton");
            this.startButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.startButton.Name = "startButton";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // commandLabel
            // 
            resources.ApplyResources(this.commandLabel, "commandLabel");
            this.commandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.commandLabel.Name = "commandLabel";
            // 
            // notifyIcon1
            // 
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
            // BotModeForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.boardLabel);
            this.Controls.Add(this.recognisedBoardPcBox);
            this.Controls.Add(this.commandLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.panel1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "BotModeForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BotModeForm_FormClosing);
            this.Load += new System.EventHandler(this.BotModeForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BotModeForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BotModeForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BotModeForm_MouseUp);
            this.Resize += new System.EventHandler(this.BotModeForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.recognisedBoardPcBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label commandLabel;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.PictureBox recognisedBoardPcBox;
        private System.Windows.Forms.Label boardLabel;
    }
}