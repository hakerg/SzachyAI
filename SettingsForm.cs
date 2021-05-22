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
        MenuForm menuForm;

        private void LoadSettings() {
            if (Thread.CurrentThread.CurrentUICulture.Name == "pl-PL") {
                comboBox1.SelectedIndex = 0;
            } else if (Thread.CurrentThread.CurrentUICulture.Name == "en") {
                comboBox1.SelectedIndex = 1;
            }
            debugModeCheckBox.Checked = Settings.enableDebugMode;
            borderCheckBox.Checked = Settings.showBorder;
            hintModeComboBox.SelectedIndex = (int)Settings.hintMode;
            mouseComboBox.SelectedIndex = (int)Settings.mouseMode;
            timeNumericUpDown.Value = Settings.eventTime;
        }

        public SettingsForm(MenuForm menuForm)
        {
            this.menuForm = menuForm;
            InitializeComponent();
            LoadSettings();
        }

        private void ChangeLanguage(CultureInfo language) {
            if (language.Name != Thread.CurrentThread.CurrentUICulture.Name) {
                Thread.CurrentThread.CurrentUICulture = language;
                Controls.Clear();
                InitializeComponent();
                LoadSettings();
                menuForm.ChangeLanguage(language);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Polski") {
                ChangeLanguage(new CultureInfo("pl-PL"));
            }
            else if(comboBox1.SelectedItem.ToString() == "English") {
                ChangeLanguage(new CultureInfo("en"));
            }
        }

        private void debugModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.enableDebugMode = debugModeCheckBox.Checked;
        }

        private void borderCheckBox_CheckedChanged(object sender, EventArgs e) {
            Settings.showBorder = borderCheckBox.Checked;
        }

        private void hintModeComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.hintMode = (HintMode)hintModeComboBox.SelectedIndex;
        }

        private void mouseComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Settings.mouseMode = (MouseMode)mouseComboBox.SelectedIndex;
        }

        private void timeNumericUpDown_ValueChanged(object sender, EventArgs e) {
            Settings.eventTime = (int)timeNumericUpDown.Value;
        }
    }
}
