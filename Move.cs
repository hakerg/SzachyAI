using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace SzachyAI {

    public class Move : IEquatable<Move> {
        public Piece piece;
        public Point from;
        public Point to;
        public Type changeFrom;
        public Type changeTo;
        public Piece capture;
        public Piece prevFragilePiece;
        public Point prevFragileField;
        public int prevHalfMoveClock;

        public Move(Piece piece, Point to, Piece capture, Piece prevFragilePiece, Point prevFragileField, int prevHalfMoveClock) {
            this.piece = piece;
            from = piece.pos;
            this.to = to;
            this.capture = capture;
            changeFrom = piece.type;
            changeTo = piece.type;
            this.prevFragilePiece = prevFragilePiece;
            this.prevFragileField = prevFragileField;
            this.prevHalfMoveClock = prevHalfMoveClock;
        }

        public Move(Piece piece, Point to, Piece capture, Type changeTo, Piece prevFragilePiece, Point prevFragileField, int prevHalfMoveClock) {
            this.piece = piece;
            from = piece.pos;
            this.to = to;
            this.capture = capture;
            changeFrom = piece.type;
            this.changeTo = changeTo;
            this.prevFragilePiece = prevFragilePiece;
            this.prevFragileField = prevFragileField;
            this.prevHalfMoveClock = prevHalfMoveClock;
        }

        public bool IsShortCastle => piece.type == Type.King && ((from.X == 4 && to.X == 6) || (from.X == 3 && to.X == 1));

        public bool IsLongCastle => piece.type == Type.King && ((from.X == 4 && to.X == 2) || (from.X == 3 && to.X == 5));

        public string StockfishString {
            get {
                string ret = from.ToChessString() + to.ToChessString();
                if (changeFrom != changeTo) {
                    ret += Piece.fen[(int)piece.color, (int)changeTo];
                }
                return ret;
            }
        }

        public override bool Equals(object obj) {
            return Equals(obj as Move);
        }

        public bool Equals(Move other) {
            return other != null &&
                   StockfishString.Equals(other.StockfishString);
        }

        public override int GetHashCode() {
            return -601501082 + EqualityComparer<string>.Default.GetHashCode(StockfishString);
        }

        public static bool operator ==(Move left, Move right) {
            return EqualityComparer<Move>.Default.Equals(left, right);
        }

        public static bool operator !=(Move left, Move right) {
            return !(left == right);
        }
    }
}