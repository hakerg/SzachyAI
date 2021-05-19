using System.Collections.Generic;
using System.Drawing;

namespace SzachyAI {

    public class Board {

        public static int width = 8;
        public static int height = 8;
        public static int maxPiecesPerColor = 16;

        // top left: (x=0, y=0), white field with black rook
        // black pawn goes positive y
        // white pawn goes negative y
        public Piece[,] board = new Piece[width, height];
        public List<Piece>[] pieces = new List<Piece>[] { new List<Piece>(maxPiecesPerColor), new List<Piece>(maxPiecesPerColor) };
        public Point[] kingsPos = new Point[(int)Color.Length];
        public Piece fragilePiece;
        public Point fragileField;

        public ref Piece PieceAt(Point pos) {
            return ref board[pos.X, pos.Y];
        }

        public void AddPiece(Piece piece) {
            PieceAt(piece.pos) = piece;
            pieces[(int)piece.color].Add(piece);
            if (piece.type == Type.King) {
                kingsPos[(int)piece.color] = piece.pos;
            }
        }

        public void RemovePiece(Piece piece) {
            PieceAt(piece.pos) = null;
            pieces[(int)piece.color].Remove(piece);
        }

        public bool IsNearKing(Point kingPos, Point nearPos) {
            return nearPos.X >= kingPos.X - 1 && nearPos.X <= kingPos.X + 1
                && nearPos.Y >= kingPos.Y - 1 && nearPos.Y <= kingPos.Y + 1;
        }

        public void GetMoves(Color color, List<Move> output) {
            output.Clear();
            Point opponentKingPos = kingsPos[color == Color.White ? 1 : 0];
            foreach (Piece piece in pieces[(int)color]) {
                if (piece.type == Type.Pawn) {
                    Point dir = MoveRule.pawnMoveDir[(int)color];
                    Point newPos = piece.pos;
                    newPos.Offset(dir);
                    if (PieceAt(newPos) == null) {
                        // promotion
                        if (newPos.Y == MoveRule.pawnFinalY[(int)color]) {
                            foreach (Type changeTo in MoveRule.pawnChangeTypes) {
                                Move move = new Move(piece, newPos, changeTo);
                                output.Add(move);
                            }
                        } else {
                            // one field move
                            Move move = new Move(piece, newPos);
                            output.Add(move);
                            // two field move
                            if (!piece.moved) {
                                Point newPos2 = newPos;
                                newPos2.Offset(dir);
                                if (PieceAt(newPos2) == null) {
                                    Move move2 = new Move(piece, newPos2);
                                    output.Add(move2);
                                }
                            }
                        }
                    }
                    // pawn capturing
                    foreach (Point captureDir in MoveRule.pawnCaptureDirs[(int)color]) {
                        Point capturePos = piece.pos;
                        capturePos.Offset(captureDir);
                        if (capturePos.IsValidInChess()) {
                            Piece attackedPiece = PieceAt(capturePos);
                            if (attackedPiece != null && attackedPiece.color != color) {
                                Move move = new Move(piece, capturePos);
                                output.Add(move);
                            }
                        }
                    }
                } else {
                    MoveRule rule = MoveRule.rules[(int)piece.type];
                    foreach (Point dir in rule.dirs) {
                        Point newPos = piece.pos;
                        newPos.Offset(dir);
                        while (newPos.IsValidInChess()) {
                            Piece attackedPiece = PieceAt(newPos);
                            if (attackedPiece == null || attackedPiece.color != color) {
                                if (piece.type != Type.King || !IsNearKing(opponentKingPos, newPos)) {
                                    Move move = new Move(piece, newPos);
                                    output.Add(move);
                                }
                            }
                            if (!rule.recursive) {
                                break;
                            }
                            newPos.Offset(dir);
                        }
                    }
                    // TODO: check if captured
                    if (piece.type == Type.King && !piece.moved) {
                        // TODO: castling
                    }
                }
            }
        }

        public void MovePiece(Piece piece, Point to) {
            PieceAt(piece.pos) = null;
            piece.pos = to;
            piece.moved = true;
            PieceAt(piece.pos) = piece;
        }

        // TODO: castling
        public GameState MakeMove(Move move, out int score) {
            score = 0;
            GameState state = GameState.Playing;
            // capture
            Piece captured = PieceAt(move.to);
            if (captured != null) {
                if (captured.type == Type.King) {
                    state = GameState.Win;
                }
                RemovePiece(captured);
                score += Piece.scores[(int)captured.type];
            }
            // capture by passing
            if (fragilePiece != null && fragileField == move.to) {
                RemovePiece(fragilePiece);
                score += Piece.scores[(int)fragilePiece.type];
            }
            int color = (int)move.piece.color;
            if (move.piece.type == Type.Pawn && !move.piece.moved && move.to.Y == MoveRule.pawnTwoFieldY[color]) {
                // activate capture by passing for next move
                fragileField.X = move.to.X;
                fragileField.Y = MoveRule.pawnPassingY[color];
                fragilePiece = move.piece;
            } else {
                fragilePiece = null;
            }
            // move
            MovePiece(move.piece, move.to);
            // promotion
            move.piece.type = move.changeTo;
            return state;
        }
    }
}