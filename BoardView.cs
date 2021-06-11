using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzachyAI {
    public class BoardView {

        public const int width = 8;
        public const int height = 8;
        public static string[] xNames = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
        public static string[] yNames = new string[] { "8", "7", "6", "5", "4", "3", "2", "1" };

        // top left: (x=0, y=0), white field with black rook
        // black pawn goes positive y
        // white pawn goes negative y
        public Piece[,] board = new Piece[width, height];
        public int[] score = new int[(int)Color.Length];

        public void AddPiece(Piece piece) {
            board.At(piece.pos) = piece;
            score[(int)piece.color] += piece.Score;
        }

        public void RemovePiece(Point where) {
            ref Piece piece = ref board.At(where);
            score[(int)piece.color] -= piece.Score;
            piece = null;
        }

        // make sure target is empty
        public void MovePiece(Piece piece, Point to) {
            score[(int)piece.color] -= piece.Score;
            board.At(piece.pos) = null;
            piece.pos = to;
            board.At(to) = piece;
            score[(int)piece.color] += piece.Score;
        }

        public bool Equals(BoardView other) {
            for (int y = 0; y < Board.height; y++) {
                for (int x = 0; x < Board.width; x++) {
                    Point p = new Point(x, y);
                    Piece p1 = board.At(p);
                    Piece p2 = other.board.At(p);
                    if ((p1 == null) != (p2 == null) || (p1 != null && !p1.Equals(p2))) {
                        return false;
                    }
                }
            }
            return true;
        }

        public BoardView Rotate180() {
            BoardView ret = new BoardView();
            foreach (Piece piece in board) {
                if (piece != null) {
                    ret.AddPiece(new Piece(piece.pos.ChessRotate180(), piece.type, piece.color, piece.moveCount));
                }
            }
            return ret;
        }
    }
}
