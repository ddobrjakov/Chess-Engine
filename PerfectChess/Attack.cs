using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    static class Attack
    {
        static Attack()
        {

        }
        //Precomputed
        static UInt64[] KnightAttack = new UInt64[64];
        static UInt64[] KingAttack = new UInt64[64];
        static UInt64[][] PawnAttack = { new UInt64[64], new UInt64[64] };

        public static UInt64 TryGetBitboard(int x, int y)
        {
            return 1UL << (x + 8 * y);
        }


        //Not precomputed

    }
}
