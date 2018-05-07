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

            //Moving
            SquarePiece[toSquare] = SquarePiece[fromSquare];
            SquarePiece[fromSquare] = 0;

            OccupiedBB ^= 1UL << fromSquare;
            OccupiedBB |= 1UL << toSquare;

            PieceBitboard[fromPiece] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece] ^= 1UL << toSquare;

            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << toSquare;

            //Resetting castling rights if king or rook moves         
            if ((fromPiece & Piece.Mask) == Piece.King)
            {
                CanCastleLong[ColorToMove] = false;
                CanCastleShort[ColorToMove] = false;
            }
            else if ((fromPiece & Piece.Mask) == Piece.Rook)
            {
                if (File(fromSquare) == 7) CanCastleShort[ColorToMove] = false;
                else CanCastleLong[ColorToMove] = false;
            }
            //Setting enpassant square if double pawn push
            else if ((fromPiece & Piece.Mask) == Piece.Pawn && Rank(fromSquare) == 1 + 5 * ColorToMove && Math.Abs(toSquare - fromSquare) == 16)
            {
                EnPassantSquare = fromSquare + 8 - 16 * ColorToMove;
                ColorToMove = 1 - ColorToMove;
                return;
            }

            //Capturing
            if (toPiece != 0)
            {
                PieceBitboard[toPiece] ^= 1UL << toSquare;
                PieceBitboard[toPiece & Color.Mask] ^= 1UL << toSquare;
            }

            //Castling
            if (PerfectChess.Move.Castling(Move))
            {
                int sidePreIndex = 56 * ColorToMove;

                int rookSquare, rookToSquare;
                //Short castling (getting rook position)
                if (Position.File(toSquare) == 6)
                {
                    rookSquare = sidePreIndex + 7;
                    rookToSquare = fromSquare + 1;
                }
                //Long castling (getting rook position)
                else
                {
                    rookSquare = sidePreIndex;
                    rookToSquare = fromSquare - 1;
                }

                //Moving the rook
                SquarePiece[rookToSquare] = SquarePiece[rookSquare];
                SquarePiece[rookSquare] = 0;

                OccupiedBB ^= 1UL << rookSquare;
                OccupiedBB |= 1UL << rookToSquare;

                PieceBitboard[Piece.Rook | ColorToMove] ^= 1UL << rookSquare;
                PieceBitboard[Piece.Rook | ColorToMove] ^= 1UL << rookToSquare;

                PieceBitboard[ColorToMove] ^= 1UL << rookSquare; //Assuming there is a rook of our colour :)
                PieceBitboard[ColorToMove] ^= 1UL << rookToSquare;
            }

            //Promotion
            if (PerfectChess.Move.Promotion(Move))
            {
                int PromotedTo = PerfectChess.Move.PromotionPiece(Move); //Either queen, knight, bishop or rook
                SquarePiece[toSquare] = PromotedTo;

                PieceBitboard[fromPiece] ^= 1UL << toSquare; //Our pawn is not gonna exist anymore, so delete it from table of pawns
                PieceBitboard[PromotedTo] ^= 1UL << toSquare; //There is no piece on that square, so we put our PromotedTo piece there

                //Color maps do not change, as well as occupied (we're just replacing the piece)
            }

            //Enpassant
            if (PerfectChess.Move.EnPassant(Move))
            {
                //Deleting the pawn
                int EnPassantVictimSquare = (EnPassantSquare + 16 * ColorToMove - 8);
                UInt64 enPassantVictimBitboard = 1UL << EnPassantVictimSquare;

                OccupiedBB ^= enPassantVictimBitboard;
                PieceBitboard[Piece.Pawn | 1 - ColorToMove] ^= enPassantVictimBitboard;
                PieceBitboard[1 - ColorToMove] ^= enPassantVictimBitboard;
                SquarePiece[EnPassantVictimSquare] = 0;
            }
            ColorToMove = 1 - ColorToMove;
            EnPassantSquare = InvalidSquare;
        }
    }
}
