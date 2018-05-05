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

namespace PerfectChess
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
            //TestLS1BMS1B();
            //TestMove();
            TestTryBitboard();
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
        }
    }
}
