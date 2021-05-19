using System.Drawing;

namespace SzachyAI {

    public struct Move {
        public Piece piece;
        public Point to;
        public Type changeTo;

        public Move(Piece piece, Point to) {
            this.piece = piece;
            this.to = to;
            changeTo = piece.type;
        }

        public Move(Piece piece, Point to, Type changeTo) {
            this.piece = piece;
            this.to = to;
            this.changeTo = changeTo;
        }
    }
}