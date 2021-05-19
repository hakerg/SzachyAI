using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SzachyAI {

    public class BoardRecognizer {

        // [bg color][piece color][piece type]
        public Image<Bgr, byte>[,,] templatePieces = new Image<Bgr, byte>[(int)Color.Length, (int)Color.Length, (int)Type.Length];

        public Image<Bgr, byte>[] templateFields = new Image<Bgr, byte>[(int)Color.Length];
        public Size fieldSize;
        public Rectangle clippedROI;
        public Size boardSize;
        public Dictionary<Size, Image<Bgr, byte>> emptyBoards = new Dictionary<Size, Image<Bgr, byte>>();
        public Dictionary<Size, Image<Bgr, byte>> masks = new Dictionary<Size, Image<Bgr, byte>>();
        public double minScaleStep;

        public void SetFieldSize(Size size) {
            if (fieldSize.IsEmpty) {
                fieldSize = size;
                clippedROI = new Rectangle(size.Width / 4, size.Height / 4, size.Width / 2, size.Height / 2);
            } else if (!fieldSize.Equals(size)) {
                throw new Exception("Template images should have the same size");
            }
        }

        public Image<Bgr, byte> ConstructBoardImage(Board board, Color backgroundColor = (Color)(-1)) {
            Image<Bgr, byte> image = new Image<Bgr, byte>(boardSize);
            for (int y = 0; y < Board.height; y++) {
                for (int x = 0; x < Board.width; x++) {
                    int bc = backgroundColor == (Color)(-1) ? (x + y) % 2 : (int)backgroundColor;
                    Point start = new Point(x * fieldSize.Width, y * fieldSize.Height);
                    image.ROI = new Rectangle(start, fieldSize);
                    Piece piece = board.PieceAt(new Point(x, y));
                    if (piece == null) {
                        templateFields[bc].CopyTo(image);
                    } else {
                        templatePieces[bc, (int)piece.color, (int)piece.type].CopyTo(image);
                    }
                }
            }
            image.ROI = Rectangle.Empty;
            return image;
        }

        public Image<Bgr, byte> CreateBoardMask() {
            Image<Bgr, byte> mask = new Image<Bgr, byte>(boardSize.Width, boardSize.Height, new Bgr(255, 255, 255));
            for (int y = 0; y < Board.height; y++) {
                for (int x = 0; x < Board.width; x++) {
                    Point start = new Point(x * fieldSize.Width + clippedROI.X, y * fieldSize.Height + clippedROI.Y);
                    mask.ROI = new Rectangle(start, clippedROI.Size);
                    mask.SetZero();
                    mask.ROI = Rectangle.Empty;
                }
            }
            return mask;
        }

        public void LoadTemplate(string templatePath) {
            for (int bc = 0; bc < (int)Color.Length; bc++) {
                string bgColor = Chess.colorNames[bc];
                string path = Path.Combine(templatePath, bgColor + ".png");
                templateFields[bc] = new Image<Bgr, byte>(path);
                SetFieldSize(templateFields[bc].Size);
                for (int pc = 0; pc < (int)Color.Length; pc++) {
                    string pieceColor = Chess.colorNames[pc];
                    for (int p = 0; p < (int)Type.Length; p++) {
                        string fileName = pieceColor + "_" + Piece.names[p] + "_on_" + bgColor + ".png";
                        path = Path.Combine(templatePath, fileName);
                        templatePieces[bc, pc, p] = new Image<Bgr, byte>(path);
                        SetFieldSize(templatePieces[bc, pc, p].Size);
                    }
                }
            }
            boardSize = new Size(fieldSize.Width * Board.width, fieldSize.Height * Board.height);
            if (boardSize.Width > boardSize.Height) {
                minScaleStep = 1.0 / boardSize.Width;
            } else {
                minScaleStep = 1.0 / boardSize.Height;
            }
            emptyBoards[boardSize] = ConstructBoardImage(new Board());
            masks[boardSize] = CreateBoardMask();
        }

        public BoardRecognizer(string templatePath = "template") {
            LoadTemplate(templatePath);
        }

        public static Bitmap CaptureScreen() {
            Rectangle vsBounds = SystemInformation.VirtualScreen;
            Bitmap bitmap = new Bitmap(vsBounds.Width, vsBounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(vsBounds.Left, vsBounds.Top, 0, 0, bitmap.Size);
            return bitmap;
        }

        public void DetectBoardCorners(Image<Bgr, byte> image, double boardScale,
            out Rectangle corners, out double score) {
            Size size = new Size((int)(boardSize.Width * boardScale), (int)(boardSize.Height * boardScale));
            Image<Bgr, byte> scaledBoard;
            Image<Bgr, byte> mask;
            if (emptyBoards.ContainsKey(size)) {
                scaledBoard = emptyBoards[size];
                mask = masks[size];
            } else {
                scaledBoard = emptyBoards[boardSize].Resize(size.Width, size.Height, Inter.Linear);
                mask = masks[boardSize].Resize(size.Width, size.Height, Inter.Linear);
                emptyBoards[size] = scaledBoard;
                masks[size] = mask;
            }
            try {
                Image<Gray, float> match = new Image<Gray, float>
                    (image.Width - scaledBoard.Width + 1, image.Height - scaledBoard.Height + 1);
                CvInvoke.MatchTemplate(image, scaledBoard, match, TemplateMatchingType.SqdiffNormed, mask);
                match.MinMax(out double[] minValues, out _, out Point[] bestMatches, out _);
                score = minValues[0];
                bestMatches[0].Offset(image.ROI.Location);
                corners = new Rectangle(bestMatches[0], size);
            } catch (CvException) {
                corners = Rectangle.Empty;
                score = double.MaxValue;
            }
        }

        public bool DetectBoardCorners(Image<Bgr, byte> image, out Rectangle corners,
            double scaleMin = 0.5, double scaleMax = 2.0, double scaleStep = 0.1) {
            corners = Rectangle.Empty;
            double bestScore = double.MaxValue, bestScale = scaleMin;
            for (double scale = scaleMin; scale <= scaleMax; scale += scaleStep) {
                DetectBoardCorners(image, scale, out Rectangle newCorners, out double score);
                if (score < bestScore) {
                    bestScore = score;
                    corners = newCorners;
                    bestScale = scale;
                }
                if (scale + scaleStep > scaleMax && scaleStep > minScaleStep) {
                    scale = bestScale - 0.75 * scaleStep;
                    scaleMax = bestScale + scaleStep;
                    scaleStep *= 0.5;
                    image.ROI = Rectangle.Empty;
                    Rectangle newRoi = corners;
                    newRoi.Inflate(fieldSize);
                    newRoi.Intersect(new Rectangle(Point.Empty, image.Size));
                    image.ROI = newRoi;
                }
            }
            image.ROI = Rectangle.Empty;
            return bestScore < 0.1;
        }

        public Image<Bgr, byte> GetScaledBoardImage(Image<Bgr, byte> image, Rectangle boardCorners) {
            image.ROI = boardCorners;
            Image<Bgr, byte> boardImage = image.Copy();
            image.ROI = Rectangle.Empty;
            return boardImage.Resize(boardSize.Width, boardSize.Height, Inter.Lanczos4);
        }

        public bool RecognizeBoard(Image<Bgr, byte> boardImage, out Board board) {
            board = new Board();
            for (int y = 0; y < Board.height; y++) {
                for (int x = 0; x < Board.width; x++) {
                    int bc = (x + y) % 2;
                    Point start = new Point(x * fieldSize.Width, y * fieldSize.Height);
                    boardImage.ROI = new Rectangle(start, fieldSize);
                    templateFields[bc].ROI = clippedROI;
                    Image<Gray, float> fieldMatch = boardImage.MatchTemplate(templateFields[bc],
                        TemplateMatchingType.SqdiffNormed);
                    templateFields[bc].ROI = Rectangle.Empty;
                    fieldMatch.MinMax(out double[] fieldScores, out _, out _, out _);
                    double fieldScore = fieldScores[0];
                    double bestPieceScore = double.MaxValue;
                    int bestPC = 0, bestP = 0;
                    for (int pc = 0; pc < (int)Color.Length; pc++) {
                        for (int p = 0; p < (int)Type.Length; p++) {
                            templatePieces[bc, pc, p].ROI = clippedROI;
                            Image<Gray, float> pieceMatch = boardImage.MatchTemplate(templatePieces[bc, pc, p],
                                TemplateMatchingType.SqdiffNormed);
                            templatePieces[bc, pc, p].ROI = Rectangle.Empty;
                            pieceMatch.MinMax(out double[] pieceScores, out _, out _, out _);
                            double pieceScore = pieceScores[0];
                            if (pieceScore < bestPieceScore) {
                                bestPieceScore = pieceScore;
                                bestPC = pc;
                                bestP = p;
                            }
                        }
                    }
                    if (fieldScore > 0.2 && bestPieceScore > 0.2) {
                        boardImage.ROI = Rectangle.Empty;
                        return false;
                    }
                    if (bestPieceScore < fieldScore) {
                        Point pos = new Point(x, y);
                        board.AddPiece(new Piece(pos, (Type)bestP, (Color)bestPC, false));
                    }
                }
            }
            boardImage.ROI = Rectangle.Empty;
            return true;
        }
    }
}