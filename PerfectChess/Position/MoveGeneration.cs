using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Move;
using static PerfectChess.Piece;

namespace PerfectChess
{
    partial class Position
    {
        public List<int> LegalMoves()
        {
            bool Check = IsInCheck(ColorToMove);


            List<int> moves = new List<int>();
            if (!Check) GenerateCastleMoves(moves);
            GenerateEnPassantMoves(moves);

            //var F = System.IO.File.Create(@"C:\programming\moves.txt");
            //List<int> Mo = MoveHistory.ToList();
            //Mo.Reverse();
            //string text = String.Empty;
            //for (int i = 0; i < Mo.Count(); i++)
            //    text += Move.Details(Mo[i]) + "\n";
            //System.IO.StreamWriter textFile = new System.IO.StreamWriter(@"C:\programming\moves.txt");
            //textFile.WriteLine(text);
            //textFile.Close();

            //Captures
            //GeneratePawnCaptures(moves);
            //GenerateKnightCaptures(moves);
            //GenerateBishopCaptures(moves);
            //GenerateRookCaptures(moves);
            //GenerateQueenCaptures(moves);
            //GenerateKingCaptures(moves);

            UInt64 PinnedPieces = (Check) ? 0 : GetPinnedPieces();
            //Silent Moves
            GeneratePawnMoves(moves, PinnedPieces, Check);
            GenerateKnightMoves(moves);
            GenerateBishopMoves(moves, PinnedPieces, Check);
            GenerateRookMoves(moves, PinnedPieces, Check);
            GenerateQueenMoves(moves, PinnedPieces, Check);
            GenerateKingMoves(moves);

            return moves;
        }
        private void GenerateCastleMoves(List<int> moves)
        {
            //if (!IsInCheck(ColorToMove))
            //{
                int preIndex = 56 * ColorToMove;

                if (CastleLongIndex[ColorToMove] == 1/*CanCastleLong[ColorToMove]*/ && (SquarePiece[1 + preIndex] | SquarePiece[2 + preIndex] | SquarePiece[3 + preIndex]) == None)
                    if (!IsAttacked(ColorToMove, 3 + preIndex) && !IsAttacked(ColorToMove, 2 + preIndex))
                        moves.Add(Move.Create(4 + preIndex, 2 + preIndex, ColorToMove | King, None, SpecCastling));

                if (CastleShortIndex[ColorToMove] == 1/*CanCastleShort[ColorToMove]*/ && (SquarePiece[5 + preIndex] | SquarePiece[6 + preIndex]) == None)
                    if (!IsAttacked(ColorToMove, 5 + preIndex) && !IsAttacked(ColorToMove, 6 + preIndex))
                        moves.Add(Move.Create(4 + preIndex, 6 + preIndex, ColorToMove | King, None, SpecCastling));
            //}
        }
        private void GenerateEnPassantMoves(List<int> moves)
        {
            if (EnPassantSquare != InvalidSquare)
            {
                int Enemy = 1 - ColorToMove;
                int kingSquare = BitOperations.OnlyBitIndex(PieceBitboard[ColorToMove | King]);

                // Enpassant Square is on the 3rd/6th rank
                UInt64 enPassantPawnBitboard = PieceBitboard[ColorToMove | Piece.Pawn] & Attack.Pawn(Enemy, EnPassantSquare);
                UInt64 enPassantVictimBitboard = 1UL << (EnPassantSquare + 8 - 16 * Enemy); //+ 8 - 16 * enemy
                while (enPassantPawnBitboard != 0)
                {
                    // Performing necessary part of the move to determine it's legality
                    int FromSquare = BitOperations.PopLS(ref enPassantPawnBitboard);
                    PieceBitboard[Enemy | Pawn] ^= enPassantVictimBitboard;
                    OccupiedBB ^= enPassantVictimBitboard;
                    OccupiedBB ^= (1UL << FromSquare) | (1UL << EnPassantSquare);

                    // Check for legality and add move. 
                    if (!IsAttacked(ColorToMove, kingSquare))
                        moves.Add(Move.Create(FromSquare, EnPassantSquare, ColorToMove | Pawn, None, SpecEnPassant));

                    // Reverting changes
                    PieceBitboard[Enemy | Pawn] ^= enPassantVictimBitboard;
                    OccupiedBB ^= enPassantVictimBitboard;
                    OccupiedBB ^= (1UL << FromSquare) | (1UL << EnPassantSquare);
                }
            }
        }

        private bool IsLegal(int SquareFrom, int SquareTo)
        {
            // 1) Performing necessary part of the move to determine it's legality

            //Saving Occupied Pieces Bitboard to restore it in near future
            UInt64 occupiedBBSaved = OccupiedBB;

            //Getting the captured piece (might be easily Piece.None = 0)
            Int32 capture = SquarePiece[SquareTo];

            //Setting up properties to prepare for the check test
                                    
            PieceBitboard[capture] ^= 1UL << SquareTo; //Deletes piece from it's table if it's there (if not, nothing bad happens, 
            //because we don't care about PieceBitBoard[0] and moreover we'll put the value back anyway

            OccupiedBB ^= 1UL << SquareFrom; //origin square now frees up
            OccupiedBB |= 1UL << SquareTo; //destinatinion square gets our piece


            // 2) Now everything is ready to test if the king is checked;
            int kingSquare = BitOperations.OnlyBitIndex(PieceBitboard[ColorToMove | King]);
            bool res = !IsAttacked(ColorToMove, kingSquare);

            // 3) We've got the answer, but have to return everything back

            PieceBitboard[capture] ^= (1UL << SquareTo); //XOR is commutative, so that one is pretty easy
            OccupiedBB = occupiedBBSaved; //thanks to us in the past saving it, now we can simply restore the occupied bitboard

            return res;
        }


        /*
        /// <summary>
        /// Generates all legal capture moves for given type of piece in current position
        /// </summary>
        /// <param name="Piece"></param>
        /// <param name="moves"></param>
        private void GeneratePieceCaptures(int Piece, List<int> moves)
        {
            /*int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Piece];
            UInt64 enemyALLpieces = PieceBitboard[1 - ourColor];

            UInt64 Captures;
            while (ourPieces != 0)
            {
                //Достаем индекс атакующей фигуры
                int pieceSquare = BitOperations.PopLS(ref ourPieces);

                //
                UInt64 attackBB = Attack.Pawn(We, PawnSquare);
                capturesBB |= enemyBB & attackBB;
            }*/
        /*}
    *

        private void GeneratePawnCaptures(List<int> moves)
        {
            /*int We = ColorToMove;
            int Them = 1 - We;

            UInt64 ourPawns = PieceBitboard[We | Pawn];
            UInt64 enemyBB = PieceBitboard[Them];

            UInt64 capturesBB = 0;
            while (ourPawns != 0)
            {
                int PawnSquare = BitOperations.PopLS(ref ourPawns);
                UInt64 attackBB = Attack.Pawn(We, PawnSquare);
                capturesBB |= enemyBB & attackBB;

                while (capturesBB != 0)
                {
                    int ToSquare = BitOperations.PopLS(ref capturesBB);
                    int CapturedPiece = SquarePiece[ToSquare];
                    if (IsLegal(PawnSquare, ToSquare))
                    {
                        if (((1UL << ToSquare) & (Bitboard.Rank1BB | Bitboard.Rank8BB)) != 0)
                        {
                            moves.Add(Move.Create(PawnSquare, ToSquare, We | Pawn, CapturedPiece, SpecPromotion, PromotionToQueen));
                            moves.Add(Move.Create(PawnSquare, ToSquare, We | Pawn, CapturedPiece, SpecPromotion, PromotionToKnight));
                            moves.Add(Move.Create(PawnSquare, ToSquare, We | Pawn, CapturedPiece, SpecPromotion, PromotionToRook));
                            moves.Add(Move.Create(PawnSquare, ToSquare, We | Pawn, CapturedPiece, SpecPromotion, PromotionToBishop));
                        }
                        moves.Add(Move.Create(PawnSquare, ToSquare, We | Pawn, CapturedPiece));
                    }
                }
                
            }*/        
        /*}
        private void GenerateKnightCaptures(List<int> moves)
        {

        }
        private void GenerateBishopCaptures(List<int> moves)
        {

        }
        private void GenerateRookCaptures(List<int> moves)
        {

        }
        private void GenerateQueenCaptures(List<int> moves)
        {

        }
        private void GenerateKingCaptures(List<int> moves)
        {

        }
        */


        private void GeneratePawnMoves(List<int> moves, UInt64 PinnedPieces, bool Check)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Pawn];
            UInt64 enemyALLpieces = PieceBitboard[1 - ourColor];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int FromSquare = BitOperations.PopLS(ref ourPieces);

                //Checking for pin
                bool Pinned = ((PinnedPieces & (1UL << FromSquare)) != 0) ? true : false; 


                //One square advance
                int ToSquare = FromSquare + 8 - 16 * ColorToMove;
                UInt64 moveBitboard = ~OccupiedBB & (1UL << ToSquare); //0 if destination square is occupied

                //Two square advance
                if (moveBitboard != 0 && (Rank(FromSquare) == 1 + 5 * ColorToMove))
                    moveBitboard |= ~OccupiedBB & (1UL << (FromSquare + 16 - 32 * ColorToMove)); //0 => doesn't change if destination square's occupied

                //Captures
                UInt64 attackBitboard = Attack.Pawn(ColorToMove, FromSquare);
                moveBitboard |= enemyALLpieces & attackBitboard;

                //Now all the pseudo-legal moves are stored in moveBitboard and we're ready to check and add them
                while (moveBitboard != 0)
                {
                    //Iterating throw each possible destination square
                    ToSquare = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[ToSquare];

                    bool Legal = (Check) ? IsLegal(FromSquare, ToSquare) : (!Pinned || Bitboard.Aligned(KingSquare(ColorToMove), FromSquare, ToSquare));
                    if (Legal)//(IsLegal(FromSquare, ToSquare))
                    {
                        if (((1UL << ToSquare) & (Bitboard.Rank1BB | Bitboard.Rank8BB)) != 0) //Check for promotion
                        {
                            moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Pawn, CapturedPiece, SpecPromotion, PromotionToQueen));
                            moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Pawn, CapturedPiece, SpecPromotion, PromotionToKnight));
                            moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Pawn, CapturedPiece, SpecPromotion, PromotionToRook));
                            moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Pawn, CapturedPiece, SpecPromotion, PromotionToBishop));
                        }
                        else moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Pawn, CapturedPiece));
                    }
                }
            }
        }
        /*private void GeneratePawnMoves(List<int> moves)
        {
            int Enemy = 1 - ColorToMove;
            UInt64 pieceBitboard = PieceBitboard[ColorToMove | Pawn];
            UInt64 enemyBitboard = PieceBitboard[1 - ColorToMove];

            //Getting pawns one by one
            while (pieceBitboard != 0)
            {
                // Consider single square advance.
                Int32 From = BitOperations.PopLS(ref pieceBitboard);
                Int32 To = From + 16 * ColorToMove - 8;
                UInt64 moveBitboard = ~OccupiedBB & (1UL << To);

                //Consider two squares advance
                if (moveBitboard != 0 && ((1UL << From) & Bitboard.InitialPawnPositionsBB) != 0)
                    moveBitboard |= ~OccupiedBB & (1UL << (From + 32 * ColorToMove - 16));

                // Consider captures. 
                UInt64 attackBitboard = Attack.Pawn(From, ColorToMove);
                moveBitboard |= enemyBitboard & attackBitboard;

                while (pieceBitboard != 0)
                {
                    //Perform just the part of move necessary to determine if the check is given
                    To = BitOperations.PopLS(ref moveBitboard);
                    UInt64 occupiedBBSaved = OccupiedBB;
                    Int32 capture = SquarePiece[To];
                    PieceBitboard[capture] ^= 1UL << To; //ничего не должно поменять
                    OccupiedBB ^= 1UL << From;
                    OccupiedBB |= 1UL << To;


                    int kingSquare = BitOperations.OnlyBitIndex(PieceBitboard[ColorToMove | King]);
                    // Check for legality and add moves. 
                    if (!IsAttacked(ColorToMove, kingSquare))
                        if (((1UL << To) & (Bitboard.Rank1BB | Bitboard.Rank8BB)) != 0)
                        {
                            moves.Add(Move.Create(From, To, ColorToMove | Pawn, None, SpecPromotion, PromotionToQueen));
                            moves.Add(Move.Create(From, To, ColorToMove | Pawn, None, SpecPromotion, PromotionToKnight));
                            moves.Add(Move.Create(From, To, ColorToMove | Pawn, None, SpecPromotion, PromotionToRook));
                            moves.Add(Move.Create(From, To, ColorToMove | Pawn, None, SpecPromotion, PromotionToBishop));
                        }
                        else moves.Add(Move.Create(From, To, ColorToMove | Pawn, None));

                    // Revert state changes. 
                    PieceBitboard[capture] ^= 1UL << To; //ничего не должно поменять обратно
                    OccupiedBB = occupiedBBSaved;
                }
            }
        }*/
        private void GenerateKnightMoves(List<int> moves)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Knight];
            UInt64 targetBitboard = ~PieceBitboard[ColorToMove];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int FromSquare = BitOperations.PopLS(ref ourPieces);

                UInt64 moveBitboard = targetBitboard & Attack.Knight(FromSquare);
                while (moveBitboard != 0)
                {
                    // Perform minimal state changes to mimick real move and check for legality. 
                    Int32 ToSquare = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[ToSquare];

                    if (IsLegal(FromSquare, ToSquare))
                    {
                        moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Knight, CapturedPiece));
                    }
                }
            }
        }
        private void GenerateBishopMoves(List<int> moves, UInt64 PinnedPieces, bool Check)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Bishop];
            UInt64 targetBitboard = ~PieceBitboard[ColorToMove];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int FromSquare = BitOperations.PopLS(ref ourPieces);

                //Checking for pin
                bool Pinned = ((PinnedPieces & (1UL << FromSquare)) != 0) ? true : false;

                UInt64 moveBitboard = targetBitboard & Attack.Bishop(FromSquare, OccupiedBB);
                while (moveBitboard != 0)
                {
                    // Perform minimal state changes to mimick real move and check for legality. 
                    Int32 ToSquare = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[ToSquare];

                    bool Legal = (Check) ? IsLegal(FromSquare, ToSquare) : (!Pinned || Bitboard.Aligned(KingSquare(ColorToMove), FromSquare, ToSquare));
                    if (Legal)//(IsLegal(FromSquare, ToSquare))
                    {
                        moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Bishop, CapturedPiece));
                    }
                }
            }
        }
        private void GenerateRookMoves(List<int> moves, UInt64 PinnedPieces, bool Check)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Rook];
            UInt64 targetBitboard = ~PieceBitboard[ColorToMove];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int FromSquare = BitOperations.PopLS(ref ourPieces);

                //Checking for pin
                bool Pinned = ((PinnedPieces & (1UL << FromSquare)) != 0) ? true : false;

                UInt64 moveBitboard = targetBitboard & Attack.Rook(FromSquare, OccupiedBB);
                while (moveBitboard != 0)
                {
                    // Perform minimal state changes to mimick real move and check for legality. 
                    Int32 ToSquare = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[ToSquare];

                    bool Legal = (Check) ? IsLegal(FromSquare, ToSquare) : (!Pinned || Bitboard.Aligned(KingSquare(ColorToMove), FromSquare, ToSquare));
                    if (Legal)//(IsLegal(FromSquare, ToSquare))
                    {
                        moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Rook, CapturedPiece));
                    }
                }
            }
        }
        private void GenerateQueenMoves(List<int> moves, UInt64 PinnedPieces, bool Check)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | Queen];
            UInt64 targetBitboard = ~PieceBitboard[ColorToMove];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int FromSquare = BitOperations.PopLS(ref ourPieces);

                //Checking for pin
                bool Pinned = ((PinnedPieces & (1UL << FromSquare)) != 0) ? true : false;

                UInt64 moveBitboard = targetBitboard & Attack.Queen(FromSquare, OccupiedBB);
                while (moveBitboard != 0)
                {
                    // Perform minimal state changes to mimick real move and check for legality. 
                    Int32 ToSquare = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[ToSquare];

                    bool Legal = (Check) ? IsLegal(FromSquare, ToSquare) : (!Pinned || Bitboard.Aligned(KingSquare(ColorToMove), FromSquare, ToSquare));
                    if (Legal)//(IsLegal(FromSquare, ToSquare))
                    {
                        moves.Add(Move.Create(FromSquare, ToSquare, ColorToMove | Queen, CapturedPiece));
                    }
                }
            }
        }
        private void GenerateKingMoves(List<int> moves)
        {
            int ourColor = ColorToMove;
            UInt64 ourPieces = PieceBitboard[ourColor | King];
            UInt64 targetBitboard = ~PieceBitboard[ColorToMove];

            while (ourPieces != 0)
            {
                //Getting the moving piece square
                int SquareFrom = BitOperations.PopLS(ref ourPieces);

                UInt64 moveBitboard = targetBitboard & Attack.King(SquareFrom);
                while (moveBitboard != 0)
                {
                    // Perform minimal state changes to mimick real move and check for legality. 
                    Int32 SquareTo = BitOperations.PopLS(ref moveBitboard);
                    int CapturedPiece = SquarePiece[SquareTo];



                    UInt64 occupiedBBSaved = OccupiedBB;

                    //Getting the captured piece (might be easily Piece.None = 0)
                    Int32 capture = SquarePiece[SquareTo];

                    //Setting up properties to prepare for the check test

                    PieceBitboard[capture] ^= 1UL << SquareTo; //Deletes piece from it's table if it's there (if not, nothing bad happens, 
                                                               //because we don't care about PieceBitBoard[0] and moreover we'll put the value back anyway

                    OccupiedBB ^= 1UL << SquareFrom; //origin square now frees up
                    OccupiedBB |= 1UL << SquareTo; //destinatinion square gets our piece


                    // 2) Now everything is ready to test if the king is checked;
                    bool res = !IsAttacked(ColorToMove, SquareTo);

                    // 3) We've got the answer, but have to return everything back

                    PieceBitboard[capture] ^= (1UL << SquareTo); //XOR is commutative, so that one is pretty easy
                    OccupiedBB = occupiedBBSaved; //thanks to us in the past saving it, now we can simply restore the occupied bitboard

                    if (res)
                    {
                        moves.Add(Move.Create(SquareFrom, SquareTo, ColorToMove | King, CapturedPiece));
                    }
                }
            }
        }




        private UInt64 GetPinnedPieces()
        {
            UInt64 PinnedPieces = 0;


            int KingSq = KingSquare(ColorToMove);

            //Sliding pieces
            UInt64 blockingPieces = 0;
            blockingPieces = Attack.Bishop(KingSq, OccupiedBB) & PieceBitboard[ColorToMove];
            UInt64 XRayBishopAttacks = Attack.Bishop(KingSq, OccupiedBB ^ blockingPieces);
            UInt64 BishopAttackers = XRayBishopAttacks & (PieceBitboard[(1 - ColorToMove) | Bishop] | PieceBitboard[(1 - ColorToMove) | Queen]);
            while (BishopAttackers != 0)
            {
                int Attacker = BitOperations.PopLS(ref BishopAttackers);
                PinnedPieces ^= Bitboard.SquaresBetween[Attacker][KingSq] & blockingPieces;
            }

            blockingPieces = Attack.Rook(KingSq, OccupiedBB) & PieceBitboard[ColorToMove];
            UInt64 XRayRookAttacks = Attack.Rook(KingSq, OccupiedBB ^ blockingPieces);
            UInt64 RookAttackers = XRayRookAttacks & (PieceBitboard[(1 - ColorToMove) | Rook] | PieceBitboard[(1 - ColorToMove) | Queen]);
            while (RookAttackers != 0)
            {
                int Attacker = BitOperations.PopLS(ref RookAttackers);
                PinnedPieces ^= Bitboard.SquaresBetween[Attacker][KingSq] & blockingPieces;
            }


            //PinnedPieces ^= Attack.Bishop(KingSq, OccupiedBB ^ blockingPieces) & blockingPieces;



            //Bitboard.Axes[King];
            //OccupiedBB 


            return PinnedPieces;
        }

        //private UInt64 XRayBishopAttacks(int FromSquare)
        //{
        //    UInt64 blockingPieces = Attack.Bishop(Square, OccupiedBB) & PieceBitboard[ColorToMove];
        //    return Attack.Bishop(Square, OccupiedBB ^ blockingPieces) & blockingPieces;
        //}
    }
}
