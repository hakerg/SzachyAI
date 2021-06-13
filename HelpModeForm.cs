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
    public partial class HelpModeForm : Form
    {
        //Make form dragable at every point
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MenuForm menuForm;

        public HelpModeForm(MenuForm menuForm)
        {
            this.menuForm = menuForm;
            InitializeComponent();
        }
        private void HelpModeForm_Load(object sender, EventArgs e)
        {
            recognisedBoardPcBox.Visible = Settings.enableDebugMode;
            boardLabel.Visible = Settings.enableDebugMode;
            if (!Settings.enableDebugMode) {
                Size = new Size(Width, Height - 200);
                commandLabel.Location = new Point(commandLabel.Location.X, commandLabel.Location.Y - 200);
            }
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            menuForm.detectBoardOnce = true;
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            menuForm.giveHintOnce = true;
        }

        private void HelpModeForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void HelpModeForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void HelpModeForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void HelpModeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Invoke((Action)delegate { menuForm.drawing.Clear(); });
            menuForm.Show();
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void HelpModeForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        public void UpdateStatus(string status) {
            commandLabel.Text = status;
            notifyIcon1.Text = "ChessAI: " + status;
        }

        public void UpdateImage(Bitmap image) {
            recognisedBoardPcBox.Image = image;
        }
    }
}
