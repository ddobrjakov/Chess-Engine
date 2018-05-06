using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PerfectChess.Move;
using static PerfectChess.Color;
using static PerfectChess.Piece;

namespace PerfectChess
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            //TestLS1BMS1B();
            //TestMove();
            //TestTryBitboard();
            //TestPreComputed();
            //TestAttacks();
            //TestRookAttacks();
            //TestBishopAttacks();
            //TestBit();
            TestPos();
        }

        private void TestLS1BMS1B()
        {
            UInt64 ToCheck = 0b1000000010000000100000001000000010000000100000001000000010000000;
            Box.Text += Convert.ToString((long)ToCheck, 2) + "\n";
            Box.Text += BitOperations.BitScanForward(ToCheck) + " " + BitOperations.BitScanReverse((ulong)ToCheck) + "\n";

            UInt64 ToCheck2 = 0b0000100010000000100000001000000010000000100000001000000010000000;
            Box.Text += Convert.ToString((long)ToCheck2, 2) + "\n";
            Box.Text += BitOperations.BitScanForward(ToCheck2) + " " + BitOperations.BitScanReverse((ulong)ToCheck2) + "\n";

            Random R = new Random();
            for (int i = 0; i < 5; i++)
            {
                int Check = R.Next();
                Box.Text += Convert.ToString((long)Check, 2) + "\n";
                Box.Text += BitOperations.BitScanForward((ulong)Check) + " " + BitOperations.BitScanReverse((ulong)Check) + "\n";

            }

            Box.Text += BitOperations.BitScanForward(0) + " " + BitOperations.BitScanForward(0);
        }
        private void TestMove()
        {
            int move = PerfectChess.Move.Create(6, 21, Piece.Knight, Piece.None);
            Box.Text += "Knight should be moving from G1 to F3\n";
            Box.Text += PerfectChess.Move.Details(move) + "\n";

            int Move2 = PerfectChess.Move.Create(60, 62, Piece.King | Color.Black, Piece.None, SpecCastling);
            Box.Text += "King should be castling short\n";
            Box.Text += PerfectChess.Move.Details(Move2) + "\n";

            int Move3 = PerfectChess.Move.Create(0, 16, Piece.Rook | Color.White, Piece.Queen | Color.Black);
            Box.Text += "Rook should be capturing queen\n";
            Box.Text += PerfectChess.Move.Details(Move3) + "\n";
        }
        private void TestTryBitboard()
        {
            UInt64 ToCheck = 0b1000000010000000100000001000000010000000100000001000000010000000;
            Box.Text += Convert.ToString((long)ToCheck, 2) + "\n" + Bitboard.ToString(ToCheck);
            //Box.Text += "\n" + Attack.TryGetBitboard(5, 5) + " " + Attack.TryGetBitboard(-2, -3) + " " + Attack.TryGetBitboard(23, 6);
        }
        private void TestPreComputed()
        {
            Box.Text += Bitboard.ToString(Bitboard.RayN[17]);
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Bitboard.RaySE[9]);
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Bitboard.RayNE[14]);
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Bitboard.RayW[63]);
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Bitboard.RaySW[56]);
            Box.Text += "\n";


        }
        private void TestAttacks()
        {
            Box.Text += Bitboard.ToString(Attack.Knight(17));
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Attack.King(23));
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Attack.King(15));
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Attack.King(42));
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Attack.Pawn(Black, 35));
            Box.Text += "\n";

            Box.Text += Bitboard.ToString(Attack.Pawn(White, 50));
            Box.Text += "\n";
        }
        private void TestRookAttacks()
        {
            UInt64 Occupied = 0x00000084800000A00;
            Box.Text += BitOperations.BitScanForward(Occupied) + "\n";
            Box.Text += Bitboard.ToString(Occupied);
            Box.Text += "\n";
            Box.Text += Bitboard.ToString(Attack.Rook(35, Occupied));
        }
        private void TestBishopAttacks()
        {
            UInt64 Occupied = 0x4002049840020000;
            Box.Text += Bitboard.ToString(Occupied);
            Box.Text += "\n";
            Box.Text += Bitboard.ToString(Attack.Bishop(35, Occupied));
        }
        private void TestBit()
        {
            UInt64 ToCheck = 0b0000000010000000000000000000000000000000000000000000000000000000;
            Box.Text += BitOperations.OnlyBitIndex(ToCheck);

            UInt64 ToCheck2 = 0b000000001000000000000000000000000000000000000000000000000000000000;
            Box.Text += "\n" + ToCheck2 + "\n";
            Box.Text += Convert.ToString((long)BitOperations.Isolate(ToCheck2), 2);
        }
        private void TestPos()
        {
            int[] Pieces =
            {
            (White|Rook), (White|Knight), 0 , (White|Queen), (White|King), (White|Bishop), (White|Knight), (White|Rook),
            (White|Pawn), (White|Bishop) ,  (White|Pawn),   (White|Pawn),  (White|Pawn), (White|Pawn),   (White|Pawn),   (White|Pawn),
            None, (White|Pawn), None, None, None, None, None, None,
            None, None, None, None, None, None, None, None,
            None, None, None, None, None, None, None, None,
            None, None, (Black|Knight), None, (Black|Pawn), None, None, None,
            (Black|Pawn), (Black|Pawn),   (Black|Pawn),   (Black|Pawn),  0, (Black|Pawn),   (Black|Pawn),   (Black|Pawn),
            (Black|Rook), 0, (Black|Bishop), (Black|Queen), (Black|King), (Black|Bishop), (Black|Knight), (Black|Rook)
            };
            Position P = new Position(Pieces, 0, 0b1111);
            Box.Text += P.ToString() + "\n";
            //List<int> moves = new List<int>();
            List<int> moves = P.LegalMoves();
            Box.Text += moves.Count + " ходов:\n";

            foreach (int move in moves)
            {
                Box.Text += PerfectChess.Move.Details(move) + "\n";
            }



            int[] Pieces2 =
            {     
                8, 0, 0, 00, 12, 6, 4, 8,
                2, 6, 2, 02, 00, 2, 2, 2,
                0, 2, 4, 00, 02, 10, 0, 0,
                0, 0, 0, 00, 00, 0, 0, 0,
                0, 0, 0, 00, 00, 0, 0, 0,
                0, 0, 5, 00, 00, 0, 0, 0,
                3, 3, 3, 03, 00, 3, 3, 3,
                9, 0, 7, 11, 13, 7, 5, 9
            };
            P = new Position(Pieces2, 0, 0b1111);
            Box.Text += P.ToString() + "\n";
            //List<int> moves = new List<int>();
            List<int> moves2 = P.LegalMoves();
            Box.Text += moves2.Count + " ходов:\n";

            foreach (int move in moves2)
            {
                Box.Text += PerfectChess.Move.Details(move) + "\n";
            }
        }
    }
}
