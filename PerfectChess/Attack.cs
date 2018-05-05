using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Color;

namespace PerfectChess
{
    static class Attack
    {
        public static UInt64 Knight(int Square)
        {
            return KnightAttack[Square];
        }
        public static UInt64 King(int Square)
        {
            return KingAttack[Square];
        }
        public static UInt64 Pawn(int Color, int Square)
        {
            return PawnAttack[Color][Square];
        }

        static Attack()
        {
            for (int square = 0; square < 64; square++)
            {
                int File = Position.File(square);
                int Rank = Position.Rank(square);

                //Knight
                for (int x = -2; x <= 2; x++)
                    for (int y = -2; y <= 2; y++)
                        if (Math.Abs(x) + Math.Abs(y) == 3)
                            KnightAttack[square] ^= TryGetBitboard(File + x, Rank + y);

                //King
                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        if ((x | y) != 0)
                            KingAttack[square] ^= TryGetBitboard(File + x, Rank + y);

                //Pawns
                PawnAttack[White][square] ^= TryGetBitboard(File - 1, Rank + 1);
                PawnAttack[White][square] ^= TryGetBitboard(File + 1, Rank + 1);
                PawnAttack[Black][square] ^= TryGetBitboard(File - 1, Rank - 1);
                PawnAttack[Black][square] ^= TryGetBitboard(File + 1, Rank - 1);
            }
        }
        //Precomputed
        static UInt64[] KnightAttack = new UInt64[64];
        static UInt64[] KingAttack = new UInt64[64];
        static UInt64[][] PawnAttack = { new UInt64[64], new UInt64[64] };

        public static UInt64 TryGetBitboard(int x, int y)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7) return 0;
            return 1UL << (x + 8 * y);
        }


        //Not precomputed
        public static UInt64 Rook(int Square, UInt64 occupiedBitboard)
        {
            UInt64 blockersBitboard, partAttackBitboard, attackBitboard;

            attackBitboard = Bitboard.RayN[Square];
            blockersBitboard = attackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanForward(blockersBitboard);
                attackBitboard ^= Bitboard.RayN[blockingSquare];
            }

            partAttackBitboard = Bitboard.RayE[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanForward(blockersBitboard);
                partAttackBitboard ^= Bitboard.RayE[blockingSquare];
            }
            attackBitboard |= partAttackBitboard;

            partAttackBitboard = Bitboard.RayS[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanReverse(blockersBitboard);
                partAttackBitboard ^= Bitboard.RayS[blockingSquare];
            }
            attackBitboard |= partAttackBitboard;

            partAttackBitboard = Bitboard.RayW[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanReverse(blockersBitboard);
                partAttackBitboard ^= Bitboard.RayW[blockingSquare];
            }       

            return attackBitboard | partAttackBitboard;
        }
        public static UInt64 Bishop(int Square, UInt64 occupiedBitboard)
        {
            UInt64 blockersBitboard, partAttackBitboard, attackBitboard;

            attackBitboard = Bitboard.RayNE[Square];
            blockersBitboard = attackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanForward(blockersBitboard);
                attackBitboard ^= Bitboard.RayNE[blockingSquare];
            }

            partAttackBitboard = Bitboard.RaySE[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanReverse(blockersBitboard);
                partAttackBitboard ^= Bitboard.RaySE[blockingSquare];
            }
            attackBitboard |= partAttackBitboard;

            partAttackBitboard = Bitboard.RaySW[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanReverse(blockersBitboard);
                partAttackBitboard ^= Bitboard.RaySW[blockingSquare];
            }
            attackBitboard |= partAttackBitboard;

            partAttackBitboard = Bitboard.RayNW[Square];
            blockersBitboard = partAttackBitboard & occupiedBitboard;
            if (blockersBitboard != 0)
            {
                int blockingSquare = BitOperations.BitScanForward(blockersBitboard);
                partAttackBitboard ^= Bitboard.RayNW[blockingSquare];
            }

            return attackBitboard | partAttackBitboard;
        }
    }
}
