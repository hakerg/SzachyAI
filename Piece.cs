using System.Drawing;

namespace SzachyAI {

    public enum Type { Pawn, Rook, Knight, Bishop, Queen, King, Length }

    public class Piece {

        public static string[] names = new string[] {
            "pawn", "rook", "knight", "bishop", "queen", "king"
        };

        public static string[] polishNames = new string[] {
            "pion", "wieża", "skoczek", "goniec", "hetman", "król"
        };

        public static string[] polishPromotionNames = new string[] {
            "piona", "wieży", "skoczka", "gońca", "hetmana", "króla"
        };

        public static string[] symbols = new string[] {
            "", "R", "N", "B", "Q", "K"
        };

        public static string[] polishSymbols = new string[] {
            "", "W", "S", "G", "H", "K"
        };

        public static string[,] fen = new string[,] {
            { "P", "R", "N", "B", "Q", "K" },
            { "p", "r", "n", "b", "q", "k" }
        };

        /*public static int[] scores = new int[] {
            100, 500, 320, 330, 900, 0
        };

        // [type, y, x]
        public static int[,,] bonuses = new int[,,] { {
                {  0,  0,  0,  0,  0,  0,  0,  0 },
                { 50, 50, 50, 50, 50, 50, 50, 50 },
                { 10, 10, 20, 30, 30, 20, 10, 10 },
                {  5,  5, 10, 25, 25, 10,  5,  5 },
                {  0,  0,  0, 20, 20,  0,  0,  0 },
                {  5, -5,-10,  0,  0,-10, -5,  5 },
                {  5, 10, 10,-20,-20, 10, 10,  5 },
                {  0,  0,  0,  0,  0,  0,  0,  0 }
            }, {
                {  0,  0,  0,  0,  0,  0,  0,  0 },
                {  5, 10, 10, 10, 10, 10, 10,  5 },
                { -5,  0,  0,  0,  0,  0,  0, -5 },
                { -5,  0,  0,  0,  0,  0,  0, -5 },
                { -5,  0,  0,  0,  0,  0,  0, -5 },
                { -5,  0,  0,  0,  0,  0,  0, -5 },
                { -5,  0,  0,  0,  0,  0,  0, -5 },
                {  0,  0,  0,  5,  5,  0,  0,  0 }
            }, {
                {-50,-40,-30,-30,-30,-30,-40,-50 },
                {-40,-20,  0,  0,  0,  0,-20,-40 },
                {-30,  0, 10, 15, 15, 10,  0,-30 },
                {-30,  5, 15, 20, 20, 15,  5,-30 },
                {-30,  0, 15, 20, 20, 15,  0,-30 },
                {-30,  5, 10, 15, 15, 10,  5,-30 },
                {-40,-20,  0,  5,  5,  0,-20,-40 },
                {-50,-40,-30,-30,-30,-30,-40,-50 }
            }, {
                {-20,-10,-10,-10,-10,-10,-10,-20 },
                {-10,  0,  0,  0,  0,  0,  0,-10 },
                {-10,  0,  5, 10, 10,  5,  0,-10 },
                {-10,  5,  5, 10, 10,  5,  5,-10 },
                {-10,  0, 10, 10, 10, 10,  0,-10 },
                {-10, 10, 10, 10, 10, 10, 10,-10 },
                {-10,  5,  0,  0,  0,  0,  5,-10 },
                {-20,-10,-10,-10,-10,-10,-10,-20 }
            }, {
                {-20,-10,-10, -5, -5,-10,-10,-20 },
                {-10,  0,  0,  0,  0,  0,  0,-10 },
                {-10,  0,  5,  5,  5,  5,  0,-10 },
                { -5,  0,  5,  5,  5,  5,  0, -5 },
                {  0,  0,  5,  5,  5,  5,  0, -5 },
                {-10,  5,  5,  5,  5,  5,  0,-10 },
                {-10,  0,  5,  0,  0,  0,  0,-10 },
                {-20,-10,-10, -5, -5,-10,-10,-20 }
            }, {
                {-30,-40,-40,-50,-50,-40,-40,-30 },
                {-30,-40,-40,-50,-50,-40,-40,-30 },
                {-30,-40,-40,-50,-50,-40,-40,-30 },
                {-30,-40,-40,-50,-50,-40,-40,-30 },
                {-20,-30,-30,-40,-40,-30,-30,-20 },
                {-10,-20,-20,-20,-20,-20,-20,-10 },
                { 20, 20,  0,  0,  0,  0, 20, 20 },
                { 20, 30, 10,  0,  0, 10, 30, 20 }
            }
        };*/

        public Point pos;
        public Type type;
        public Color color;
        public int moveCount;

        public Piece(Point pos, Type type, Color color, int moveCount = 0) {
            this.pos = pos;
            this.type = type;
            this.color = color;
            this.moveCount = moveCount;
        }

        public string Name => names[(int)type];
        public string PolishName => polishNames[(int)type];
        public string Symbol => symbols[(int)type];
        public string PolishSymbol => polishSymbols[(int)type];

        public string Fen => fen[(int)color, (int)type];

        /*public int GetScore(bool swapBonus) {
            return scores[(int)type] + bonuses[(int)type, swapBonus ? Board.height - 1 - pos.Y : pos.Y, pos.X];
        }*/

        public bool Equals(Piece other) {
            return pos == other.pos
                && type == other.type
                && color == other.color;
        }

        public Piece Clone() {
            return new Piece(pos, type, color, moveCount);
        }
    }
}