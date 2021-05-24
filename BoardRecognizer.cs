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
        public double reduceFactor = 0.25;

        public void SetFieldSize(Size size) {
            if (fieldSize.IsEmpty) {
                fieldSize = size;
                clippedROI = new Rectangle(size.Width.Scale(0.125), size.Height.Scale(0.125),
                    size.Width.Scale(0.75), size.Height.Scale(0.75));
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

        public static Image<Bgr, byte> CaptureScreen(Screen screen) {
            Rectangle bounds = screen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(bounds.Location, Point.Empty, bitmap.Size);
            return bitmap.ToImage<Bgr, byte>();
        }

        public static List<Image<Bgr, byte>> CaptureScreens() {
            List<Image<Bgr, byte>> images = new List<Image<Bgr, byte>>();
            foreach (Screen screen in Screen.AllScreens) {
                images.Add(CaptureScreen(screen));
            }
            return images;
        }

        public void DetectBoardCorners(Image<Bgr, byte> image, Size boardSize,
            out Rectangle corners, out double score) {
            Image<Bgr, byte> scaledBoard;
            Image<Bgr, byte> mask;
            if (emptyBoards.ContainsKey(boardSize)) {
                scaledBoard = emptyBoards[boardSize];
                mask = masks[boardSize];
            } else {
                scaledBoard = emptyBoards[this.boardSize].Resize(boardSize.Width, boardSize.Height, Inter.Linear);
                mask = masks[this.boardSize].Resize(boardSize.Width, boardSize.Height, Inter.Linear);
                emptyBoards[boardSize] = scaledBoard;
                masks[boardSize] = mask;
            }
            try {
                Image<Gray, float> match = new Image<Gray, float>
                    (image.Width - scaledBoard.Width + 1, image.Height - scaledBoard.Height + 1);
                CvInvoke.MatchTemplate(image, scaledBoard, match, TemplateMatchingType.SqdiffNormed, mask);
                match.MinMax(out double[] minValues, out _, out Point[] bestMatches, out _);
                score = minValues[0];
                bestMatches[0].Offset(image.ROI.Location);
                corners = new Rectangle(bestMatches[0], boardSize);
            } catch (CvException) {
                corners = Rectangle.Empty;
                score = double.MaxValue;
            }
        }

        public double FindBestScale(Image<Bgr, byte> image, double scaleMin, double scaleMax,
            double scaleStep, out Rectangle corners, out double score) {
            double bestScale = 0.0;
            corners = Rectangle.Empty;
            score = double.MaxValue;
            for (double scale = scaleMin; scale <= scaleMax; scale += scaleStep) {
                DetectBoardCorners(image, boardSize.Scale(scale), out Rectangle newCorners, out double newScore);
                if (newScore < score) {
                    bestScale = scale;
                    corners = newCorners;
                    score = newScore;
                }
            }
            return bestScale;
        }

        public bool DetectBoardCorners(List<Image<Bgr, byte>> images, out Rectangle corners, out int imageIndex,
            double scaleMin = 0.25, double scaleMax = 4.0, double scaleStep = 0.1) {
            corners = Rectangle.Empty;
            imageIndex = 0;
            double bestScale = 0.0;
            double bestScore = double.MaxValue;
            for (int i = 0; i < images.Count; i++) {
                Image<Bgr, byte> image = images[i];
                Image<Bgr, byte> imageReduced = image.Resize(reduceFactor, Inter.Linear);
                double scale = FindBestScale(imageReduced, scaleMin * reduceFactor, scaleMax * reduceFactor,
                    scaleStep * reduceFactor, out Rectangle newCorners, out double score) / reduceFactor;
                if (score < bestScore) {
                    corners = newCorners.Scale(1.0 / reduceFactor);
                    imageIndex = i;
                    bestScale = scale;
                    bestScore = score;
                }
            }
            Image<Bgr, byte> bestImage = images[imageIndex];
            while (scaleStep > minScaleStep) {
                scaleStep *= 0.5;
                scaleMin = bestScale - scaleStep * 0.5;
                scaleMax = scaleMin + scaleStep;
                bestImage.ROI = Rectangle.Empty;
                Rectangle newRoi = corners;
                newRoi.Inflate(fieldSize.Scale(0.5));
                newRoi.Intersect(new Rectangle(Point.Empty, bestImage.Size));
                bestImage.ROI = newRoi;
                double scale = FindBestScale(
                    bestImage, scaleMin, scaleMax, scaleStep, out Rectangle newCorners, out double score);
                if (score < bestScore) {
                    corners = newCorners;
                    bestScale = scale;
                    bestScore = score;
                }
            }
            bestImage.ROI = Rectangle.Empty;
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
            int allowedMistakes = 4;
            for (int y = 0; y < Board.height && allowedMistakes >= 0; y++) {
                for (int x = 0; x < Board.width && allowedMistakes >= 0; x++) {
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
                    if (fieldScore > 0.1 && bestPieceScore > 0.1) {
                        allowedMistakes--;
                        if (fieldScore > 0.2 && bestPieceScore > 0.2) {
                            allowedMistakes = -1;
                        }
                    }
                    if (bestPieceScore < fieldScore) {
                        Point pos = new Point(x, y);
                        board.AddPiece(new Piece(pos, (Type)bestP, (Color)bestPC, false));
                    }
                }
            }
            boardImage.ROI = Rectangle.Empty;
            return allowedMistakes >= 0;
        }
    }
}