using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SzachyAI {

    public partial class Drawing : Form {
        public int WM_NCHITTEST = 0x84;
        public int HTTRANSPARENT = -1;

        public Graphics graphics;
        public bool cornersVisible = false;

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // <--- use 0x20000
                return cp;
            }
        }

        public Drawing() {
            InitializeComponent();
            graphics = CreateGraphics();
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)HTTRANSPARENT;
            else
                base.WndProc(ref m);
        }

        public void Clear() {
            graphics.Clear(BackColor);
            cornersVisible = false;
        }

        public static Brush GetBrush(float winningProb) {
            if (winningProb <= 0.5F) {
                int intensity = (int)(winningProb * 510);
                return new SolidBrush(System.Drawing.Color.FromArgb(255, intensity, 0));
            }
            else {
                int intensity = (int)((winningProb - 0.5F) * 510);
                return new SolidBrush(System.Drawing.Color.FromArgb(255 - intensity, 255, 0));
            }
        }

        public void Draw(Screen screen, Rectangle corners, List<Move> moves, bool rotated) {
            Clear();
            Point screenLoc = screen.Bounds.Location;
            Location = new Point(corners.X + screenLoc.X - 2, corners.Y + screenLoc.Y - 2);
            ClientSize = new Size(corners.Width + 4, corners.Height + 4);
            graphics = CreateGraphics();
            if (Settings.showBorder) {
                graphics.DrawRectangle(Pens.Red, 0, 0, corners.Width + 3, corners.Height + 3);
                graphics.DrawRectangle(Pens.Red, 1, 1, corners.Width + 1, corners.Height + 1);
            }
            if (moves != null) {
                foreach (Move move in moves) {
                    Point from = move.from;
                    Point to = move.to;
                    if (rotated) {
                        from = from.ChessRotate180();
                        to = to.ChessRotate180();
                    }
                    float x1 = 2 + (from.X + 0.5F) * corners.Width / Board.width;
                    float y1 = 2 + (from.Y + 0.5F) * corners.Height / Board.height;
                    float x2 = 2 + (to.X + 0.5F) * corners.Width / Board.width;
                    float y2 = 2 + (to.Y + 0.5F) * corners.Height / Board.height;
                    Brush brush = GetBrush(1.0F / (1.0F + (float)Math.Exp(move.score * -0.005)));
                    graphics.DrawLine(new Pen(brush, 3), x1, y1, x2, y2);
                    graphics.FillEllipse(brush, x2 - 5, y2 - 5, 10, 10);
                }
            }
            cornersVisible = true;
        }

        public void Draw(Screen screen, Rectangle corners) {
            Draw(screen, corners, null, false);
        }
    }
}