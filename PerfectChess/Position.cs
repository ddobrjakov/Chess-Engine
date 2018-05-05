using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Piece;
using static PerfectChess.Color;

namespace PerfectChess
{
    public class Position
    {

        /// <summary>
        /// Возвращает битбоард по коду фигуры (все расположения этой фигуры), максимум 14 = 8(color)+ 6(piece);
        /// </summary>
        private UInt64[] PieceBitboard = new UInt64[15];
        private UInt64 WhitePawns => PieceBitboard[White | Pawn];
        private UInt64 WhiteKnights;
        private UInt64 WhiteBishops;
        private UInt64 WhiteRooks;
        private UInt64 WhiteQueens;
        private UInt64 WhiteKing;
        private UInt64 WhitePieces;

        private UInt64 BlackPawns;
        private UInt64 BlackKnights;
        private UInt64 BlackBishops;
        private UInt64 BlackRooks;
        private UInt64 BlackQueens;
        private UInt64 BlackKing;
        private UInt64 BlackPieces;

        private UInt64 Occupied => WhitePieces | BlackPieces;
        //by side

        


        //private UInt64 WhitePawnsAttacks => ((WhitePawns & ~FileABB) << 7) | ((WhitePawns & ~FileHBB) << 9);




        public void MakeMove(int move)
        {
            int fromSquare, toSquare;


        }
        public void UnMakeMove(int move) { }


        int bitboard_popcount(UInt64 b)
        {
            int cnt = 0;
            while (b != 0)
            {
                b &= (b - 1);
                cnt++;
            }
            return cnt;
        }




    }
    
}
