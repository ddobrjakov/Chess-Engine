using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    public static class ViewModelConnector
    {
        public static Dictionary<int, Image> PieceImage = new Dictionary<int, Image>
        {
            { Color.White | Piece.Pawn, ViewSettings.WHITE_PAWN },
            { Color.White | Piece.Knight, ViewSettings.WHITE_KNIGHT },
            { Color.White | Piece.Bishop, ViewSettings.WHITE_BISHOP },
            { Color.White | Piece.Rook, ViewSettings.WHITE_ROOK },
            { Color.White | Piece.Queen, ViewSettings.WHITE_QUEEN },
            { Color.White | Piece.King, ViewSettings.WHITE_KING },
            { Color.Black | Piece.Pawn, ViewSettings.BLACK_PAWN },
            { Color.Black | Piece.Knight, ViewSettings.BLACK_KNIGHT },
            { Color.Black | Piece.Bishop, ViewSettings.BLACK_BISHOP },
            { Color.Black | Piece.Rook, ViewSettings.BLACK_ROOK },
            { Color.Black | Piece.Queen, ViewSettings.BLACK_QUEEN },
            { Color.Black | Piece.King, ViewSettings.BLACK_KING }
        };
        public static Dictionary<int, System.Drawing.Color> RealColor = new Dictionary<int, System.Drawing.Color>
        {
            { Color.White, ViewSettings.WHITE_SQUARE_COLOR },
            { Color.Black, ViewSettings.BLACK_SQUARE_COLOR }
        };
    }
}
