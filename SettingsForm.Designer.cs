
namespace SzachyAI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.debugModeCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.borderCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.hintModeComboBox = new System.Windows.Forms.ComboBox();
            this.mouseComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.eventTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.findTimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.eventTimeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.findTimeNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1")});
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // debugModeCheckBox
            // 
            resources.ApplyResources(this.debugModeCheckBox, "debugModeCheckBox");
            this.debugModeCheckBox.Name = "debugModeCheckBox";
            this.debugModeCheckBox.UseVisualStyleBackColor = true;
            this.debugModeCheckBox.CheckedChanged += new System.EventHandler(this.debugModeCheckBox_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // borderCheckBox
            // 
            resources.ApplyResources(this.borderCheckBox, "borderCheckBox");
            this.borderCheckBox.Name = "borderCheckBox";
            this.borderCheckBox.UseVisualStyleBackColor = true;
            this.borderCheckBox.CheckedChanged += new System.EventHandler(this.borderCheckBox_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // hintModeComboBox
            // 
            this.hintModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.hintModeComboBox.FormattingEnabled = true;
            this.hintModeComboBox.Items.AddRange(new object[] {
            resources.GetString("hintModeComboBox.Items"),
            resources.GetString("hintModeComboBox.Items1"),
            resources.GetString("hintModeComboBox.Items2")});
            resources.ApplyResources(this.hintModeComboBox, "hintModeComboBox");
            this.hintModeComboBox.Name = "hintModeComboBox";
            this.hintModeComboBox.SelectedIndexChanged += new System.EventHandler(this.hintModeComboBox_SelectedIndexChanged);
            // 
            // mouseComboBox
            // 
            this.mouseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mouseComboBox.FormattingEnabled = true;
            this.mouseComboBox.Items.AddRange(new object[] {
            resources.GetString("mouseComboBox.Items"),
            resources.GetString("mouseComboBox.Items1")});
            resources.ApplyResources(this.mouseComboBox, "mouseComboBox");
            this.mouseComboBox.Name = "mouseComboBox";
            this.mouseComboBox.SelectedIndexChanged += new System.EventHandler(this.mouseComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // eventTimeNumericUpDown
            // 
            this.eventTimeNumericUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.eventTimeNumericUpDown, "eventTimeNumericUpDown");
            this.eventTimeNumericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.eventTimeNumericUpDown.Name = "eventTimeNumericUpDown";
            this.eventTimeNumericUpDown.ValueChanged += new System.EventHandler(this.timeNumericUpDown_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // findTimeNumericUpDown
            // 
            resources.ApplyResources(this.findTimeNumericUpDown, "findTimeNumericUpDown");
            this.findTimeNumericUpDown.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.findTimeNumericUpDown.Name = "findTimeNumericUpDown";
            this.findTimeNumericUpDown.ValueChanged += new System.EventHandler(this.findTimeNumericUpDown_ValueChanged);
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.findTimeNumericUpDown);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.eventTimeNumericUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mouseComboBox);
            this.Controls.Add(this.hintModeComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.borderCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.debugModeCheckBox);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.eventTimeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.findTimeNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox debugModeCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox borderCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox hintModeComboBox;
        private System.Windows.Forms.ComboBox mouseComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown eventTimeNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown findTimeNumericUpDown;
    }
}