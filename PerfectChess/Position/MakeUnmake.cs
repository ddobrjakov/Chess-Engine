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
            MoveHistory.Push(Move);

            int fromSquare = PerfectChess.Move.FromSquare(Move);
            int toSquare = PerfectChess.Move.ToSquare(Move);
            int fromPiece = PerfectChess.Move.FromPiece(Move);
            int toPiece = PerfectChess.Move.ToPiece(Move);

            //Fifty move rule
            if (toPiece != 0 || (fromPiece & Piece.Mask) == Piece.Pawn)
            {
                MovesFiftyHistory.Push(0);
            }
            else
            {
                MovesFiftyHistory.Push(MovesFiftyRuleCount + 1);
            }

            //Moving
            SquarePiece[toSquare] = SquarePiece[fromSquare];
            SquarePiece[fromSquare] = 0;

            OccupiedBB ^= 1UL << fromSquare;
            OccupiedBB |= 1UL << toSquare;

            PieceBitboard[fromPiece] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece] ^= 1UL << toSquare;

            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << fromSquare;
            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << toSquare;

            if (CastleShortIndex[ColorToMove] <= 0) CastleShortIndex[ColorToMove]--;
            if (CastleLongIndex[ColorToMove] <= 0) CastleLongIndex[ColorToMove]--;
            if (CastleShortIndex[1 - ColorToMove] <= 0) CastleShortIndex[1 - ColorToMove]--;
            if (CastleLongIndex[1 - ColorToMove] <= 0) CastleLongIndex[1 - ColorToMove]--;

            //Resetting castling rights if king or rook moves         
            if ((fromPiece & Piece.Mask) == Piece.King)
            {
                //CanCastleLong[ColorToMove] = false;
                //CanCastleShort[ColorToMove] = false;

                if (CastleShortIndex[ColorToMove] == 1) CastleShortIndex[ColorToMove]--;
                if (CastleLongIndex[ColorToMove] == 1) CastleLongIndex[ColorToMove]--;
            }
            else if ((fromPiece & Piece.Mask) == Piece.Rook)
            {
                if (fromSquare == 56 * ColorToMove + 7)
                {
                    //CanCastleShort[ColorToMove] = false;
                    if (CastleShortIndex[ColorToMove] == 1) CastleShortIndex[ColorToMove]--;
                }
                else if (fromSquare == 56 * ColorToMove)
                {
                    //CanCastleLong[ColorToMove] = false;
                    if (CastleLongIndex[ColorToMove] == 1) CastleLongIndex[ColorToMove]--;
                }
            }
            //Setting enpassant square if double pawn push
            else if ((fromPiece & Piece.Mask) == Piece.Pawn && Rank(fromSquare) == 1 + 5 * ColorToMove && Math.Abs(toSquare - fromSquare) == 16)
            {               
                //EnPassantSquare = fromSquare + 8 - 16 * ColorToMove;
                EnPassantHistory.Push(fromSquare + 8 - 16 * ColorToMove);

                ColorToMove = 1 - ColorToMove;
                return;
            }

            //Capturing
            if (toPiece != 0)
            {
                PieceBitboard[toPiece] ^= 1UL << toSquare;
                PieceBitboard[toPiece & Color.Mask] ^= 1UL << toSquare;

                int EnemyLeftRookIndex = 56 * (1 - ColorToMove);
                if (toSquare == EnemyLeftRookIndex + 7 && (toPiece & Piece.Mask) == Piece.Rook && CastleShortIndex[1-ColorToMove] == 1) CastleShortIndex[1-ColorToMove]--;
                else if (toSquare == EnemyLeftRookIndex &&
                    (toPiece & Piece.Mask) == Piece.Rook && CastleLongIndex[1-ColorToMove] == 1)
                    CastleLongIndex[1-ColorToMove]--;
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

            //EnPassantSquare = InvalidSquare;
            EnPassantHistory.Push(InvalidSquare);
        }

        public void UnMake()
        {
            if (!MoveHistory.Any()) throw new Exception("No move to unmake");
            int MoveToUnmake = MoveHistory.Pop();
            int ColorMadeMove = 1 - ColorToMove;

            int fromSquare = Move.FromSquare(MoveToUnmake);
            int toSquare = Move.ToSquare(MoveToUnmake);
            int fromPiece = Move.FromPiece(MoveToUnmake);
            int toPiece = Move.ToPiece(MoveToUnmake);

            MovesFiftyHistory.Pop();


            //Moving Back And Cancelling Capturing
            SquarePiece[toSquare] = toPiece;
            SquarePiece[fromSquare] = fromPiece;

            PieceBitboard[fromPiece] ^= 1UL << fromSquare; //no changes
            PieceBitboard[fromPiece] ^= 1UL << toSquare; //no changes

            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << fromSquare; //no changes
            PieceBitboard[fromPiece & Color.Mask] ^= 1UL << toSquare; //no changes

            OccupiedBB ^= 1UL << fromSquare; //no changes
            if (toPiece != 0)
            {
                PieceBitboard[toPiece] ^= 1UL << toSquare;
                PieceBitboard[toPiece & Color.Mask] ^= 1UL << toSquare;

                int EnemyLeftRookIndex = 56 * ColorMadeMove;
                if (toSquare == EnemyLeftRookIndex && toPiece == Piece.Rook && CastleShortIndex[1 - ColorMadeMove] <= 0) CastleShortIndex[1 - ColorMadeMove]++;
                else if (toSquare == EnemyLeftRookIndex + 7 && toPiece == Piece.Rook && CastleLongIndex[1 - ColorMadeMove] <= 0) CastleShortIndex[1 - ColorMadeMove]++;
            }
            else
            {
                OccupiedBB ^= 1UL << toSquare;
            }

            if (CastleShortIndex[ColorMadeMove] <= 0) CastleShortIndex[ColorMadeMove]++;
            if (CastleLongIndex[ColorMadeMove] <= 0) CastleLongIndex[ColorMadeMove]++;
            if (CastleShortIndex[1 - ColorMadeMove] <= 0) CastleShortIndex[1 - ColorMadeMove]++;
            if (CastleLongIndex[1 - ColorMadeMove] <= 0) CastleLongIndex[1 - ColorMadeMove]++;

            //Castling Back
            if (Move.Castling(MoveToUnmake))
            {
                int sidePreIndex = 56 * ColorMadeMove;

                int rookSquare, rookToSquare;
                //Short castling (getting rook position)
                if (File(toSquare) == 6)
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

                //Moving the rook back
                SquarePiece[rookSquare] = SquarePiece[rookToSquare];
                SquarePiece[rookToSquare] = 0;

                OccupiedBB ^= 1UL << rookSquare;
                OccupiedBB ^= 1UL << rookToSquare;

                PieceBitboard[Piece.Rook | ColorMadeMove] ^= 1UL << rookSquare;
                PieceBitboard[Piece.Rook | ColorMadeMove] ^= 1UL << rookToSquare;

                PieceBitboard[ColorMadeMove] ^= 1UL << rookSquare;
                PieceBitboard[ColorMadeMove] ^= 1UL << rookToSquare;
            }

            //Promotion
            if (Move.Promotion(MoveToUnmake))
            {
                int PromotedTo = Move.PromotionPiece(MoveToUnmake); //Either queen, knight, bishop or rook
                //SquarePiece[toSquare] = PromotedTo;

                //If we are here, we've just set the PieceBitboard[fromPiece] to 1 at the toSquare thinking of doing the opposite, so we need to cancel that
                PieceBitboard[fromPiece] ^= 1UL << toSquare;
                PieceBitboard[PromotedTo] ^= 1UL << toSquare;

                //Color maps do not change, as well as occupied (we're just replacing the piece)
            }

            //Enpassant
            if (Move.EnPassant(MoveToUnmake))
            {
                //Restoring the pawn
                int EnPassantVictimSquare = toSquare + 16 * ColorMadeMove - 8;
                UInt64 enPassantVictimBitboard = 1UL << EnPassantVictimSquare;

                OccupiedBB ^= enPassantVictimBitboard;
                PieceBitboard[Piece.Pawn | ColorToMove] ^= enPassantVictimBitboard;
                PieceBitboard[ColorToMove] ^= enPassantVictimBitboard;
                SquarePiece[EnPassantVictimSquare] = Piece.Pawn | ColorToMove;
            }

            ColorToMove = ColorMadeMove;
            EnPassantHistory.Pop();
        }
    }
}
