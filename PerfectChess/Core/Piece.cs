using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    /// <summary>
    /// Piece type, no color (full piece contains color as additional bit)
    /// </summary>
    static class Piece
    {
        public const int None = 0;
        public const int Pawn = 2;
        public const int Knight = 4;
        public const int Bishop = 6;
        public const int Rook = 8;
        public const int Queen = 10;
        public const int King = 12;

        public const int Mask = 14; //1110

        public static string Name(int Piece)
        {
            string Name;
            switch (Piece & Mask)
            {
                case Pawn:
                    Name = "Pawn";
                    break;
                case Knight:
                    Name = "Knight";
                    break;
                case Bishop:
                    Name = "Bishop";
                    break;
                case Rook:
                    Name = "Rook";
                    break;
                case Queen:
                    Name = "Queen";
                    break;
                case King:
                    Name = "King";
                    break;
                default:
                    Name = "[Error]";
                    break;
            }
            return Name;
        }
    }
}
