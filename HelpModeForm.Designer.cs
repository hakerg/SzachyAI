
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
            this.scanButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpButton = new System.Windows.Forms.Button();
            this.commandLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // scanButton
            // 
            this.scanButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scanButton.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.scanButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.scanButton.Location = new System.Drawing.Point(28, 12);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(200, 45);
            this.scanButton.TabIndex = 1;
            this.scanButton.Text = "Scan for board";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(134)))), ((int)(((byte)(39)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(15, 300);
            this.panel1.TabIndex = 2;
            // 
            // helpButton
            // 
            this.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.helpButton.Font = new System.Drawing.Font("Segoe UI", 13.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.helpButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.helpButton.Location = new System.Drawing.Point(270, 12);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(200, 45);
            this.helpButton.TabIndex = 3;
            this.helpButton.Text = "Help me!";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // commandLabel
            // 
            this.commandLabel.AutoSize = true;
            this.commandLabel.Font = new System.Drawing.Font("Segoe UI", 16.16F);
            this.commandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(38)))), ((int)(((byte)(74)))));
            this.commandLabel.Location = new System.Drawing.Point(30, 207);
            this.commandLabel.Name = "commandLabel";
            this.commandLabel.Size = new System.Drawing.Size(127, 37);
            this.commandLabel.TabIndex = 4;
            this.commandLabel.Text = "Waiting...";
            // 
            // HelpModeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(229)))), ((int)(((byte)(231)))));
            this.ClientSize = new System.Drawing.Size(482, 253);
            this.Controls.Add(this.commandLabel);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.scanButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "HelpModeForm";
            this.Text = "HelpMode";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelpModeForm_FormClosing);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HelpModeForm_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label commandLabel;
    }
}