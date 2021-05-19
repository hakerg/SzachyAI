using System.Drawing;

namespace SzachyAI {

    public enum Color { White, Black, Length }

    public enum GameState { Playing, Win, Lose, Stalemate, Length }

    public static class Chess {
        public static string[] colorNames = new string[] { "white", "black" };

        public static bool IsValidInChess(this Point point) {
            return point.X >= 0 && point.X < Board.width && point.Y >= 0 && point.Y < Board.height;
        }
    }
}