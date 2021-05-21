using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class MenuForm : Form
    {
        BotModeForm botForm;
        HelpModeForm helpForm;
        Form1 debugForm;
        SettingsForm settingsForm;
        AboutForm aboutForm;

        //Make "FormBorderStyle = None" form dragable
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public MenuForm()
        {
            InitializeComponent();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void helpModeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            helpForm.Show();
            
        }

        private void botModeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            botForm.Show();
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            botForm = new BotModeForm();
            helpForm = new HelpModeForm();
            debugForm = new Form1();
            settingsForm = new SettingsForm();
            aboutForm= new AboutForm();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            debugModeButton.Visible = Settings.EnableDebugMode;
        }

        private void MenuForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void MenuForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void MenuForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void debugModeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            debugForm.Show();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            settingsForm.Show();
        }
    }
}
