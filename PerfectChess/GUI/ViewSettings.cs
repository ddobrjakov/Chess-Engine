using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    public static class ViewSettings
    {
        public const int SQUARESIZE = 60;
        public static readonly System.Drawing.Color BACKGROUND_COLOR = System.Drawing.Color.FromArgb(26, 26, 26);
        public static readonly System.Drawing.Color WHITE_SQUARE_COLOR = System.Drawing.Color.FromArgb(240, 217, 181);
        public static readonly System.Drawing.Color BLACK_SQUARE_COLOR = System.Drawing.Color.FromArgb(181, 136, 99);

        public static readonly System.Drawing.Color WHITE_AVAILIBLE_COLOR = System.Drawing.Color.FromArgb(240, 149, 119);
        public static readonly System.Drawing.Color BLACK_AVAILIBLE_COLOR = System.Drawing.Color.FromArgb(181, 105, 74);


        public static readonly Image WHITE_PAWN = Image.FromFile("../../../images/WhitePawn.png");
        public static readonly Image WHITE_KNIGHT = Image.FromFile("../../../images/WhiteKnight.png");
        public static readonly Image WHITE_BISHOP = Image.FromFile("../../../images/WhiteBishop.png");
        public static readonly Image WHITE_ROOK = Image.FromFile("../../../images/WhiteRook.png");
        public static readonly Image WHITE_QUEEN = Image.FromFile("../../../images/WhiteQueen.png");
        public static readonly Image WHITE_KING = Image.FromFile("../../../images/WhiteKing.png");

        public static readonly Image BLACK_PAWN = Image.FromFile("../../../images/BlackPawn.png");
        public static readonly Image BLACK_KNIGHT = Image.FromFile("../../../images/BlackKnight.png");
        public static readonly Image BLACK_BISHOP = Image.FromFile("../../../images/BlackBishop.png");
        public static readonly Image BLACK_ROOK = Image.FromFile("../../../images/BlackRook.png");
        public static readonly Image BLACK_QUEEN = Image.FromFile("../../../images/BlackQueen.png");
        public static readonly Image BLACK_KING = Image.FromFile("../../../images/BlackKing.png");


        public static readonly Image CIRCLE_FILLED = Image.FromFile("../../../images/CircleFilled.png");
    }
}
