using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    /// <summary>
    /// Клетка доски - структура, хранящая столбец и строку. 
    /// </summary>
    public struct Square
    {
        public static Square Get(int index)
        {
            return ALL[index];
        }
        public static Square Get(string Square)
        {
            if (Square.Length != 2) throw new ArgumentException();
            string Lower = Square.ToLower();

            Files File;
            switch (Lower[0])
            {
                case 'a':
                    File = Files.A;
                    break;
                case 'b':
                    File = Files.B;
                    break;
                case 'c':
                    File = Files.C;
                    break;
                case 'd':
                    File = Files.D;
                    break;
                case 'e':
                    File = Files.E;
                    break;
                case 'f':
                    File = Files.F;
                    break;
                case 'g':
                    File = Files.G;
                    break;
                case 'h':
                    File = Files.H;
                    break;
                default: throw new ArgumentException();
            }
            if (!Char.IsDigit(Lower[1]) || (Char.GetNumericValue(Lower[1]) > 8) || (Char.GetNumericValue(Lower[1]) < 1)) throw new ArgumentException();
            int Rank = (int)Char.GetNumericValue(Lower[1]);
            return Get(File, Rank);//new Square(File, Rank);
        }
        public static Square Get(Files File, int Rank)
        {
            return Get((int)File, Rank);
        }
        public static Square Get(int File, int Rank)
        {
            return ALL[(File - 1) + (Rank - 1) * 8];
        }

        private Square(Files File, int Rank)
        {
            if (Rank < 1 || Rank > 8) throw new ArgumentException();
            this.File = File;
            this.Rank = Rank;
        }
        private Square(int File, int Rank)
        {
            this.File = (Files)File;
            this.Rank = Rank;
        }
        public enum Files { A = 1, B, C, D, E, F, G, H }
        public int Rank { get; private set; }
        public Files File { get; private set; }
        public int Color { get { return (X + Y + 1) % 2; } }
        public override string ToString()
        {
            return "[" + File + Rank + "]";
        }

        static Square()
        {
            Square[] All = new Square[64];
            for (int i = 0; i < 64; i++)
                All[i] = new Square(i % 8 + 1, i / 8 + 1);
            ALL = All;
        }
        public static readonly Square[] ALL;

        public int X => (int)File - 1;
        public int Y => Rank - 1;
        public int Index => X + 8 * Y;

        public static bool operator ==(Square S1, Square S2)
        {
            return (S1.File == S2.File && S1.Rank == S2.Rank);
        }
        public static bool operator !=(Square S1, Square S2)
        {
            return !(S1 == S2);
        }
        public override bool Equals(object O)
        {
            if (!(O is Square)) return false;
            return ((Square)(O) == this);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
