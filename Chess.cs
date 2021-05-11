using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI
{
    public enum Color { White, Black }
    public enum GameState { Playing, Win, Lose, Stalemate }

    public static class Chess
    {
        public static string[] colorNames = new string[] { "white", "black" };

        public static bool IsValidInChess(this Point point)
        {
            return point.X >= 0 && point.X < 8 && point.Y >= 0 && point.Y < 8;
        }
    }
}
