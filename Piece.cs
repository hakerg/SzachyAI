using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI
{
    public class Piece
    {
        public enum Type { Pawn, Rook, Knight, Bishop, Queen, King }
        public static string[] names = new string[]
        {
            "pawn", "rook", "knight", "bishop", "queen", "king"
        };

        public Point pos;
        public Type type;
        public Color color;
        public bool moved;

        public Piece(Point pos, Type type, Color color, bool moved)
        {
            this.pos = pos;
            this.type = type;
            this.color = color;
            this.moved = moved;
        }
    }
}
