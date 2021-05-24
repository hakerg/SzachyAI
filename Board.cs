using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SzachyAI {

    public class Board {

        public static int width = 8;
        public static int height = 8;
        public static int maxPiecesPerColor = 16;
        public static string[] xNames = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };
        public static string[] yNames = new string[] { "8", "7", "6", "5", "4", "3", "2", "1" };

        // top left: (x=0, y=0), white field with black rook
        // black pawn goes positive y
        // white pawn goes negative y
        public Piece[,] board = new Piece[width, height];
        public List<Piece>[] pieces = new List<Piece>[] { new List<Piece>(maxPiecesPerColor), new List<Piece>(maxPiecesPerColor) };
        public Point[] kingsPos = new Point[(int)Color.Length];
        public Piece fragilePiece = null;
        public Point fragileField = Point.Empty;
        public int[] scores = new int[(int)Color.Length];

        public ref Piece PieceAt(Point pos) {
            return ref board[pos.X, pos.Y];
        }

        public void AddPiece(Piece piece) {
            PieceAt(piece.pos) = piece;
            pieces[(int)piece.color].Add(piece);
            if (piece.type == Type.King) {
                kingsPos[(int)piece.color] = piece.pos;
            }
            scores[(int)piece.color] += piece.Score;
        }

        public void RemovePiece(Piece piece) {
            PieceAt(piece.pos) = null;
            pieces[(int)piece.color].Remove(piece);
            scores[(int)piece.color] -= piece.Score;
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
                            if (piece.pos.Y == MoveRule.pawnStartY[(int)color]) {
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
                            if (!rule.recursive || attackedPiece != null) {
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
            if (piece.type == Type.King) {
                kingsPos[(int)piece.color] = piece.pos;
            }
        }

        // TODO: castling
        public GameState MakeMove(Move move) {
            GameState state = GameState.Playing;
            Piece piece = move.piece;
            Point to = move.to;
            // capture
            Piece captured = PieceAt(to);
            if (captured != null) {
                if (captured.type == Type.King) {
                    state = GameState.Win;
                }
                RemovePiece(captured);
            }
            // capture by passing
            if (fragilePiece != null && fragileField == to) {
                RemovePiece(fragilePiece);
            }
            int color = (int)piece.color;
            if (piece.type == Type.Pawn &&
                piece.pos.Y == MoveRule.pawnStartY[color] &&
                to.Y == MoveRule.pawnTwoFieldY[color]) {
                // activate capture by passing for next move
                fragileField.X = to.X;
                fragileField.Y = MoveRule.pawnPassingY[color];
                fragilePiece = piece;
            } else {
                fragilePiece = null;
            }
            // move
            MovePiece(piece, to);
            // promotion
            if (piece.type != move.changeTo) {
                scores[color] -= piece.Score;
                piece.type = move.changeTo;
                scores[color] += piece.Score;
            }
            return state;
        }

        public float EstimateWinningProb(Color color) {
            int sum = scores.Sum();
            if (sum == 0) {
                return 0.5F;
            } else {
                return (float)scores[(int)color] / sum;
            }
        }
    }
}