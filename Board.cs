using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SzachyAI {

    public class Board : BoardView {

        public Piece fragilePiece = null;
        public Point fragileField = Point.Empty;
        public Color nextPlayer;
        public Stockfish.NET.Core.Stockfish stockfish = new Stockfish.NET.Core.Stockfish("stockfish_20090216_x64.exe");
        public bool rotated;
        public int halfMoveClock = 0;
        public int fullMoveNumber = 1;

        public Board(BoardView boardView) {
            int[] piecesTop = new int[(int)Color.Length];
            int[] piecesBottom = new int[(int)Color.Length];
            foreach (Piece piece in boardView.board) {
                if (piece != null) {
                    if (piece.pos.Y < height / 2) {
                        piecesTop[(int)piece.color]++;
                    } else {
                        piecesBottom[(int)piece.color]++;
                    }
                }
            }
            int whiteAtBottomVotes = piecesTop[(int)Color.Black] + piecesBottom[(int)Color.White];
            int blackAtBottomVotes = piecesTop[(int)Color.White] + piecesBottom[(int)Color.Black];
            if (whiteAtBottomVotes >= blackAtBottomVotes) {
                board = boardView.board;
                nextPlayer = Color.White;
                rotated = false;
            } else {
                board = boardView.Rotate180().board;
                nextPlayer = Color.Black;
                rotated = true;
            }
        }

        public void AddPawnMoveWithChangeTo(List<Move> nextMoves, Piece piece, Point to, Piece capture) {
            // promotion
            if (to.Y == MoveRule.pawnFinalY[(int)nextPlayer]) {
                foreach (Type changeTo in MoveRule.pawnChangeTypes) {
                    nextMoves.Add(new Move(piece, to, capture, changeTo, fragilePiece, fragileField, halfMoveClock));
                }
            } else {
                nextMoves.Add(new Move(piece, to, capture, fragilePiece, fragileField, halfMoveClock));
            }
        }

        public bool IsCastlingPossible(Color player, int side) {
            Piece atKingPos = board.At(MoveRule.initKingPos[(int)player]);
            if (atKingPos != null && atKingPos.type == Type.King && atKingPos.color == player && atKingPos.moveCount == 0) {
                Piece atRookPos = board.At(MoveRule.initRookPos[(int)player, side]);
                if (atRookPos != null && atRookPos.type == Type.Rook && atRookPos.color == player && atRookPos.moveCount == 0) {
                    return true;
                }
            }
            return false;
        }

        public Piece GetAttackedPiece(Point moveTo) {
            Piece attackedPiece = board.At(moveTo);
            if (fragilePiece != null && moveTo == fragileField) {
                attackedPiece = fragilePiece;
            }
            return attackedPiece;
        }

        public List<Move> GetMovesInaccurate() {
            return GetMovesInaccurate(nextPlayer);
        }

        public List<Move> GetMovesInaccurate(Color nextPlayer) {
            List<Move> nextMoves = new List<Move>();
            foreach (Piece piece in board) {
                if (piece != null && piece.color == nextPlayer) {
                    // pawn
                    if (piece.type == Type.Pawn) {
                        Point dir = MoveRule.pawnMoveDir[(int)nextPlayer];
                        Point newPos = piece.pos;
                        newPos.Offset(dir);
                        if (board.At(newPos) == null) {
                            // one field move
                            AddPawnMoveWithChangeTo(nextMoves, piece, newPos, null);
                            // two field move
                            if (piece.pos.Y == MoveRule.pawnStartY[(int)nextPlayer]) {
                                Point newPos2 = newPos;
                                newPos2.Offset(dir);
                                if (board.At(newPos2) == null) {
                                    AddPawnMoveWithChangeTo(nextMoves, piece, newPos2, null);
                                }
                            }
                        }
                        // pawn capturing
                        foreach (Point captureDir in MoveRule.pawnCaptureDirs[(int)nextPlayer]) {
                            Point capturePos = piece.pos;
                            capturePos.Offset(captureDir);
                            if (capturePos.IsValidInChess()) {
                                Piece attackedPiece = GetAttackedPiece(capturePos);
                                if (attackedPiece != null && attackedPiece.color != nextPlayer) {
                                    AddPawnMoveWithChangeTo(nextMoves, piece, capturePos, attackedPiece);
                                }
                            }
                        }
                        // rest of pieces
                    } else {
                        MoveRule rule = MoveRule.rules[(int)piece.type];
                        foreach (Point dir in rule.dirs) {
                            Point newPos = piece.pos;
                            newPos.Offset(dir);
                            while (newPos.IsValidInChess()) {
                                Piece attackedPiece = board.At(newPos);
                                if (attackedPiece == null || attackedPiece.color != nextPlayer) {
                                    Piece capturedPiece = attackedPiece;
                                    if (fragilePiece != null && newPos == fragileField) {
                                        capturedPiece = fragilePiece;
                                    }
                                    nextMoves.Add(new Move(piece, newPos, capturedPiece, fragilePiece, fragileField, halfMoveClock));
                                }
                                if (!rule.recursive || attackedPiece != null) {
                                    break;
                                }
                                newPos.Offset(dir);
                            }
                        }
                    }
                }
            }
            for (int side = 0; side < 2; side++) {
                if (IsCastlingPossible(nextPlayer, side)) {
                    bool clear = true;
                    foreach (Point point in MoveRule.castleEmpty[(int)nextPlayer, side]) {
                        if (board.At(point) != null) {
                            clear = false;
                            break;
                        }
                    }
                    if (clear) {
                        nextMoves.Add(new Move(
                            board.At(MoveRule.initKingPos[(int)nextPlayer]),
                            MoveRule.kingPosAfterCastle[(int)nextPlayer, side],
                            null, fragilePiece, fragileField, halfMoveClock));
                    }
                }
            }
            return nextMoves;
        }

        public GameState MakeMove(Move move) {
            GameState state = GameState.Playing;
            Piece piece = move.piece;
            Point to = move.to;
            // capture
            if (move.capture != null) {
                if (move.capture.type == Type.King) {
                    state = GameState.Win;
                }
                board.At(move.capture.pos) = null;
            }
            // activate capture by passing for next move
            if (piece.type == Type.Pawn &&
                move.from.Y == MoveRule.pawnStartY[(int)nextPlayer] &&
                to.Y == MoveRule.pawnTwoFieldY[(int)nextPlayer]) {
                fragileField.X = to.X;
                fragileField.Y = MoveRule.pawnPassingY[(int)nextPlayer];
                fragilePiece = piece;
            } else {
                fragilePiece = null;
            }
            // move
            MovePiece(piece, to);
            piece.moveCount++;
            // castling
            if (piece.type == Type.King && move.from == MoveRule.initKingPos[(int)nextPlayer]) {
                for (int side = 0; side < 2; side++) {
                    if (move.to == MoveRule.kingPosAfterCastle[(int)nextPlayer, side]) {
                        Piece rook = board.At(MoveRule.initRookPos[(int)nextPlayer, side]);
                        MovePiece(rook, MoveRule.rookPosAfterCastle[(int)nextPlayer, side]);
                        break;
                    }
                }
            }
            // promotion
            piece.type = move.changeTo;
            // next player
            nextPlayer = Chess.Opponent(nextPlayer);
            // clocks
            if (move.capture == null && piece.type != Type.Pawn) {
                halfMoveClock++;
            } else {
                halfMoveClock = 0;
            }
            if (nextPlayer == Color.White) {
                fullMoveNumber++;
            }

            return state;
        }

        public void UndoMove(Move lastMove) {
            Piece piece = lastMove.piece;
            // clocks
            if (nextPlayer == Color.White) {
                fullMoveNumber--;
            }
            halfMoveClock = lastMove.prevHalfMoveClock;
            // prev player
            nextPlayer = Chess.Opponent(nextPlayer);
            // cancel promotion
            piece.type = lastMove.changeFrom;
            // reverse castling
            if (piece.type == Type.King && lastMove.from == MoveRule.initKingPos[(int)nextPlayer]) {
                for (int side = 0; side < 2; side++) {
                    if (lastMove.to == MoveRule.kingPosAfterCastle[(int)nextPlayer, side]) {
                        Piece rook = board.At(MoveRule.rookPosAfterCastle[(int)nextPlayer, side]);
                        MovePiece(rook, MoveRule.initRookPos[(int)nextPlayer, side]);
                        break;
                    }
                }
            }
            // move back
            MovePiece(piece, lastMove.from);
            piece.moveCount--;
            // activate capture by passing for prev move
            fragilePiece = lastMove.prevFragilePiece;
            fragileField = lastMove.prevFragileField;
            // bring back captured piece
            if (lastMove.capture != null) {
                AddPiece(lastMove.capture);
            }
        }

        /*public float AlphaBeta(Color player, int depth, float alpha = float.MinValue, float beta = float.MaxValue) {
            if (depth == 0) {
                return 1.0F / (1.0F + (float)Math.Exp((scores[(int)Chess.Opponent(player)] - scores[(int)player]) * 0.005));
            } else {
                depth--;
                List<Move> moves = GetMoves();
                if (nextPlayer == player) {
                    foreach (Move move in moves) {
                        GameState state = MakeMove(move);
                        if (state == GameState.Win) {
                            alpha = 1.0F;
                        } else {
                            alpha = Math.Max(alpha, AlphaBeta(player, depth, alpha, beta));
                        }
                        UndoMove(move);
                        if (alpha >= beta) {
                            break;
                        }
                    }
                    return alpha;
                } else {
                    foreach (Move move in moves) {
                        GameState state = MakeMove(move);
                        if (state == GameState.Win) {
                            beta = 0.0F;
                        } else {
                            beta = Math.Min(beta, AlphaBeta(player, depth, alpha, beta));
                        }
                        UndoMove(move);
                        if (alpha >= beta) {
                            break;
                        }
                    }
                    return beta;
                }
            }
        }*/

        public string FenPosition {
            get {
                string ret = "";
                for (int y = 0; y != height; y++) {
                    int emptyCount = 0;
                    for (int x = 0; x != width; x++) {
                        Point point = new Point(x, y);
                        Piece piece = board.At(point);
                        if (piece == null) {
                            emptyCount++;
                        } else {
                            if (emptyCount > 0) {
                                ret += emptyCount;
                                emptyCount = 0;
                            }
                            ret += piece.Fen;
                        }
                    }
                    if (emptyCount > 0) {
                        ret += emptyCount;
                    }
                    if (y + 1 != height) {
                        ret += "/";
                    }
                }
                if (nextPlayer == Color.White) {
                    ret += " w";
                } else {
                    ret += " b";
                }

                string castling = "";
                if (IsCastlingPossible(Color.White, 1)) castling += "K";
                if (IsCastlingPossible(Color.White, 0)) castling += "Q";
                if (IsCastlingPossible(Color.Black, 1)) castling += "k";
                if (IsCastlingPossible(Color.Black, 0)) castling += "q";
                if (castling.Length == 0) {
                    castling = "-";
                }
                ret += " " + castling;

                if (fragilePiece != null) {
                    ret += " " + fragileField.ToChessString();
                } else {
                    ret += " -";
                }

                ret += " " + halfMoveClock + " " + fullMoveNumber;
                return ret;
            }
        }

        public Point GetPosFromString(string pos) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Point point = new Point(x, y);
                    string ret = point.ToChessString();
                    if (ret.Equals(pos)) {
                        return point;
                    }
                }
            }
            return Point.Empty;
        }

        public Move GetBestMove(int time) {
            stockfish.SetFenPosition(FenPosition);
            Console.WriteLine("Best move of: " + stockfish.GetFenPosition());
            string ret = stockfish.GetBestMoveTime(time);
            Console.WriteLine("Best move: " + ret);
            if (ret != null) {
                Point from = GetPosFromString(ret.Substring(0, 2));
                Point to = GetPosFromString(ret.Substring(2, 2));
                Piece piece = board.At(from);
                if (ret.Length == 4) {
                    return new Move(piece, to, GetAttackedPiece(to), fragilePiece, fragileField, halfMoveClock);
                } else if (ret.Length == 5) {
                    foreach (Type type in MoveRule.pawnChangeTypes) {
                        if (Piece.fen[0, (int)type][0] == ret[4] || Piece.fen[1, (int)type][0] == ret[4]) {
                            return new Move(piece, to, GetAttackedPiece(to), type, fragilePiece, fragileField, halfMoveClock);
                        }
                    }
                }
            }
            return null;
        }

        public bool IsCheck(Move move) {
            bool isCheck = false;
            MakeMove(move);
            List<Move> moves = GetMovesInaccurate(Chess.Opponent(nextPlayer));
            foreach (Move move2 in moves) {
                Piece to = board.At(move2.to);
                if (to != null && to.type == Type.King) {
                    isCheck = true;
                    break;
                }
            }
            UndoMove(move);
            return isCheck;
        }

        public bool IsMate(Move move) {
            bool isMate = true;
            MakeMove(move);
            List<Move> moves = GetMovesInaccurate();
            foreach (Move move2 in moves) {
                MakeMove(move2);
                List<Move> moves2 = GetMovesInaccurate();
                bool killed = false;
                foreach (Move move3 in moves2) {
                    Piece to = board.At(move3.to);
                    if (to != null && to.type == Type.King) {
                        killed = true;
                        break;
                    }
                }
                UndoMove(move2);
                if (!killed) {
                    isMate = false;
                    break;
                }
            }
            UndoMove(move);
            return isMate;
        }

        /*// TODO: stalemate
        // TODO: not enough material
        // TODO: repeated pos / no progress

        public List<Move> GetMovesWithProbs(DateTime timeout) {
            List<Move> moves = GetMoves();
            List<Move> checkOrder = new List<Move>(moves);
            Dictionary<Move, int> notBestSince = new Dictionary<Move, int>();
            foreach (Move move in moves) {
                notBestSince.Add(move, 0);
            }
            int depth = 0;
            Color player = nextPlayer;
            Console.WriteLine("\nGet moves for " + Chess.colorNames[(int)player]);
            while (DateTime.Now < timeout && checkOrder.Count > 1) {
                Console.WriteLine("\nDepth: " + depth);
                float best = float.MinValue;
                for (int i = 0; i < checkOrder.Count; i++) {
                    Move move = checkOrder[i];
                    GameState state = MakeMove(move);
                    if (state == GameState.Win) {
                        move.winningProb = 1.0F;
                    } else {
                        move.winningProb = AlphaBeta(player, depth);
                    }
                    UndoMove(move);
                    Console.WriteLine(MoveShortString(move) + " - " + MoveLongString(move) + ": " + move.winningProb);
                    if (!(move.winningProb > 0.0F && move.winningProb < 1.0F)) {
                        checkOrder.RemoveAt(i);
                        i--;
                    }
                    best = Math.Max(best, move.winningProb);
                }
                for (int i = 0; i < checkOrder.Count; i++) {
                    Move move = checkOrder[i];
                    if (move.winningProb < best) {
                        notBestSince[move]++;
                        if (notBestSince[move] >= 4) {
                            checkOrder.RemoveAt(i);
                            i--;
                            move.winningProb = 0.001F;
                        }
                    } else {
                        notBestSince[move] = 0;
                    }
                }
                checkOrder.Sort((a, b) => b.winningProb.CompareTo(a.winningProb));
                depth++;
                if (best == 1.0F) {
                    break;
                }
            }
            return moves.OrderByDescending(m => m.winningProb).ThenByDescending(m => {
                Point king = OpponentKingPos;
                return m.from.GetVectorTo(king).Distance() - m.to.GetVectorTo(king).Distance();
            }).ToList();
        }*/

        public bool Update(BoardView boardView, int maxMoves) {
            if (rotated ? Equals(boardView.Rotate180()) : Equals(boardView)) {
                return true;
            }
            else if (maxMoves > 0) {
                maxMoves--;
                List<Move> moves = GetMovesInaccurate();
                foreach (Move move in moves) {
                    MakeMove(move);
                    if (Update(boardView, maxMoves)) {
                        return true;
                    }
                    UndoMove(move);
                }
            }
            return false;
        }

        public string MoveShortString(Move move) {
            string ret = "";
            if (move.IsShortCastle) {
                ret += "O-O";
            }
            else if (move.IsLongCastle) {
                ret += "O-O-O";
            }
            else {
                List<Move> moves = GetMovesInaccurate();
                bool addX = move.capture != null && move.piece.type == Type.Pawn;
                bool addY = false;
                if (move.piece.type != Type.Pawn) {
                    foreach (Move otherMove in moves) {
                        if (otherMove != move && otherMove.piece.type == move.piece.type && otherMove.to == move.to) {
                            if (otherMove.from.X == move.from.X) {
                                addY = true;
                            } else {
                                addX = true;
                            }
                        }
                    }
                }
                if (Thread.CurrentThread.CurrentUICulture.Name == "pl-PL") {
                    ret += move.piece.PolishSymbol;
                    if (addX) {
                        ret += xNames[move.from.X];
                    } else if (addY) {
                        ret += yNames[move.from.Y];
                    }
                    if (move.capture != null) {
                        ret += "x";
                    }
                    ret += move.to.ToChessString();
                    if (move.changeFrom != move.changeTo) {
                        ret += "=" + Piece.polishSymbols[(int)move.changeTo];
                    }
                } else {
                    ret += move.piece.Symbol;
                    if (addX) {
                        ret += xNames[move.from.X];
                    } else if (addY) {
                        ret += yNames[move.from.Y];
                    }
                    if (move.capture != null) {
                        ret += "x";
                    }
                    ret += move.to.ToChessString();
                    if (move.changeFrom != move.changeTo) {
                        ret += "=" + Piece.symbols[(int)move.changeTo];
                    }
                }
            }
            if (IsCheck(move)) {
                if (IsMate(move)) {
                    ret += "#";
                } else {
                    ret += "+";
                }
            }
            return ret;
        }

        public string MoveLongString(Move move) {
            string ret = "";
            List<Move> moves = GetMovesInaccurate();
            bool addPos = false;
            foreach (Move otherMove in moves) {
                if (otherMove != move && otherMove.piece.type == move.piece.type && otherMove.to == move.to) {
                    addPos = true;
                    break;
                }
            }
            if (Thread.CurrentThread.CurrentUICulture.Name == "pl-PL") {
                if (move.IsShortCastle) {
                    ret += "krótka roszada";
                }
                else if (move.IsLongCastle) {
                    ret += "długa roszada";
                }
                else {
                    if (addPos) {
                        ret += move.from.ToChessString();
                    } else {
                        ret += move.piece.PolishName;
                    }
                    if (move.capture != null) {
                        ret += " bierze ";
                    } else {
                        ret += " na ";
                    }
                    ret += move.to.ToChessString();
                    if (move.changeFrom != move.changeTo) {
                        ret += " i promuje do " + Piece.polishPromotionNames[(int)move.changeTo];
                    }
                }
                if (IsCheck(move)) {
                    if (IsMate(move)) {
                        ret += ", szach mat";
                    } else {
                        ret += ", szach";
                    }
                }
            } else {
                if (move.IsShortCastle) {
                    ret += "kingside castle";
                }
                else if (move.IsLongCastle) {
                    ret += "queenside castle";
                }
                else {
                    if (addPos) {
                        ret += move.from.ToChessString();
                    } else {
                        ret += move.piece.Name;
                    }
                    if (move.capture != null) {
                        ret += " takes ";
                    } else {
                        ret += " to ";
                    }
                    ret += move.to.ToChessString();
                    if (move.changeFrom != move.changeTo) {
                        ret += " and promotes to the " + Piece.names[(int)move.changeTo];
                    }
                }
                if (IsCheck(move)) {
                    if (IsMate(move)) {
                        ret += ", checkmate";
                    } else {
                        ret += ", check";
                    }
                }
            }
            return ret;
        }
    }
}