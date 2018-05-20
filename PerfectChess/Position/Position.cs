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
        public Position() : this(InitialPieces, White, 0b1111)
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
                PieceBitboard[PieceToAdd] |= (1UL << i); //Piece array
                PieceBitboard[PieceToAdd & Color.Mask] |= (1UL << i); //Color array
                OccupiedBB |= (1UL << i); //All pieces array
            }
            ColorToMove = ToMove;

            CastleShortIndex[White] = CastlingRights & 0b0001;
            CastleShortIndex[Black] = (CastlingRights & 0b0010) >> 1;
            CastleLongIndex[White] = (CastlingRights & 0b0100) >> 2;
            CastleLongIndex[Black] = (CastlingRights & 0b1000) >> 3;

            EnPassantHistory.Push(EnPassant);
        }

        /// <summary>
        /// Returns a bitboard of all squares containing piece of given type 
        /// </summary>
        public readonly UInt64[] PieceBitboard = new UInt64[15];
        
        /// <summary>
        /// Returns a bitboard with all occupied squares with pieces of either color
        /// </summary>
        private UInt64 OccupiedBB;

        /// <summary>
        /// Returns piece located on given square
        /// </summary>
        private int[] SquarePiece = new int[64];

        /// <summary>
        /// Public property to get a piece located on certain square
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[int index] => SquarePiece[index];

        /// <summary>
        /// Represents castling king side rights for each color (1 means "can castle", 0 and less means no)
        /// </summary>
        public int[] CastleShortIndex = new int[2];

        /// <summary>
        /// Represents castling queen side rights for each color (1 means "can castle", 0 and less means no)
        /// </summary>
        public int[] CastleLongIndex = new int[2];

        /// <summary>
        /// Stores the current side to move
        /// </summary>
        public int ColorToMove { get; private set; } = White;

        /// <summary>
        /// Stores the square behind enpassant pawn (-1 if none)
        /// </summary>
        public int EnPassantSquare => EnPassantHistory.Peek();

        /// <summary>
        /// Stores the history of enPassant existance and location at each move
        /// </summary>
        private Stack<int> EnPassantHistory = new Stack<int>();

        /// <summary>
        /// Returns the number of moves played since the position creation
        /// </summary>
        private int HalfMoves => MoveHistory.Count();

        /// <summary>
        /// Stores the history of moves made, particularly needed as the source for unmaking moves
        /// </summary>
        private Stack<int> MoveHistory = new Stack<int>();

        /// <summary>
        /// Returns if any moves were played in position
        /// </summary>
        public bool AnyMoves => MoveHistory.Any();

        /// <summary>
        /// Returns the last move played
        /// </summary>
        public int? LastMove {
            get
            {
                return MoveHistory.Any() ? MoveHistory.Peek() : (int?)null;
            }
        }

        //private UInt64 WhitePawnsAttacks => ((WhitePawns & ~FileABB) << 7) | ((WhitePawns & ~FileHBB) << 9);

        /// <summary>
        /// Returns the file of the given square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public static int File(int square)
        {
            return square & 7;
        }

        /// <summary>
        /// Returns the rank of the given square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public static int Rank(int square)
        {
            return square >> 3;
        }

        /// <summary>
        /// Represents an invalid square
        /// </summary>
        private const int InvalidSquare = -1;

        /// <summary>
        /// Represents the starting position
        /// </summary>
        public static readonly int[] InitialPieces =
        {
            (White|Rook), (White|Knight), (White|Bishop), (White|Queen), (White|King), (White|Bishop), (White|Knight), (White|Rook),
            (White|Pawn), (White|Pawn),   (White|Pawn),   (White|Pawn),  (White|Pawn), (White|Pawn),   (White|Pawn),   (White|Pawn),
            None,          None,          None,           None,          None,         None,           None,           None,
            None,          None,          None,           None,          None,         None,           None,           None,
            None,          None,          None,           None,          None,         None,           None,           None,
            None,          None,          None,           None,          None,         None,           None,           None,           
            (Black|Pawn), (Black|Pawn),   (Black|Pawn),   (Black|Pawn),  (Black|Pawn), (Black|Pawn),   (Black|Pawn),   (Black|Pawn),
            (Black|Rook), (Black|Knight), (Black|Bishop), (Black|Queen), (Black|King), (Black|Bishop), (Black|Knight), (Black|Rook)
        };

        /// <summary>
        /// Returns if the given square with a piece of given color is attcked
        /// </summary>
        /// <param name="Color"></param>
        /// <param name="Square"></param>
        /// <returns></returns>
        public bool IsAttacked(int Color, int Square)
        {
            int enemy = 1 - Color;

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

        public bool Attacks(int FromSquare, int ToSquare)
        {
            //if (SquarePiece[FromSquare] == 0 || (SquarePiece[FromSquare] & Color.Mask) == (SquarePiece[ToSquare] & Color.Mask)) return false;
            int piece = SquarePiece[FromSquare];
            switch (piece & Piece.Mask)
            {
                case Piece.None: return false;
                case Piece.Pawn: return (Attack.Pawn(SquarePiece[FromSquare] & Color.Mask, FromSquare) & (1UL << ToSquare)) != 0;
                case Piece.Knight: return (Attack.Knight(FromSquare) & (1UL << ToSquare)) != 0;
                case Piece.Bishop: return (Attack.Bishop(FromSquare, OccupiedBB) & (1UL << ToSquare)) != 0;
                case Piece.Rook: return (Attack.Rook(FromSquare, OccupiedBB) & (1UL << ToSquare)) != 0;
                case Piece.Queen: return (Attack.Queen(FromSquare, OccupiedBB) & (1UL << ToSquare)) != 0;
                case Piece.King: return (Attack.King(FromSquare) & (1UL << ToSquare)) != 0;
                default: throw new Exception();
            }
        }



        /// <summary>
        /// Returns if side is in check
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public bool IsInCheck(int Color)
        {
            return IsAttacked(Color, BitOperations.OnlyBitIndex(PieceBitboard[Color | King]));
        }

        /// <summary>
        /// Returns if current position is a check
        /// </summary>
        public bool Check => IsInCheck(ColorToMove);

        /// <summary>
        /// Returns if current position is a checkmate
        /// </summary>
        public bool Checkmate => Check && !LegalMoves().Any();

        /// <summary>
        /// Returns if current position is a stalemate
        /// </summary>
        public bool Stalemate => !LegalMoves().Any() && !Check;

        public bool GameFinished => !LegalMoves().Any();

        /// <summary>
        /// Returns new copy of current Position entity
        /// </summary>
        /// <returns></returns>
        public Position DeepCopy()
        {
            Position Copied = new Position();
            List<int> MovesInOrder = MoveHistory.ToList();
            MovesInOrder.Reverse();
            foreach (int Move in MovesInOrder)
                Copied.Make(Move);
            return Copied;
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
