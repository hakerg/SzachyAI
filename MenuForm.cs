using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SzachyAI
{
    public partial class MenuForm : Form
    {

        [DllImport("user32.dll")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        const int VK_LBUTTON = 0x01;
        const int VK_RBUTTON = 0x02;

        public enum Status { Ready, DetectingCorners, DetectingPieces, FindingMove, SimulatingMove };

        public List<string> englishStatus = new List<string> {
            "Ready",
            "Detecting chessboard corners",
            "Detecting pieces",
            "Finding move",
            "Simulating mouse clicks"
        };

        public List<string> polishStatus = new List<string> {
            "Gotowy",
            "Wykrywanie krawędzi planszy",
            "Wykrywanie figur",
            "Wyszukiwanie ruchu",
            "Wykonywanie ruchu"
        };

        //Make "FormBorderStyle = None" form dragable
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private HelpModeForm helpModeForm;
        private BotModeForm botModeForm;
        public Drawing drawing;

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

        public void ValidateBorder(List<Move> moves = null) {
            cornersValid = true;
            Invoke((Action)delegate { drawing.Draw(boardScreen, corners, moves); });
        }

        public void SimulateMove(Move move) {
            UpdateStatus(Status.SimulatingMove);
            int x1 = (int)(boardScreen.Bounds.Location.X + corners.X + (move.piece.pos.X + 0.5F) * corners.Width / Board.width);
            int y1 = (int)(boardScreen.Bounds.Location.Y + corners.Y + (move.piece.pos.Y + 0.5F) * corners.Height / Board.height);
            int x2 = (int)(boardScreen.Bounds.Location.X + corners.X + (move.to.X + 0.5F) * corners.Width / Board.width);
            int y2 = (int)(boardScreen.Bounds.Location.Y + corners.Y + (move.to.Y + 0.5F) * corners.Height / Board.height);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(Settings.eventTime);
            Cursor.Position = new Point(x1, y1);
            Thread.Sleep(Settings.eventTime);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(Settings.eventTime);
            if (Settings.mouseMode == MouseMode.Clicking) {
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                Thread.Sleep(Settings.eventTime);
            }
            Cursor.Position = new Point(x2, y2);
            Thread.Sleep(Settings.eventTime);
            if (Settings.mouseMode == MouseMode.Clicking) {
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                Thread.Sleep(Settings.eventTime);
            }
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(Settings.eventTime);
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

                if (GetAsyncKeyState(VK_RBUTTON) > 0) {
                    Invoke((Action)delegate {
                        if (botModeForm != null && !botModeForm.IsDisposed) {
                            botModeForm.StopBot();
                        }
                    });
                }

                if (cornersValid && (giveHintOnce || runBot)) {
                    UpdateStatus(Status.DetectingPieces);
                    if (boardScreenImage == null) {
                        ValidateBorder();
                        Cursor.Position = new Point(drawing.Bounds.Right, drawing.Bounds.Bottom);
                        Thread.Sleep(100);
                        boardScreenImage = BoardRecognizer.CaptureScreen(boardScreen);
                    }
                    Image<Bgr, byte> boardImage = recognizer.GetScaledBoardImage(boardScreenImage, corners);
                    if (recognizer.RecognizeBoard(boardImage, out Board board)) {
                        constructedImage = recognizer.ConstructBoardImage(board).ToBitmap();
                        List<Move> moves = new List<Move>();
                        board.GetMoves(Color.White, moves);
                        Move move = moves[0];
                        if (giveHintOnce) {
                            switch (Settings.hintMode) {
                                case HintMode.DrawOnScreen:
                                    ValidateBorder(new List<Move> { move });
                                    break;
                                case HintMode.WindowLabel:
                                    // TODO
                                    break;
                                case HintMode.TextToSpeech:
                                    // TODO
                                    break;
                            }
                            giveHintOnce = false;
                        } else if (runBot) {
                            SimulateMove(move);
                        }
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
