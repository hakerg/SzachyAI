using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI
{
    // TODO: pawn piece change
    public struct Move
    {
        public Piece piece;
        public Point to;
        //public PieceType changeTo;

        public Move(Piece piece, Point to)
        {
            this.piece = piece;
            this.to = to;
        }
    }
}
