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
    public partial class BotModeForm : Form
    {
        bool boardRecognised = false;
        bool botPlaying = false;

        //Make form dragable at every point
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public BotModeForm()
        {
            InitializeComponent();
        }
        private void BotModeForm_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            recognisedBoardPcBox.Visible = Settings.EnableDebugMode;
            boardLabel.Visible = Settings.EnableDebugMode;
            if (Settings.EnableDebugMode)
            {
                this.Size = new Size(500, 500);
                commandLabel.Location = new Point(32, 407);
            }
            else 
            {
                commandLabel.Location = new Point(32, 207);
                this.Size = new Size(500, 300); 
            }
        }
        private void scanButton_Click(object sender, EventArgs e)
        {
            commandLabel.Text = "Board recognised.";
            boardRecognised = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!boardRecognised)
            {
                commandLabel.Text = "First recognise the board.";
            }
            else
            {
                botPlaying = !botPlaying;
                if (botPlaying)
                {
                    startButton.Text = "Stop the bot";
                    commandLabel.Text = "Playing.";
                }
                else
                {
                    startButton.Text = "Start";
                    commandLabel.Text = "Waiting.";
                    // Tutaj też można dawać teksty sytuacyjne podczas trwania gry bota, np zeby 'komentował' własną grę, jak coś zabiera to mówi itd.
                    // ale w taki bardziej komentatorski sposób niż "Bishop takes D4", tylko np "Free piece, I like that :)",
                    // tylko to niestety tez wymaga nieco więcej kodowania bo musi wiedzieć czy faktycznie pion był darmowy czy to wymiana,
                    // więc to raczej opcjonalne jak starczy czasu
                }
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
            MenuForm menuForm = new MenuForm();
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
    }
}
