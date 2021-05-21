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
        bool boardRecognised = false;

        //Make form dragable at every point
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public HelpModeForm()
        {
            InitializeComponent();
        }
        private void HelpModeForm_Load(object sender, EventArgs e)
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

        private void helpButton_Click(object sender, EventArgs e)
        {
            if (!boardRecognised)
            {
                commandLabel.Text = "First recognise the board.";
            }
            else
            {
                Random random = new Random();
                int draw = random.Next(0, 5);
                switch (draw)
                {
                    case 0:
                        commandLabel.Text = "Pawn to D4.";
                        break;
                    case 1:
                        commandLabel.Text = "Bishop takes E5!";
                        break;
                    case 2:
                        commandLabel.Text = "Knight E takes Queen."; //Jeśli dwie figury o tej samej nazwie mogą ruszyc się w to samo miejsce, należy podać położenie figury
                                                                     //(zwykle kolumnę, ale jeśli dwie są na tej samej to wtedy należy podać z którego wiersza)
                        break;
                    case 3:
                        commandLabel.Text = "Rook to D4, CHECK!"; // słowo szach raczej opcjonalne, ale IMO fajne ;)
                                                                  // można jeszcze dodać jakieś inne słowa sytuacyjne typu "CheckMate" czy "Free Queen ;)"
                        break;
                    case 4:
                        int pieceIndex = random.Next(Piece.names.Length);
                        commandLabel.Text = Piece.names[pieceIndex] + " to A1.";
                        break;
                }
            }
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
            MenuForm menuForm = new MenuForm();
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
    }
}
