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
            SetSquareColor(S, (S.Color == Game.Colors.White) ?
                        ViewSettings.WHITE_SQUARE_COLOR : ViewSettings.BLACK_SQUARE_COLOR);
        }

        public void Restore()
        {
            foreach (Square S in SquareInfo.Keys)
                Restore(S);
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
            int X = (int)(Square.File - 1) * ViewSettings.SQUARESIZE;
            int Y = (8 - Square.Rank) * ViewSettings.SQUARESIZE;


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
            int File = Location.X / ViewSettings.SQUARESIZE + 1;
            int Rank = 8 - Location.Y / ViewSettings.SQUARESIZE;
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
            LocationRectangle.X = (int)(S.File - 1) * ViewSettings.SQUARESIZE;
            LocationRectangle.Y = (8 - S.Rank) * ViewSettings.SQUARESIZE;
            LocationRectangle.Width = LocationRectangle.Height = ViewSettings.SQUARESIZE;

            return LocationRectangle;
        }
    }

    /// <summary>
    /// Клетка доски - структура, хранящая столбец и строку. 
    /// </summary>
    public struct Square
    {
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
            return new Square(File, Rank);
        }
        public static Square Get(Files File, int Rank)
        {
            return new Square(File, Rank);
        }
        public static Square Get(int File, int Rank)
        {
            return new Square(File, Rank);
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
        public Game.Colors Color { get { return ((int)this.File + this.Rank) % 2 == 0 ? Game.Colors.Black : Game.Colors.White; } }
        public override string ToString()
        {
            return "[" + File + Rank + "]";
        }

        static Square()
        {
            List<Square> Squares = new List<Square>();
            foreach (Files F in Enum.GetValues(typeof(Files)))
                for (int i = 1; i <= 8; i++)
                    Squares.Add(new Square(F, i));
            AllSquares = new ReadOnlyCollection<Square>(Squares);
        }
        public static readonly ReadOnlyCollection<Square> AllSquares;

        public int X => (int)File - 1;
        public int Y => Rank - 1;

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
    public class Game
    {
        public enum Colors { White = 0, Black = 1 }
    }
}
