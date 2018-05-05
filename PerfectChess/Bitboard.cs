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
