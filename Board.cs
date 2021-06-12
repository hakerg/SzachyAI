using System;
using System.Collections.Concurrent;
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
        public GameState gameState = GameState.Playing;
        public ConcurrentDictionary<string, int> repeatedFens = new ConcurrentDictionary<string, int>();

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
            score = boardView.score;
            if (whiteAtBottomVotes >= blackAtBottomVotes) {
                board = boardView.board;
                nextPlayer = Color.White;
                rotated = false;
            } else {
                board = boardView.Rotate180().board;
                nextPlayer = Color.Black;
                rotated = true;
            }
            if (FenLayout.Equals(initLayout)) {
                nextPlayer = Color.White;
            }
            repeatedFens.AddOrUpdate(FenPositionNoClocks, 1, (_, count) => count + 1);
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

        public bool IsCastlingAvailable(Color player, int side) {
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

        public bool[,] GetAttackedFields(Color attacker) {
            bool[,] fields = new bool[width, height];
            List<Move> moves = GetMovesInaccurate(attacker, false);
            foreach (Move move in moves) {
                fields.At(move.to) = true;
            }
            return fields;
        }

        public List<Move> GetMovesInaccurate() {
            return GetMovesInaccurate(nextPlayer);
        }

        public bool CanMakeCastle(Color nextPlayer, int side, ref bool[,] attacked) {
            if (IsCastlingAvailable(nextPlayer, side)) {
                foreach (Point point in MoveRule.castleEmpty[(int)nextPlayer, side]) {
                    if (board.At(point) != null) {
                        return false;
                    }
                }
                if (attacked == null) {
                    attacked = GetAttackedFields(Chess.Opponent(nextPlayer));
                }
                if (attacked.At(MoveRule.initKingPos[(int)nextPlayer]) ||
                    attacked.At(MoveRule.initRookPos[(int)nextPlayer, side])) {
                    return false;
                }
                foreach (Point point in MoveRule.castleEmpty[(int)nextPlayer, side]) {
                    if (attacked.At(point)) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public List<Move> GetMovesInaccurate(Color nextPlayer, bool castling = true) {
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
            // castling
            if (castling) {
                bool[,] attacked = null;
                for (int side = 0; side < 2; side++) {
                    if (CanMakeCastle(nextPlayer, side, ref attacked)) {
                        nextMoves.Add(new Move(
                            board.At(MoveRule.initKingPos[(int)nextPlayer]),
                            MoveRule.kingPosAfterCastle[(int)nextPlayer, side],
                            null, fragilePiece, fragileField, halfMoveClock));
                    }
                }
            }
            if (nextPlayer == Color.Black) {
                nextMoves.Reverse();
            }
            return nextMoves;
        }

        public void MakeMove(Move move) {
            Piece piece = move.piece;
            Point to = move.to;
            // capture
            if (move.capture != null) {
                if (move.capture.type == Type.King) {
                    gameState = GameState.Win;
                }
                RemovePiece(move.capture.pos);
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
            score[(int)piece.color] -= piece.Score;
            piece.type = move.changeTo;
            score[(int)piece.color] += piece.Score;
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
            if (gameState == GameState.Playing && halfMoveClock == 100) {
                gameState = GameState.Draw;
            }
        }

        public void UndoMove(Move lastMove) {
            Piece piece = lastMove.piece;
            gameState = GameState.Playing;
            // clocks
            if (nextPlayer == Color.White) {
                fullMoveNumber--;
            }
            halfMoveClock = lastMove.prevHalfMoveClock;
            // prev player
            nextPlayer = Chess.Opponent(nextPlayer);
            // cancel promotion
            score[(int)piece.color] -= piece.Score;
            piece.type = lastMove.changeFrom;
            score[(int)piece.color] += piece.Score;
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

        public bool Update(BoardView boardView, int maxMoves) {
            if (rotated ? Equals(boardView.Rotate180()) : Equals(boardView)) {
                return true;
            } else if (maxMoves > 0) {
                maxMoves--;
                List<Move> moves = GetMovesInaccurate();
                foreach (Move move in moves) {
                    MakeMove(move);
                    string fen = FenPositionNoClocks;
                    if (Update(boardView, maxMoves)) {
                        repeatedFens.AddOrUpdate(fen, 1, (_, count) => count + 1);
                        return true;
                    }
                    UndoMove(move);
                }
            }
            return false;
        }

        public string FenPositionNoClocks {
            get {
                string ret = FenLayout;

                if (nextPlayer == Color.White) {
                    ret += " w";
                } else {
                    ret += " b";
                }

                string castling = "";
                if (IsCastlingAvailable(Color.White, 1)) castling += "K";
                if (IsCastlingAvailable(Color.White, 0)) castling += "Q";
                if (IsCastlingAvailable(Color.Black, 1)) castling += "k";
                if (IsCastlingAvailable(Color.Black, 0)) castling += "q";
                if (castling.Length == 0) {
                    castling = "-";
                }
                ret += " " + castling;

                if (fragilePiece != null) {
                    ret += " " + fragileField.ToChessString();
                } else {
                    ret += " -";
                }

                return ret;
            }
        }

        public string FenPosition => FenPositionNoClocks + " " + halfMoveClock + " " + fullMoveNumber;

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

        public Move GetStockfishMove(int time) {
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

        public int AlphaBeta(Color player, int depth, int alpha = int.MinValue, int beta = int.MaxValue) {
            if (gameState == GameState.Win) {
                return nextPlayer == player ? -100000 : 100000;
            } else if (gameState == GameState.Draw) {
                return 0;
            }
            else if (depth == 0) {
                return score[(int)player] - score[(int)Chess.Opponent(player)];
            } else {
                depth--;
                List<Move> moves = GetMovesInaccurate();
                if (nextPlayer == player) {
                    foreach (Move move in moves) {
                        MakeMove(move);
                        alpha = Math.Max(alpha, AlphaBeta(player, depth, alpha, beta));
                        UndoMove(move);
                        if (alpha >= beta) {
                            break;
                        }
                    }
                    return alpha;
                } else {
                    foreach (Move move in moves) {
                        MakeMove(move);
                        beta = Math.Min(beta, AlphaBeta(player, depth, alpha, beta));
                        UndoMove(move);
                        if (alpha >= beta) {
                            break;
                        }
                    }
                    return beta;
                }
            }
        }

        // TODO: not enough material

        public void CalculateMoveScore(Move move) {
            Color player = nextPlayer;
            MakeMove(move);
            if (IsLastMoveStalemate()) {
                move.score = 0;
            } else if (repeatedFens.TryGetValue(FenPositionNoClocks, out int count) && count >= 1) { // 2 repeats end game faster
                move.score = 0;
            } else {
                move.score = AlphaBeta(player, move.depth);
            }
            UndoMove(move);
            Console.WriteLine(move.StockfishString + ", depth " + (move.depth + 1) + ": " + move.score + " cp");
        }

        public Move GetBestMove(DateTime timeout) {
            List<Move> moves = GetMovesInaccurate();
            if (moves.Count == 1) {
                return moves[0];
            }
            if (moves.Count > 1) {
                Console.WriteLine("\nEvaluate moves for " + Chess.colorNames[(int)nextPlayer]);
                Dictionary<Move, int> notBestSince = new Dictionary<Move, int>();
                foreach (Move move in moves) {
                    notBestSince.Add(move, 0);
                }
                while (DateTime.Now < timeout) {
                    int best = int.MinValue;
                    foreach (Move move in moves) {
                        if (notBestSince[move] < 4) {
                            move.depth++;
                            CalculateMoveScore(move);
                            if (move.score == 100000) {
                                return move;
                            } else if (move.score == -100000) {
                                notBestSince[move] = 4;
                            }
                            best = Math.Max(best, move.score);
                        }
                    }
                    int checkCount = 0;
                    foreach (Move move in moves) {
                        if (move.score >= best) {
                            notBestSince[move] = 0;
                        } else {
                            notBestSince[move]++;
                        }
                        if (notBestSince[move] < 4) {
                            checkCount++;
                        }
                    }
                    if (checkCount <= 1) {
                        break;
                    }
                }
                moves.Sort((a, b) => b.score.CompareTo(a.score));
                return moves[0];
            }
            return null;
        }

        public bool IsLastMoveCheck() {
            List<Move> moves = GetMovesInaccurate(Chess.Opponent(nextPlayer));
            foreach (Move move in moves) {
                Piece to = board.At(move.to);
                if (to != null && to.type == Type.King) {
                    return true;
                }
            }
            return false;
        }

        public bool IsLastMoveMate() {
            List<Move> moves = GetMovesInaccurate();
            foreach (Move move in moves) {
                MakeMove(move);
                List<Move> moves2 = GetMovesInaccurate();
                bool killed = false;
                foreach (Move move2 in moves2) {
                    Piece to = board.At(move2.to);
                    if (to != null && to.type == Type.King) {
                        killed = true;
                        break;
                    }
                }
                UndoMove(move);
                if (!killed) {
                    return false;
                }
            }
            return true;
        }

        public bool IsLastMoveStalemate() {
            return !IsLastMoveCheck() && IsLastMoveMate();
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
            MakeMove(move);
            if (IsLastMoveCheck()) {
                if (IsLastMoveMate()) {
                    ret += "#";
                } else {
                    ret += "+";
                }
            }
            UndoMove(move);
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
                MakeMove(move);
                if (IsLastMoveCheck()) {
                    if (IsLastMoveMate()) {
                        ret += ", szach mat";
                    } else {
                        ret += ", szach";
                    }
                }
                UndoMove(move);
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
                MakeMove(move);
                if (IsLastMoveCheck()) {
                    if (IsLastMoveMate()) {
                        ret += ", checkmate";
                    } else {
                        ret += ", check";
                    }
                }
                UndoMove(move);
            }
            return ret;
        }
    }
}