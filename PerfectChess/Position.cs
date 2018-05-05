using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    public class Position
    {

        /// <summary>
        /// Возвращает битбоард по коду фигуры (все расположения этой фигуры), максимум 14 = 8(color)+ 6(piece);
        /// </summary>
        private UInt64[] PieceBitboard = new UInt64[15];
        private UInt64 WhitePawns => PieceBitboard[1];
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

        


        private UInt64 WhitePawnsAttacks => ((WhitePawns & ~FileABB) >> 7) | ((WhitePawns & ~FileHBB) >> 9);




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



        const UInt64 FileABB = 0b10000000100000001000000010000000100000001000000010000000;
        const UInt64 FileHBB = 0b00000001000000010000000100000001000000010000000100000001;
    }
    static class PreComputedTables
    {
        static PreComputedTables()
        {

        }

        //Sliding pieces rays from square to different directions
        static UInt64[] RayN;
        static UInt64[] RayE;
        static UInt64[] RayS;
        static UInt64[] RayW;

        static UInt64[] RayNE;
        static UInt64[] RaySE;
        static UInt64[] RaySW;
        static UInt64[] RayNW;


        static UInt64[] eligibleKnightMoves;
        static UInt64[] eligibleKingMoves;





        static UInt64 elegibleMovesGenerator()
        {
            return 0;
        }




        const UInt64 DeBruijn_64 = 0x37E84A99DAE458F;
        static readonly int[] MagicTable =
            {
                0, 1, 17, 2, 18, 50, 3, 57,
                47, 19, 22, 51, 29, 4, 33, 58,
                15, 48, 20, 27, 25, 23, 52, 41,
                54, 30, 38, 5, 43, 34, 59, 8,
                63, 16, 49, 56, 46, 21, 28, 32,
                14, 26, 24, 40, 53, 37, 42, 7,
                62, 55, 45, 31, 13, 39, 36, 6,
                61, 44, 12, 35, 60, 11, 10, 9,
            };
        public static int BitScanForward(UInt64 b)
        {
            //Debug.Assert(b > 0, "Target number should not be zero");
            return MagicTable[((ulong)((long)b & -(long)b) * DeBruijn_64) >> 58];
        }
        public static int BitScanReverse(UInt64 b)
        {
            b |= b >> 1;
            b |= b >> 2;
            b |= b >> 4;
            b |= b >> 8;
            b |= b >> 16;
            b |= b >> 32;
            b = b & ~(b >> 1);
            return MagicTable[b * DeBruijn_64 >> 58];
        }
    }
}
