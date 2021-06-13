using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class BotModeForm : Form
    {
        //Make form dragable at every point
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        MenuForm menuForm;

        public BotModeForm(MenuForm menuForm)
        {
            this.menuForm = menuForm;
            InitializeComponent();
        }
        private void BotModeForm_Load(object sender, EventArgs e)
        {
            recognisedBoardPcBox.Visible = Settings.enableDebugMode;
            boardLabel.Visible = Settings.enableDebugMode;
            if (!Settings.enableDebugMode)
            {
                Size = new Size(Width, Height - 200);
                commandLabel.Location = new Point(commandLabel.Location.X, commandLabel.Location.Y - 200);
            }
        }
        private void scanButton_Click(object sender, EventArgs e)
        {
            menuForm.detectBoardOnce = true;
        }

        public void StartBot() {
            if (Thread.CurrentThread.CurrentUICulture.Name == "pl-PL") {
                MessageBox.Show(this, "Kliknij i przytrzymaj prawy przycisk myszy aby zatrzymać", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show(this, "Press and hold right mouse button to stop", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            menuForm.runBot = true;
            startButton.Text = "Stop";
        }

        public void StopBot() {
            menuForm.runBot = false;
            startButton.Text = "Start";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (startButton.Text == "Start") {
                StartBot();
            } else {
                StopBot();
            }
        }

        private void BotModeForm_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void BotModeForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void BotModeForm_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void BotModeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopBot();
            Invoke((Action)delegate { menuForm.drawing.Clear(); });
            menuForm.Show();
        }


        private void BotModeForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
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
