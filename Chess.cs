using System;
using System.Drawing;

namespace SzachyAI {

    public enum Color { White, Black, Length }

    public enum GameState { Playing, Win, Lose, Stalemate, Length }

    public static class Chess {
        public static string[] colorNames = new string[] { "white", "black" };

        public static bool IsValidInChess(this Point point) {
            return point.X >= 0 && point.X < Board.width && point.Y >= 0 && point.Y < Board.height;
        }

        public static string ToChessString(this Point point) {
            return Board.xNames[point.X] + Board.yNames[point.Y];
        }

        public static int Scale(this int v, double scale) {
            return (int)Math.Round(v * scale);
        }

        public static Point Scale(this Point point, double scale) {
            return new Point(point.X.Scale(scale), point.Y.Scale(scale));
        }

        public static Size Scale(this Size size, double scale) {
            return new Size(size.Width.Scale(scale), size.Height.Scale(scale));
        }

        public static Rectangle Scale(this Rectangle ret, double scale) {
            return new Rectangle(ret.Location.Scale(scale), ret.Size.Scale(scale));
        }
    }
}