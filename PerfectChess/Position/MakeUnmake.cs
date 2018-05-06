using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    partial class Position
    {
        public void Make(int Move)
        {
            int fromSquare = PerfectChess.Move.FromSquare(Move);
            int toSquare = PerfectChess.Move.ToSquare(Move);
            int fromPiece = PerfectChess.Move.FromPiece(Move);
            int toPiece = PerfectChess.Move.ToPiece(Move);

            SquarePiece[toSquare] = SquarePiece[fromSquare];
            SquarePiece[fromSquare] = 0;

            OccupiedBB ^= 1UL << fromSquare;
            OccupiedBB |= 1UL << toSquare;

            PieceBitboard[fromPiece] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece] ^= 1UL << toSquare;

            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << toSquare;



            if (toPiece != 0)
            {
                PieceBitboard[toPiece] ^= 1UL << toSquare;
                PieceBitboard[toPiece & Color.Mask] ^= 1UL << toSquare;
            }
        }
    }
}
