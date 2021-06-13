using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
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

        public enum Status { Ready, DetectingCorners, FindingMove, BoardNotFound, MoveNotFound, BoardFound };

        public List<string> englishStatus = new List<string> {
            "Ready",
            "Detecting chessboard corners",
            "Finding move",
            "Board not found",
            "No valid move",
            "Board found"
        };

        public List<string> polishStatus = new List<string> {
            "Gotowy",
            "Wykrywanie krawędzi planszy",
            "Wyszukiwanie ruchu",
            "Nie znaleziono planszy",
            "Brak ruchu",
            "Plansza wykryta"
        };

        //Make "FormBorderStyle = None" form dragable
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private HelpModeForm helpModeForm;
        private BotModeForm botModeForm;
        public Drawing drawing;
        public Screen boardScreen;
        public Rectangle corners;
        public Board lastBoard;
        public bool runBot = false;
        public bool detectBoardOnce = false;
        public bool giveHintOnce = false;
        public BoardRecognizer recognizer = new BoardRecognizer();
        public Random random = new Random();
        public Thread creatorThread;
        public SpeechSynthesizer synth;
        public bool cornersValid = false;

        public MenuForm()
        {
            creatorThread = Thread.CurrentThread;
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

        private void UpdateStatus(string status) {
            Invoke((Action)delegate {
                if (helpModeForm != null && !helpModeForm.IsDisposed) {
                    helpModeForm.UpdateStatus(status);
                }
                if (botModeForm != null && !botModeForm.IsDisposed) {
                    botModeForm.UpdateStatus(status);
                }
            });
        }

        private void UpdateStatus(Status statusEnum) {
            string status = "";
            Invoke((Action)delegate {
                if (Thread.CurrentThread.CurrentUICulture.Name == "pl-PL") {
                    status = polishStatus[(int)statusEnum];
                } else {
                    status = englishStatus[(int)statusEnum];
                }
            });
            UpdateStatus(status);
        }

        private void UpdateImage(BoardView boardView) {
            Bitmap constructedImage = recognizer.ConstructBoardImage(boardView).ToBitmap();
            Invoke((Action)delegate {
                if (helpModeForm != null && !helpModeForm.IsDisposed) {
                    helpModeForm.UpdateImage(constructedImage);
                }
                if (botModeForm != null && !botModeForm.IsDisposed) {
                    botModeForm.UpdateImage(constructedImage);
                }
            });
        }

        public void SimulateMove(Move move) {
            Point from = move.from;
            Point to = move.to;
            if (lastBoard.rotated) {
                from = from.ChessRotate180();
                to = to.ChessRotate180();
            }
            int x1 = (int)(boardScreen.Bounds.Location.X + corners.X + (from.X + 0.5F) * corners.Width / Board.width);
            int y1 = (int)(boardScreen.Bounds.Location.Y + corners.Y + (from.Y + 0.5F) * corners.Height / Board.height);
            int x2 = (int)(boardScreen.Bounds.Location.X + corners.X + (to.X + 0.5F) * corners.Width / Board.width);
            int y2 = (int)(boardScreen.Bounds.Location.Y + corners.Y + (to.Y + 0.5F) * corners.Height / Board.height);
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

        public void StopBot() {
            runBot = false;
            Invoke((Action)delegate {
                if (botModeForm != null && !botModeForm.IsDisposed) {
                    botModeForm.StopBot();
                }
            });
        }

        private void FindCorners() {
            Invoke((Action)delegate { drawing.Clear(); });
            UpdateStatus(Status.DetectingCorners);
            Thread.Sleep(100);
            List<Image<Bgr, byte>> screens = BoardRecognizer.CaptureScreens();
            if (recognizer.DetectBoardCorners(screens, out Rectangle newCorners, out int imageIndex)) {
                corners = newCorners;
                boardScreen = Screen.AllScreens[imageIndex];
                Invoke((Action)delegate { drawing.Draw(boardScreen, corners); });
                UpdateStatus(Status.BoardFound);
                cornersValid = true;
            } else {
                UpdateStatus(Status.BoardNotFound);
                cornersValid = false;
                giveHintOnce = false;
                StopBot();
            }
            detectBoardOnce = false;
        }

        private void UpdateBoard() {
            // make screenshot
            //if (drawing.Bounds.Contains(Cursor.Position)) {
            //    Cursor.Position = new Point(drawing.Bounds.Right, drawing.Bounds.Bottom);
            //}
            Image<Bgr, byte> boardScreenImage = BoardRecognizer.CaptureScreen(boardScreen);
            Image<Bgr, byte> boardImage = recognizer.GetScaledBoardImage(boardScreenImage, corners);

            // detect pieces
            if (recognizer.RecognizeBoard(boardImage, out BoardView boardView)) {
                UpdateImage(boardView);
                if (!drawing.cornersVisible) {
                    Invoke((Action)delegate { drawing.Draw(boardScreen, corners); });
                }

                if (lastBoard == null || !lastBoard.Update(boardView, 2)) {
                    lastBoard = new Board(boardView);
                }
                cornersValid = true;
            } else {
                Invoke((Action)delegate { drawing.Clear(); });
                cornersValid = false;
                UpdateStatus(Status.BoardNotFound);
                StopBot();
            }
        }

        private void FindAndProcessMove() {
            UpdateStatus(Status.FindingMove);
            Move move;
            if (Settings.useStockfish) {
                move = lastBoard.GetStockfishMove(Settings.findingTime * 1000);
                if (move != null) {
                    move.score = 100000;
                }
            } else {
                move = lastBoard.GetBestMove(DateTime.Now + TimeSpan.FromSeconds(Settings.findingTime));
            }
            if (move != null) {
                UpdateStatus(lastBoard.MoveShortString(move));

                // stop bot
                if (GetAsyncKeyState(VK_RBUTTON) > 0) {
                    StopBot();
                }

                // give hint
                if (giveHintOnce) {
                    switch (Settings.hintMode) {
                        case HintMode.DrawOnScreen:
                            Invoke((Action)delegate { drawing.Draw(boardScreen, corners, new List<Move> { move }, lastBoard.rotated); });
                            break;
                        case HintMode.TextToSpeech:
                            synth.Speak(lastBoard.MoveLongString(move));
                            break;
                    }
                // make move
                } else if (runBot) {
                    SimulateMove(move);
                    if (move.changeFrom != move.changeTo) {
                        Invoke((Action)delegate {
                            if (botModeForm != null && !botModeForm.IsDisposed) {
                                botModeForm.StopBot();
                            }
                        });
                    }
                }
            } else {
                UpdateStatus(Status.MoveNotFound);
                StopBot();
            }
            giveHintOnce = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            while (!backgroundWorker1.CancellationPending) {
                if (Thread.CurrentThread.CurrentUICulture.Name != creatorThread.CurrentUICulture.Name) {
                    Thread.CurrentThread.CurrentUICulture = creatorThread.CurrentUICulture;
                    synth = new SpeechSynthesizer();
                    synth.SetOutputToDefaultAudioDevice();
                }

                if (!corners.IsEmpty && (runBot || giveHintOnce)) {
                    UpdateBoard();
                    if (cornersValid && (giveHintOnce || (runBot && lastBoard.nextPlayer == (lastBoard.rotated ? Color.Black : Color.White)))) {
                        if (lastBoard.IsLastMoveMate()) {
                            StopBot();
                            giveHintOnce = false;
                        }
                        else {
                            FindAndProcessMove();
                        }
                    }
                }

                if (detectBoardOnce || (!cornersValid && (runBot || giveHintOnce))) {
                    FindCorners();
                }

                Thread.Sleep(100);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Close();
            Application.Exit();
        }
    }
}
