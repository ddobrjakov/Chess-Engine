using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    /// <summary>
    /// Move — int: pr sp topce frmpc tosqur fromsq(base 2)
    /// ffffff — from
    /// ttttt — to
    /// pr — promotion (00 — knight, 01 — bishop, 10 — rook, 11 — queen)
    /// sp — specialtype (00 — none 01 — promotion, 10 — en passant, 11 — castling)
    /// </summary>
    static class Move
    {
        public const int PromotionToKnight = 0;
        public const int PromotionToBishop = 1;
        public const int PromotionToRook = 2;
        public const int PromotionToQueen = 3;

        public const int SpecNone = 0;
        public const int SpecPromotion = 1;
        public const int SpecEnPassant = 2;
        public const int SpecCastling = 3;

        public static int FromSquare(int Move)
        {
            return Move & 0b111111;
        }
        public static int ToSquare(int Move)
        {
            return (Move >> 6) & 0b111111;
        }
        public static int FromPiece(int Move)
        {
            return (Move >> 12) & 0b11111;
        }
        public static int ToPiece(int Move)
        {
            return (Move >> 17) & 0b11111;
        }
        private static int SpecialCode(int Move)
        {
            return (Move >> 22) & 0b11;
        }
        private static int PromotionCode(int Move)
        {
            return Move >> 24;
        }

        public static int Create(int fromSquare, int toSquare, int fromPiece, int toPiece, int specCode = 0, int promCode = 0)
        {
            return (((((((((promCode << 2) | specCode) << 5) | toPiece) << 5) | fromPiece) << 6) | toSquare) << 6) | fromSquare;
        }

        public static bool Castling(int Move)
        {
            return SpecialCode(Move) == SpecCastling;
        }
        public static bool EnPassant(int Move)
        {
            return SpecialCode(Move) == SpecEnPassant;
        }
        public static bool Promotion(int Move)
        {
            return SpecialCode(Move) == SpecPromotion;
        }
        public static int PromotionPiece(int Move)
        {
            return (FromPiece(Move) & Color.Mask) | ((PromotionCode(Move) + 2) * 2);
        }

        public static string Details(int Move)
        {
            string pieceFrom = Piece.Name(FromPiece(Move));
            char fileFrom = (char)((int)'A' + FromSquare(Move) % 8);
            string squareFrom = fileFrom + (FromSquare(Move) / 8 + 1).ToString();

            string pieceTo = Piece.Name(ToPiece(Move));
            char fileTo = (char)((int)'A' + ToSquare(Move) % 8);
            string squareTo = fileTo + (ToSquare(Move) / 8 + 1).ToString();

            bool Captures = ToPiece(Move) != 0;

            string Description;
            if (Castling(Move))
            {
                if (ToSquare(Move) % 8 == 6) Description = "King castles short";
                else if (ToSquare(Move) % 8 == 2) Description = "King castles long";
                else Description = "King's trying to castle but he's totally lost and having troubles";
            }
            else if (Promotion(Move))
            {
                Description = "Pawn promotes to " + Piece.Name(PromotionPiece(Move));
            }
            else if (EnPassant(Move))
            {
                Description = "Pawn takes pawn enpassant";
            }
            else
            {
                Description = pieceFrom + (Captures ? (" takes " + pieceTo) : " moves"); 
            }

            string Details = String.Empty;
            Details += "[" + Color.Name(FromPiece(Move) & Color.Mask) + "]: ";
            Details += squareFrom + (Captures ? "x" : "-") + squareTo;
            Details += " (" + Description + ")";

            return Details;
        }
    }
}
