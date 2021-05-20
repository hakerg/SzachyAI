using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Threading;
using System.Runtime.InteropServices;

namespace SzachyAI
{
    public partial class Form1 : Form
    {
        public BoardRecognizer recognizer = new BoardRecognizer();
        public List<Image<Bgr, byte>> screenImages;
        public Bitmap screenBitmap, boardBitmap, constructedBitmap;
        public Rectangle corners = Rectangle.Empty;
        public int screenIndex = 0;
        public string buttonText;
        public Drawing drawing = new Drawing();
        public Graphics graphics;

        public MenuForm menuform = new MenuForm();

        public void HideCorners()
        {
            graphics.Clear(drawing.BackColor);
        }

        public void ShowCornersOnScreen()
        {
            HideCorners();
            Point screenLoc = Screen.AllScreens[screenIndex].Bounds.Location;
            Point formLoc = new Point(corners.X + screenLoc.X - 2, corners.Y + screenLoc.Y - 2);
            Size formSize = new Size(corners.Width + 4, corners.Height + 4);
            drawing.Location = formLoc;
            drawing.ClientSize = formSize;
            float scale = 96.0F / drawing.DeviceDpi;
            drawing.Scale(new SizeF(scale, scale));
            graphics = drawing.CreateGraphics();
            graphics.DrawRectangle(Pens.Red, 0, 0, corners.Width + 3, corners.Height + 3);
            graphics.DrawRectangle(Pens.Red, 1, 1, corners.Width + 1, corners.Height + 1);
        }

        public Form1()
        {
            InitializeComponent();
            drawing.Show();
            graphics = drawing.CreateGraphics();
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "Uruchamianie wykrywania";
            backgroundWorker1.RunWorkerAsync();
        }

        public bool UpdateBoardWithCorners()
        {
            buttonText = "Przycinanie i skalowanie obrazu";
            backgroundWorker1.ReportProgress(0);
            Image<Bgr, byte> boardImage = recognizer.GetScaledBoardImage(screenImages[screenIndex], corners);
            boardBitmap = boardImage.ToBitmap();
            buttonText = "Wykrywanie pionków";
            backgroundWorker1.ReportProgress(0);
            if (recognizer.RecognizeBoard(boardImage, out Board board))
            {
                constructedBitmap = recognizer.ConstructBoardImage(board).ToBitmap();
                buttonText = "Gotowe";
                backgroundWorker1.ReportProgress(0);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            screenBitmap = null;
            boardBitmap = null;
            constructedBitmap = null;
            buttonText = "Robienie zrzutu ekranu";
            backgroundWorker1.ReportProgress(0);
            Thread.Sleep(100);
            screenImages = BoardRecognizer.CaptureScreens();
            screenBitmap = screenImages[screenIndex].ToBitmap();
            buttonText = "Zrzut ekranu gotowy";
            backgroundWorker1.ReportProgress(0);
            if (!corners.IsEmpty)
            {
                if (UpdateBoardWithCorners())
                {
                    return;
                }
            }
            buttonText = "Wyszukiwanie planszy";
            backgroundWorker1.ReportProgress(0);
            if (recognizer.DetectBoardCorners(screenImages, out corners, out screenIndex))
            {
                UpdateBoardWithCorners();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void TabPage1_Click(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            menuform.Show();
        }

        public void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            button1.Text = buttonText;
            pictureBox3.Image = screenBitmap;
            pictureBox1.Image = boardBitmap;
            pictureBox2.Image = constructedBitmap;
            if (corners.IsEmpty)
            {
                HideCorners();
            }
            else
            {
                ShowCornersOnScreen();
            }
        }

        public void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Wykryj planszę";
            button1.Enabled = true;
        }
    }
}