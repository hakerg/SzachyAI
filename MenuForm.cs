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
    public partial class MenuForm : Form
    {
        //Make "FormBorderStyle = None" form dragable
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private HelpModeForm helpModeForm;
        private BotModeForm botModeForm;
        private Drawing drawing;

        public Screen boardScreen;
        public Rectangle corners;
        public Bitmap constructedImage;
        public string status;

        public MenuForm()
        {
            InitializeComponent();
            drawing = new Drawing();
            drawing.Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void helpModeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            helpModeForm = new HelpModeForm(this);
            helpModeForm.Show();
        }

        private void botModeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            botModeForm = new BotModeForm(this);
            botModeForm.Show();
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

        private void settingsButton_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this);
            settingsForm.ShowDialog();
        }

        public void ChangeLanguage() {
            Controls.Clear();
            InitializeComponent();
        }

        private void aboutButton_Click(object sender, EventArgs e) {
            Hide();
            AboutForm aboutForm = new AboutForm(this);
            aboutForm.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            while (!backgroundWorker1.CancellationPending) {
                Thread.Sleep(100);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (helpModeForm != null && !helpModeForm.IsDisposed) {
                helpModeForm.UpdateStatus();
            }
            if (botModeForm != null && !botModeForm.IsDisposed) {
                botModeForm.UpdateStatus();
            }
            if (Settings.showBorder && boardScreen != null && corners != Rectangle.Empty) {
                drawing.Draw(boardScreen, corners);
            } else {
                drawing.Clear();
            }
        }

        public void DetectBoard() {

        }

        public void GiveHint() {

        }

        public void StartBotMode() {

        }

        public void StopBotMode() {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Close();
            Application.Exit();
        }
    }
}
