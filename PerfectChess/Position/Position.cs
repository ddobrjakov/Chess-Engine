using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Piece;
using static PerfectChess.Color;

namespace PerfectChess
{
    public partial class Position
    {
        
        public Position() : this(InitialPieces, White, 1111)
        {
        }
        public Position(int[] Pieces, int ToMove, int CastlingRights, int EnPassant = InvalidSquare)
        {
            if (Pieces.Count() != 64) throw new ArgumentException();
            for (int i = 0; i < 64; i++)
            {
                int PieceToAdd = Pieces[i];
                SquarePiece[i] = PieceToAdd;

                if (PieceToAdd == 0) continue;
                PieceBitboard[PieceToAdd] |= (1UL << i); //массив для фигуры
                PieceBitboard[PieceToAdd & Color.Mask] |= (1UL << i); //массив для цвета
                OccupiedBB |= (1UL << i); //массив всех фигур

                //if ((PieceToAdd & Color.Mask) == White) WhitePieces |= (1UL << i);
                //else BlackPieces |= (1UL << i);
            }
            ColorToMove = ToMove;

            CanCastleShort[White] = (CastlingRights & 0b0001) != 0;
            CanCastleShort[Black] = (CastlingRights & 0b0010) != 0;
            CanCastleLong[White] = (CastlingRights & 0b0100) != 0;
            CanCastleLong[Black] = (CastlingRights & 0b1000) != 0;

            this.EnPassantSquare = EnPassant;
        }
        /// <summary>
        /// Returns a bitboard of all squares containing piece of given type 
        /// </summary>
        private UInt64[] PieceBitboard = new UInt64[15];

        /// <summary>
        /// Returns piece located on given square
        /// </summary>
        private int[] SquarePiece = new int[64];

        public int this[int index] => SquarePiece[index];

        /// <summary>
        /// Represents castling king side rights for each color
        /// </summary>
        private bool[] CanCastleShort = new bool[2];
        
        /// <summary>
        /// Represents castling queen side rights for each color
        /// </summary>
        private bool[] CanCastleLong = new bool[2];

        /// <summary>
        /// Stores the current side to move
        /// </summary>
        public int ColorToMove { get; private set; } = 0;

        /// <summary>
        /// Stores the square behind enpassant pawn (-1 if none)
        /// </summary>
        private int EnPassantSquare;

        private UInt64 WhitePawns => PieceBitboard[White | Pawn];
        private UInt64 WhiteKnights => PieceBitboard[White | Knight];
        private UInt64 WhiteBishops => PieceBitboard[White | Bishop];
        private UInt64 WhiteRooks => PieceBitboard[White | Rook];
        private UInt64 WhiteQueens => PieceBitboard[White | Queen];
        private UInt64 WhiteKing => PieceBitboard[White | King];
        private UInt64 WhitePiecesBB => PieceBitboard[White];

        private UInt64 BlackPawns => PieceBitboard[Black | Pawn];
        private UInt64 BlackKnights => PieceBitboard[Black | Knight];
        private UInt64 BlackBishops => PieceBitboard[Black | Bishop];
        private UInt64 BlackRooks => PieceBitboard[Black | Rook];
        private UInt64 BlackQueens => PieceBitboard[Black | Queen];
        private UInt64 BlackKing => PieceBitboard[Black | King];
        private UInt64 BlackPiecesBB => PieceBitboard[Black];

        private UInt64 OccupiedBB;// => WhitePiecesBB | BlackPiecesBB;
        //by side
        


        //private UInt64 WhitePawnsAttacks => ((WhitePawns & ~FileABB) << 7) | ((WhitePawns & ~FileHBB) << 9);



        public static int File(int square)
        {
            return square & 7;
        }
        public static int Rank(int square)
        {
            return square >> 3;
        }

        private const int InvalidSquare = -1;
        public static readonly int[] InitialPieces =
        {
            (White|Rook), (White|Knight), (White|Bishop), (White|Queen), (White|King), (White|Bishop), (White|Knight), (White|Rook),
            (White|Pawn), (White|Pawn),   (White|Pawn),   (White|Pawn),  (White|Pawn), (White|Pawn),   (White|Pawn),   (White|Pawn),
            None, None, None, None, None, None, None, None,
            None, None, None, None, None, None, None, None,
            None, None, None, None, None, None, None, None,
            None,          None,          None,           None,          None,         None,           None,           None,           
            (Black|Pawn), (Black|Pawn),   (Black|Pawn),   (Black|Pawn),  (Black|Pawn), (Black|Pawn),   (Black|Pawn),   (Black|Pawn),
            (Black|Rook), (Black|Knight), (Black|Bishop), (Black|Queen), (Black|King), (Black|Bishop), (Black|Knight), (Black|Rook)
        };




        public bool IsAttacked(int Color, int Square)
        {
            Int32 enemy = 1 - Color;

            //Easy cases when pieces are not sliding
            if ((PieceBitboard[enemy | Knight] & Attack.Knight(Square)) != 0
             || (PieceBitboard[enemy | Pawn] & Attack.Pawn(Color, Square)) != 0
             || (PieceBitboard[enemy | King] & Attack.King(Square)) != 0)
                return true;

            //Cases with sliding pieces
            UInt64 bishopQueenBitboard = PieceBitboard[enemy | Bishop] | PieceBitboard[enemy | Queen];
            if ((bishopQueenBitboard & Bitboard.Diagonals[Square]) != 0
             && (bishopQueenBitboard & Attack.Bishop(Square, OccupiedBB)) != 0)
                return true;

            UInt64 rookQueenBitboard = PieceBitboard[enemy | Rook] | PieceBitboard[enemy | Queen];
            if ((rookQueenBitboard & Bitboard.Axes[Square]) != 0
             && (rookQueenBitboard & Attack.Rook(Square, OccupiedBB)) != 0)
                return true;

            return false;
        }
        public bool IsInCheck(int Color)
        {
            return IsAttacked(Color, BitOperations.OnlyBitIndex(PieceBitboard[Color | King]));
        }



        public override string ToString()
        {
            string Res = string.Empty;
            UInt64 Occ = OccupiedBB;
            UInt64 bits8;
            for (int j = 0; j < 8; j++)
            {
                bits8 = (Occ & 0xFF00000000000000) >> 56;
                for (int i = 0; i < 8; i++)
                {
                    Res += bits8 & 1;
                    bits8 >>= 1;
                }
                Res += "\n";
                Occ <<= 8;
            }
            return Res;
        }
    }
    
}
