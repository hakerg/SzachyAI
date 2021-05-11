using System.Collections.Generic;
using System.Drawing;

namespace SzachyAI
{
    public class Board
    {
        // top left: (x=0, y=0), white field with black rook
        // black pawn goes positive y
        // white pawn goes negative y
        public Piece[,] board = new Piece[8, 8];
        public List<Piece>[] pieces = new List<Piece>[] { new List<Piece>(16), new List<Piece>(16) };
        public Point[] kingsPos = new Point[2];
        public Move lastMove;

        public ref Piece PieceAt(Point pos)
        {
            return ref board[pos.X, pos.Y];
        }

        public void AddPiece(Piece piece)
        {
            PieceAt(piece.pos) = piece;
            pieces[(int)piece.color].Add(piece);
            if (piece.type == Piece.Type.King)
            {
                kingsPos[(int)piece.color] = piece.pos;
            }
        }

        public bool IsAttacked(Point pos, Color attackedColor)
        {
            // TODO
            return false;
        }

        public bool IsMoveSafe(Move move)
        {
            // TODO
            return true;
        }

        public bool IsNearKing(Point kingPos, Point nearPos)
        {
            return nearPos.X >= kingPos.X - 1 && nearPos.X <= kingPos.X + 1
                && nearPos.Y >= kingPos.Y - 1 && nearPos.Y <= kingPos.Y + 1;
        }

        // TODO: castling
        public GameState GetMoves(Color color, List<Move> output)
        {
            output.Clear();
            Point kingPos = kingsPos[(int)color];
            Point opponentKingPos = kingsPos[color == Color.White ? 1 : 0];
            foreach (Piece piece in pieces[(int)color])
            {
                if (piece.type == Piece.Type.Pawn)
                {
                    Point stepPos = piece.pos;
                    stepPos.Offset(MoveRule.pawnMoveDir[(int)color]);
                    if (PieceAt(stepPos) == null)
                    {
                        Move move = new Move(piece, stepPos);
                        if (IsMoveSafe(move))
                        {
                            output.Add(move);
                        }
                        
                    }
                    // TODO: capturing, 2-field move
                }
                else
                {
                    MoveRule rule = MoveRule.rules[(int)piece.type];
                    foreach (Point dir in rule.dirs)
                    {
                        Point newPos = piece.pos;
                        newPos.Offset(dir);
                        while (newPos.IsValidInChess())
                        {
                            Piece attackedPiece = PieceAt(newPos);
                            if (attackedPiece == null || attackedPiece.color != color)
                            {
                                if (attackedPiece != null && attackedPiece.type == Piece.Type.King)
                                {
                                    return GameState.Win;
                                }
                                if (piece.type != Piece.Type.King || !IsNearKing(opponentKingPos, newPos))
                                {
                                    Move move = new Move(piece, newPos);
                                    if (IsMoveSafe(move))
                                    {
                                        output.Add(move);
                                    }
                                }
                            }
                            if (!rule.recursive)
                            {
                                break;
                            }
                            newPos.Offset(dir);
                        }
                    }
                }
            }
            if (output.Count == 0)
            {
                if (IsAttacked(kingPos, color))
                {
                    return GameState.Lose;
                }
                else
                {
                    return GameState.Stalemate;
                }
            }
            else
            {
                return GameState.Playing;
            }
        }

        // TODO: capture in passing
        // TODO: change pawn
        // TODO: castling
        public void MakeMove(Move move)
        {
            // TODO
            move.piece.moved = true;
            lastMove = move;
        }
    }
}