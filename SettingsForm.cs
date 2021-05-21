using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace SzachyAI
{
    public partial class SettingsForm : Form
    {
        MenuForm menuForm = new MenuForm();
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            menuForm.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Polish")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
            }
            else if(comboBox1.SelectedItem.ToString() == "English")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            }
            Controls.Clear();
            InitializeComponent();
            debugModeCheckBox.Checked = Settings.EnableDebugMode;
            menuForm = new MenuForm();
        }

        private void debugModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.EnableDebugMode = debugModeCheckBox.Checked;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            debugModeCheckBox.Checked = Settings.EnableDebugMode;
        }
    }
}
