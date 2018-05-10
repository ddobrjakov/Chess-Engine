using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    static class Bitboard
    {
        static Bitboard()
        {
            for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                RayN[squareIndex] = GetRayBitboard(squareIndex, 0, 1) ^ (1UL << squareIndex);
                RayE[squareIndex] = GetRayBitboard(squareIndex, 1, 0) ^ (1UL << squareIndex);
                RayS[squareIndex] = GetRayBitboard(squareIndex, 0, -1) ^ (1UL << squareIndex);
                RayW[squareIndex] = GetRayBitboard(squareIndex, -1, 0) ^ (1UL << squareIndex);

                RayNE[squareIndex] = GetRayBitboard(squareIndex, 1, 1) ^ (1UL << squareIndex);
                RaySE[squareIndex] = GetRayBitboard(squareIndex, 1, -1) ^ (1UL << squareIndex);
                RaySW[squareIndex] = GetRayBitboard(squareIndex, -1, -1) ^ (1UL << squareIndex);
                RayNW[squareIndex] = GetRayBitboard(squareIndex, -1, 1) ^ (1UL << squareIndex);

                Axes[squareIndex] = RayN[squareIndex] | RayE[squareIndex] | RayS[squareIndex] | RayW[squareIndex];
                Diagonals[squareIndex] = RayNE[squareIndex] | RaySE[squareIndex] | RaySW[squareIndex] | RayNW[squareIndex];
                Square[squareIndex] = 1UL << squareIndex;
            }
        }
        public static readonly UInt64[] RayN = new UInt64[64];
        public static readonly UInt64[] RayE = new UInt64[64];
        public static readonly UInt64[] RayS = new UInt64[64];
        public static readonly UInt64[] RayW = new UInt64[64];

        public static readonly UInt64[] RayNE = new UInt64[64];
        public static readonly UInt64[] RaySE = new UInt64[64];
        public static readonly UInt64[] RaySW = new UInt64[64];
        public static readonly UInt64[] RayNW = new UInt64[64];

        private static UInt64 GetRayBitboard(int square, int dx, int dy)
        {
            if (square > 63 || square < 0) return 0;
            UInt64 bitboard = 1UL << square;
            if (Math.Floor(square / 8F) == Math.Floor((square + dx) / 8F))
                bitboard |= GetRayBitboard(square + dx + 8 * dy, dx, dy);
            return bitboard;
        }

        public static readonly UInt64[] Diagonals = new UInt64[64];
        public static readonly UInt64[] Axes = new UInt64[64];
        public static readonly UInt64[] Square = new UInt64[64];

        public const UInt64 FileABB = 0b00000001000000010000000100000001000000010000000100000001;
        public const UInt64 FileBBB = 0b00000010000000100000001000000010000000100000001000000010;
        public const UInt64 FileCBB = 0b00000100000001000000010000000100000001000000010000000100;
        public const UInt64 FileDBB = 0b00001000000010000000100000001000000010000000100000001000;
        public const UInt64 FileEBB = 0b00010000000100000001000000010000000100000001000000010000;
        public const UInt64 FileFBB = 0b00100000001000000010000000100000001000000010000000100000;
        public const UInt64 FileGBB = 0b01000000010000000100000001000000010000000100000001000000;
        public const UInt64 FileHBB = 0b10000000100000001000000010000000100000001000000010000000;

        public const UInt64 Rank1BB = 0xFF;
        public const UInt64 Rank2BB = 0xFF00;
        public const UInt64 Rank3BB = 0xFF0000;
        public const UInt64 Rank4BB = 0xFF000000;
        public const UInt64 Rank5BB = 0xFF00000000;
        public const UInt64 Rank6BB = 0xFF0000000000;
        public const UInt64 Rank7BB = 0xFF000000000000;
        public const UInt64 Rank8BB = 0xFF00000000000000;

        public const UInt64 InitialPawnPositionsBB = Rank2BB | Rank7BB;


        //static UInt64[] Diagonals;
        //static UInt64[] OneSquare;


        public static string ToString(UInt64 bitboard)
        {
            string Res = string.Empty;

            UInt64 bits8;
            for (int j = 0; j < 8; j++)
            {
                bits8 = (bitboard & 0xFF00000000000000) >> 56;
                for (int i = 0; i < 8; i++)
                {
                    Res += bits8 & 1;
                    bits8 >>= 1;
                }
                Res += "\n";
                bitboard <<= 8;
            }
        
            return Res;
        }
    }
}
