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
                RayN[squareIndex] = GetRayBitboard(squareIndex, 0, 1);
                RayE[squareIndex] = GetRayBitboard(squareIndex, 1, 0);
                RayS[squareIndex] = GetRayBitboard(squareIndex, 0, -1);
                RayW[squareIndex] = GetRayBitboard(squareIndex, -1, 0);

                RayNE[squareIndex] = GetRayBitboard(squareIndex, 1, 1);
                RaySE[squareIndex] = GetRayBitboard(squareIndex, 1, -1);
                RaySW[squareIndex] = GetRayBitboard(squareIndex, -1, -1);
                RayNW[squareIndex] = GetRayBitboard(squareIndex, -1, 1);
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
            if (square > 63 | square < 0) return 0;
            UInt64 bitboard = 1UL << square;
            if (square / 8 == (square + dx) / 8)
                bitboard |= GetRayBitboard(square + dx + 8 * dy, dx, dy);
            return bitboard;
        }



        const UInt64 FileABB = 0b00000001000000010000000100000001000000010000000100000001;
        const UInt64 FileHBB = 0b10000000100000001000000010000000100000001000000010000000;

        static UInt64[] Diagonals;
        static UInt64[] OneSquare;


        public static string ToString(UInt64 bitboard)
        {
            string Res = string.Empty;

            for (int i = 0; i < 64; i++)
            {
                Res += bitboard & 1;
                bitboard >>= 1;
                if (i % 8 == 7) Res += "\n";
            }
            return Res;
        }
    }
}
