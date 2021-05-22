using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class MenuForm : Form
    {
        public enum Status { Ready, DetectingCorners, DetectingPieces, FindingMove };

        public List<string> englishStatus = new List<string> {
            "Ready",
            "Detecting chessboard corners",
            "Detecting pieces",
            "Finding move"
        };

        public List<string> polishStatus = new List<string> {
            "Gotowy",
            "Wykrywanie krawędzi planszy",
            "Wykrywanie figur",
            "Wyszukiwanie ruchu"
        };

        //Make "FormBorderStyle = None" form dragable
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private HelpModeForm helpModeForm;
        private BotModeForm botModeForm;
        private Drawing drawing;

        public bool cornersValid = false;
        public Screen boardScreen;
        public Rectangle corners;
        public Bitmap constructedImage;
        public string status;

        public bool runBot = false;
        public bool detectBoardOnce = false;
        public bool giveHintOnce = false;

        public BoardRecognizer recognizer = new BoardRecognizer();
        public CultureInfo language = Thread.CurrentThread.CurrentUICulture;

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

        public void ChangeLanguage(CultureInfo language) {
            this.language = language;
            Controls.Clear();
            InitializeComponent();
        }

        private void aboutButton_Click(object sender, EventArgs e) {
            Hide();
            AboutForm aboutForm = new AboutForm(this);
            aboutForm.Show();
        }

        private void UpdateStatus(Status statusEnum) {
            Invoke((Action)delegate {
                if (language.Name == "pl-PL") {
                    status = polishStatus[(int)statusEnum];
                } else {
                    status = englishStatus[(int)statusEnum];
                }
                if (helpModeForm != null && !helpModeForm.IsDisposed) {
                    helpModeForm.UpdateStatus();
                }
                if (botModeForm != null && !botModeForm.IsDisposed) {
                    botModeForm.UpdateStatus();
                }
            });
        }

        public void InvalidateBorder() {
            cornersValid = false;
            Invoke((Action)delegate { drawing.Clear(); });
        }

        public void ValidateBorder() {
            cornersValid = true;
            Invoke((Action)delegate { drawing.Draw(boardScreen, corners); });
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            while (!backgroundWorker1.CancellationPending) {
                Image<Bgr, byte> boardScreenImage = null;
                if (detectBoardOnce || (!cornersValid && (runBot || giveHintOnce))) {
                    InvalidateBorder();
                    UpdateStatus(Status.DetectingCorners);

                    Thread.Sleep(100);
                    List<Image<Bgr, byte>> screens = BoardRecognizer.CaptureScreens();
                    if (recognizer.DetectBoardCorners(screens, out corners, out int imageIndex)) {
                        boardScreenImage = screens[imageIndex];
                        boardScreen = Screen.AllScreens[imageIndex];
                        ValidateBorder();
                        detectBoardOnce = false;
                    }
                    UpdateStatus(Status.Ready);
                }

                if (cornersValid) {
                    UpdateStatus(Status.DetectingPieces);
                    if (boardScreenImage == null) {
                        boardScreenImage = BoardRecognizer.CaptureScreen(boardScreen);
                    }
                    Image<Bgr, byte> boardImage = recognizer.GetScaledBoardImage(boardScreenImage, corners);
                    if (recognizer.RecognizeBoard(boardImage, out Board board)) {
                        constructedImage = recognizer.ConstructBoardImage(board).ToBitmap();
                        // TODO: hint / bot
                        giveHintOnce = false;
                    }
                    else {
                        InvalidateBorder();
                    }
                }

                UpdateStatus(Status.Ready);
                Thread.Sleep(100);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Close();
            Application.Exit();
        }
    }
}
