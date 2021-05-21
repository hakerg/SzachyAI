using System;
using System.Drawing;
using System.Windows.Forms;

namespace SzachyAI {

    public partial class Drawing : Form {
        public int WM_NCHITTEST = 0x84;
        public int HTTRANSPARENT = -1;

        public Graphics graphics;

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
        }

        public void Draw(Screen screen, Rectangle corners) {
            Clear();
            Point screenLoc = screen.Bounds.Location;
            Location = new Point(corners.X + screenLoc.X - 2, corners.Y + screenLoc.Y - 2);
            ClientSize = new Size(corners.Width + 4, corners.Height + 4);
            graphics = CreateGraphics();
            graphics.DrawRectangle(Pens.Red, 0, 0, corners.Width + 3, corners.Height + 3);
            graphics.DrawRectangle(Pens.Red, 1, 1, corners.Width + 1, corners.Height + 1);
        }
    }
}