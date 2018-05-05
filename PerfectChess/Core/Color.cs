using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    static class Color
    {
        public const int White = 0;
        public const int Black = 1;

        public const int Mask = 1; //0001

        public static string Name(int Color)
        {
            if (Color == 0) return "White";
            else if (Color == 1) return "Black";
            else return "[Error]";
        }
    }
}
