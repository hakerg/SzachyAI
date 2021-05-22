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

        public static string[] symbols = new string[] {
            "", "R", "N", "B", "Q", "K"
        };

        public static string[] polishSymbols = new string[] {
            "", "W", "S", "G", "H", "K"
        };

        public static int[] scores = new int[] {
            1, 5, 3, 3, 9, 0
        };

        public Point pos;
        public Type type;
        public Color color;
        public bool moved;

        public Piece(Point pos, Type type, Color color, bool moved) {
            this.pos = pos;
            this.type = type;
            this.color = color;
            this.moved = moved;
        }
    }
}