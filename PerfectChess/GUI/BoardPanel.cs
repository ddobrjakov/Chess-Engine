using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectChess
{
    /// <summary>
    /// Контрол, представляющий доску
    /// </summary>
    public class BoardPanel : Panel
    {
        public BoardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint, true);

            //Squares = new SquareVisualInfo[9, 9];
            this.DoubleBuffered = true;
            this.Width = ViewSettings.SQUARESIZE * 8;
            this.Height = ViewSettings.SQUARESIZE * 8;
            this.Reset();
        }

        public bool Flipped { get; private set; } = false;
        public void Flip()
        {
            Flipped = !Flipped;
            SoftReset();
            Restore();
        }

        //public SquareInfo[,] Squares;
        private Dictionary<Square, Image> SquareInfo = new Dictionary<Square, Image>();//SquareVisualInfo> SquareInfo = new Dictionary<Square, SquareVisualInfo>();
        private Graphics BoardGraphics;

        //
        // Public Methods
        //

        //Изменения не приводят к изменению содержимого словаря
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
                BoardGraphics = Graphics.FromImage(this.BackgroundImage);
            }
        }
        public void DrawSquare(Square Square, Image Image)
        {
            BoardGraphics.DrawImage(Image, GetLocationRectangle(Square));
        }
        public void DrawSquareCenter(Square Square, Image Image)
        {
            Rectangle LocationRectangle = new Rectangle();
            int X, Y;

            if (!Flipped)
            {
                X = (int)(Square.File - 1) * ViewSettings.SQUARESIZE;
                Y = (8 - Square.Rank) * ViewSettings.SQUARESIZE;
            }
            else
            {
                X = (int)(8 - Square.File) * ViewSettings.SQUARESIZE;
                Y = (Square.Rank - 1) * ViewSettings.SQUARESIZE;
            }

            LocationRectangle.X = X + (ViewSettings.SQUARESIZE - Image.Width) / 2;
            LocationRectangle.Y = Y + (ViewSettings.SQUARESIZE - Image.Height) / 2;
            LocationRectangle.Width = Image.Width;
            LocationRectangle.Height = Image.Height;

            BoardGraphics.DrawImage(Image, LocationRectangle);
        }
        public void Draw(Image Image, Rectangle R)
        {
            BoardGraphics.DrawImage(Image, R);
        }
        public void Draw(Image Image, int x, int y)
        {
            BoardGraphics.DrawImage(Image, x, y);
        }
        public void Draw(Image Image, int x, int y, int width, int height)
        {
            BoardGraphics.DrawImage(Image, x, y, width, height);
        }
        public void Draw(Image Image)
        {
            BoardGraphics.DrawImage(Image, 0, 0, this.Width, this.Height);
        }

        public void SoftReset()
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            for (int file = 1; file <= 8; file++)
                for (int rank = 1; rank <= 8; rank++)
                    SoftResetSquare(Square.Get(file, rank));
        }
        public void SoftResetSquare(Square S)
        {
            System.Drawing.Color C = (S.Color == Color.White) ?
                       ViewSettings.WHITE_SQUARE_COLOR : ViewSettings.BLACK_SQUARE_COLOR;
            Bitmap SolidColorBMP = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics.FromImage(SolidColorBMP).FillRectangle(new SolidBrush(C), 0, 0, SolidColorBMP.Width, SolidColorBMP.Height);

            DrawSquare(S, SolidColorBMP);
        }

        //Воздействуют на словарь (зависят от него)
        public void Reset()
        {
            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            for (int file = 1; file <= 8; file++)
                for (int rank = 1; rank <= 8; rank++)
                    ResetSquare(Square.Get(file, rank));
        }
        public void ResetSquare(Square S)
        {
            SetSquareColor(S, (S.Color == Color.White) ?
                        ViewSettings.WHITE_SQUARE_COLOR : ViewSettings.BLACK_SQUARE_COLOR);
        }


        public void Restore()
        {
            IEnumerable<Square> Squares = Square._ALL;
            foreach (Square S in Squares)
                Restore(S);
            this.Refresh();
        }
        public void Restore(Square S)
        {
            SetSquareImage(S, GetSquareImage(S));
        }

        public void SetSquareImage(Square Square, Image Image)
        {
            BoardGraphics.DrawImage(Image, GetLocationRectangle(Square));

            //Заносим в словарь
            if (SquareInfo.ContainsKey(Square)) { SquareInfo[Square] = Image; } //SquareVisualInfo info = SquareInfo[Square]; info.Image = Image; }
            else SquareInfo.Add(Square, Image);//new SquareVisualInfo(Square, Image));
        }
        public void SetSquareColor(Square Square, System.Drawing.Color Color)
        {
            Bitmap SolidColorBMP = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics.FromImage(SolidColorBMP).FillRectangle(new SolidBrush(Color), 0, 0, SolidColorBMP.Width, SolidColorBMP.Height);

            SetSquareImage(Square, SolidColorBMP);
        }
        public void SetSquareImageCenter(Square Square, Image Image)
        {
            Rectangle LocationRectangle = new Rectangle();
            int X, Y;

            if (!Flipped)
            {
                X = (int)(Square.File - 1) * ViewSettings.SQUARESIZE;
                Y = (8 - Square.Rank) * ViewSettings.SQUARESIZE;
            }
            else
            {
                X = (int)(8 - Square.File) * ViewSettings.SQUARESIZE;
                Y = (Square.Rank - 1) * ViewSettings.SQUARESIZE;
            }

            LocationRectangle.X = X + (ViewSettings.SQUARESIZE - Image.Width) / 2;
            LocationRectangle.Y = Y + (ViewSettings.SQUARESIZE - Image.Height) / 2;
            LocationRectangle.Width = Image.Width;
            LocationRectangle.Height = Image.Height;

            BoardGraphics.DrawImage(Image, LocationRectangle);
            if (SquareInfo.ContainsKey(Square)) { SquareInfo[Square] = Image; } //SquareVisualInfo info = SquareInfo[Square]; info.Image = Image; }
            else SquareInfo.Add(Square, Image);//new SquareVisualInfo(Square, Image));
        }

        public Square GetSquare(Point Location)
        {
            if (Location.X > 8 * ViewSettings.SQUARESIZE || Location.Y > 8 * ViewSettings.SQUARESIZE) throw new ArgumentException();

            int File, Rank;
            if (!Flipped)
            {
                File = Location.X / ViewSettings.SQUARESIZE + 1;
                Rank = 8 - Location.Y / ViewSettings.SQUARESIZE;
            }
            else
            {
                File = 8 - Location.X / ViewSettings.SQUARESIZE;
                Rank = Location.Y / ViewSettings.SQUARESIZE + 1;
            }
            return Square.Get(File, Rank);
        }
        public Image GetSquareImage(Square Square)
        {
            return SquareInfo[Square];
        }

        //
        // Private Methods
        //
        private void SetSquareImage(int File, int Rank, Image Image)
        {
            SetSquareImage(Square.Get((Square.Files)File, Rank), Image);
        }
        private void SetSquareColor(int File, int Rank, System.Drawing.Color Color)
        {
            SetSquareColor(Square.Get((Square.Files)File, Rank), Color);
        }
        private Rectangle GetLocationRectangle(Square S)
        {
            Rectangle LocationRectangle = new Rectangle();
            if (!Flipped)
            {
                LocationRectangle.X = (int)(S.File - 1) * ViewSettings.SQUARESIZE;
                LocationRectangle.Y = (8 - S.Rank) * ViewSettings.SQUARESIZE;
                LocationRectangle.Width = LocationRectangle.Height = ViewSettings.SQUARESIZE;
            }
            else
            {
                LocationRectangle.X = (int)(8 - S.File) * ViewSettings.SQUARESIZE;
                LocationRectangle.Y = (S.Rank - 1) * ViewSettings.SQUARESIZE;
                LocationRectangle.Width = LocationRectangle.Height = ViewSettings.SQUARESIZE;
            }
            return LocationRectangle;
        }
    }

    /// <summary>
    /// Клетка доски - структура, хранящая столбец и строку. 
    /// </summary>
    public struct Square
    {
        public static Square Get(int index)
        {
            //return Square.Get(index % 8 + 1, index / 8 + 1);
            return _ALL[index];
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
            //return new Square(File, Rank);
            return Get((int)File, Rank);//_ALL[(int)File + Rank * 8];
        }
        public static Square Get(int File, int Rank)
        {
            return _ALL[(File - 1) + (Rank - 1) * 8];//new Square(File, Rank);
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
        public int Color { get { return ((int)this.File + this.Rank) % 2 == 0 ? PerfectChess.Color.Black : PerfectChess.Color.White; } }
        public override string ToString()
        {
            return "[" + File + Rank + "]";
        }

        static Square()
        {
            Square[] All = new Square[64];
            for (int i = 0; i < 64; i++)
                All[i] = new Square(i % 8 + 1, i / 8 + 1);
            _ALL = All;
        }
        public static readonly Square[] _ALL;

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
    public static class ViewSettings
    {
        public const int SQUARESIZE = 60;
        public static readonly System.Drawing.Color BACKGROUND_COLOR = System.Drawing.Color.FromArgb(26, 26, 26);
        public static readonly System.Drawing.Color WHITE_SQUARE_COLOR = System.Drawing.Color.FromArgb(240, 217, 181);
        public static readonly System.Drawing.Color BLACK_SQUARE_COLOR = System.Drawing.Color.FromArgb(181, 136, 99);

        public static readonly System.Drawing.Color WHITE_AVAILIBLE_COLOR = System.Drawing.Color.FromArgb(240, 149, 119);
        public static readonly System.Drawing.Color BLACK_AVAILIBLE_COLOR = System.Drawing.Color.FromArgb(181, 105, 74);


        public static readonly Image WHITE_PAWN = Image.FromFile("../../../images/WhitePawn.png");
        public static readonly Image WHITE_KNIGHT = Image.FromFile("../../../images/WhiteKnight.png");
        public static readonly Image WHITE_BISHOP = Image.FromFile("../../../images/WhiteBishop.png");
        public static readonly Image WHITE_ROOK = Image.FromFile("../../../images/WhiteRook.png");
        public static readonly Image WHITE_QUEEN = Image.FromFile("../../../images/WhiteQueen.png");
        public static readonly Image WHITE_KING = Image.FromFile("../../../images/WhiteKing.png");

        public static readonly Image BLACK_PAWN = Image.FromFile("../../../images/BlackPawn.png");
        public static readonly Image BLACK_KNIGHT = Image.FromFile("../../../images/BlackKnight.png");
        public static readonly Image BLACK_BISHOP = Image.FromFile("../../../images/BlackBishop.png");
        public static readonly Image BLACK_ROOK = Image.FromFile("../../../images/BlackRook.png");
        public static readonly Image BLACK_QUEEN = Image.FromFile("../../../images/BlackQueen.png");
        public static readonly Image BLACK_KING = Image.FromFile("../../../images/BlackKing.png");


        public static readonly Image CIRCLE_FILLED = Image.FromFile("../../../images/CircleFilled.png");
    }


    public static class ViewModelConnector
    {
        public static Dictionary<int, Image> PieceImage = new Dictionary<int, Image>
        {
            { PerfectChess.Color.White | PerfectChess.Piece.Pawn, ViewSettings.WHITE_PAWN },
            { PerfectChess.Color.White | PerfectChess.Piece.Knight, ViewSettings.WHITE_KNIGHT },
            { PerfectChess.Color.White | PerfectChess.Piece.Bishop, ViewSettings.WHITE_BISHOP },
            { PerfectChess.Color.White | PerfectChess.Piece.Rook, ViewSettings.WHITE_ROOK },
            { PerfectChess.Color.White | PerfectChess.Piece.Queen, ViewSettings.WHITE_QUEEN },
            { PerfectChess.Color.White | PerfectChess.Piece.King, ViewSettings.WHITE_KING },
            { PerfectChess.Color.Black | PerfectChess.Piece.Pawn, ViewSettings.BLACK_PAWN },
            { PerfectChess.Color.Black | PerfectChess.Piece.Knight, ViewSettings.BLACK_KNIGHT },
            { PerfectChess.Color.Black | PerfectChess.Piece.Bishop, ViewSettings.BLACK_BISHOP },
            { PerfectChess.Color.Black | PerfectChess.Piece.Rook, ViewSettings.BLACK_ROOK },
            { PerfectChess.Color.Black | PerfectChess.Piece.Queen, ViewSettings.BLACK_QUEEN },
            { PerfectChess.Color.Black | PerfectChess.Piece.King, ViewSettings.BLACK_KING }
        };
        public static Dictionary<int, System.Drawing.Color> RealColor = new Dictionary<int, System.Drawing.Color>
        {
            { PerfectChess.Color.White, ViewSettings.WHITE_SQUARE_COLOR },
            { PerfectChess.Color.Black, ViewSettings.BLACK_SQUARE_COLOR }
        };
    }
}
