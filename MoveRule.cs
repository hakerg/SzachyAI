using System.Drawing;

namespace SzachyAI {

    public class MoveRule {
        public Point[] dirs;
        public bool recursive;

        public static Point N = new Point(0, -1);
        public static Point NE = new Point(1, -1);
        public static Point E = new Point(1, 0);
        public static Point SE = new Point(1, 1);
        public static Point S = new Point(0, 1);
        public static Point SW = new Point(-1, 1);
        public static Point W = new Point(-1, 0);
        public static Point NW = new Point(-1, -1);

        public static Point[] axes = new Point[] { N, E, S, W };
        public static Point[] diagonals = new Point[] { NE, SE, SW, NW };
        public static Point[] allDirs = new Point[] { N, NE, E, SE, S, SW, W, NW };

        public static Point[] knightDirs = new Point[] {
            new Point(1, -2),
            new Point(2, -1),
            new Point(2, 1),
            new Point(1, 2),
            new Point(-1, 2),
            new Point(-2, 1),
            new Point(-2, -1),
            new Point(-1, -2)
        };

        public static MoveRule[] rules = new MoveRule[] {
            null,
            new MoveRule(axes, true),
            new MoveRule(knightDirs, false),
            new MoveRule(diagonals, true),
            new MoveRule(allDirs, true),
            new MoveRule(allDirs, false)
        };

        public static Point[] pawnMoveDir = new Point[] { N, S };

        public static Point[][] pawnCaptureDirs = new Point[][] {
            new Point[] { NW, NE },
            new Point[] { SW, SE }
        };

        public static int[] pawnStartY = new int[] { 6, 1 };

        public static int[] pawnPassingY = new int[] { 5, 2 };

        public static int[] pawnTwoFieldY = new int[] { 4, 3 };

        public static int[] pawnFinalY = new int[] { 0, 7 };

        public static Type[] pawnChangeTypes = new Type[] {
            Type.Rook, Type.Knight, Type.Bishop, Type.Queen
        };

        public MoveRule(Point[] dirs, bool recursive) {
            this.dirs = dirs;
            this.recursive = recursive;
        }
    }
}