using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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


        public void DrawBorder(Square Square, System.Drawing.Color C)
        {
            Pen p = new Pen(C);
            p.Alignment = PenAlignment.Inset;
            BoardGraphics.DrawRectangle(p, GetLocationRectangle(Square));
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
            IEnumerable<Square> Squares = Square.ALL;
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







        private int[] Pieces = new int[64];
        public void SetPiece(Square S, int Piece)
        {
            ResetSquare(S);
            if (Piece != 0) SetSquareImage(S, ViewModelConnector.PieceImage[Piece]);
            Pieces[S.Index] = Piece;
        }
        public void DeletePiece(Square S)
        {
            ResetSquare(S);
            Pieces[S.Index] = 0;
        }

        public void MarkAttacked(Square S)
        {
            if (Pieces[S.Index] != 0) MarkAttackedOccupied(S);
            else MarkAttackedFree(S);
        }
        private void MarkAttackedOccupied(Square S)
        {
            if (S.Color == Color.White) SetSquareColor(S, ViewSettings.WHITE_AVAILIBLE_COLOR);
            else SetSquareColor(S, ViewSettings.BLACK_AVAILIBLE_COLOR);
        }
        private void MarkAttackedFree(Square S)
        {
            DrawSquareCenter(S, ViewSettings.CIRCLE_FILLED);
        }

        public void SetPosition(int[] Pieces)
        {
            this.Pieces = new int[64];
            for (int i = 0; i < 64; i++) this.Pieces[i] = Pieces[i];

        }

        private List<(Square S, Point CurrentLocation)> ActiveMoves = new List<(Square S, Point CurrentLocation)>();

    }
    public class BoardPanelNew : Panel
    {
        public BoardPanelNew()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint, true);

            this.DoubleBuffered = true;
            this.Width = ViewSettings.SQUARESIZE * 8;
            this.Height = ViewSettings.SQUARESIZE * 8;

            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            this.Reset();
        }

        /// <summary>
        /// Stores the pieces on their locations
        /// </summary>
        private int[] Pieces = new int[64];

        /// <summary>
        /// Sets piece but saves effects
        /// </summary>
        /// <param name="S"></param>
        /// <param name="Piece"></param>
        public void SetPiece(Square S, int Piece, bool Update = true)
        {
            Pieces[S.Index] = Piece;
            if (Update) Restore(S);
        }

        /// <summary>
        /// Deletes piece but saves effects
        /// </summary>
        /// <param name="S"></param>
        public void DeletePiece(Square S, bool Update = true)
        {
            Pieces[S.Index] = 0;
            if (Update) Restore(S);
        }

        /// <summary>
        /// List of squares marked as attacked
        /// </summary>
        private List<Square> MarkedAttacked = new List<Square>();

        /// <summary>
        /// Marks all the given squares as attacked
        /// </summary>
        /// <param name="Attacked"></param>
        public void MarkAttacked(List<Square> Attacked, bool Update = true)
        {
            foreach (Square S in Attacked)
            {
                MarkedAttacked.Add(S);
                if (Update) Restore(S);
            }
        }


        public List<(Square From, Square To)> HighlightedSquares = new List<(Square From, Square To)>();
        /// <summary>
        /// Adds squares to highlighted
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        public void ShowLastMove(Square From, Square To, bool Update = true)
        {
            HighlightedSquares.Add((From, To));
            if (Update) Restore();
        }




        /// <summary>
        /// Returns Image of piece on given square
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        private Image SquarePieceImage(Square S)
        {
            return ViewModelConnector.PieceImage[Pieces[S.Index]];
        }

        /// <summary>
        /// Sets new position and deletes the effects
        /// </summary>
        /// <param name="Pieces"></param>
        public void SetPosition(int[] Pieces, bool Update = true)
        {
            this.Pieces = new int[64];
            for (int i = 0; i < 64; i++) this.Pieces[i] = Pieces[i];

            DeleteEffects(Update);
        }

        public void DeleteEffects(bool Update = true)
        {
            this.MarkedAttacked.Clear();
            this.HighlightedSquares.Clear();
            if (Update) Restore();
        }
        public void DeleteEffects(Square S, bool Update = true)
        {
            while (MarkedAttacked.Contains(S)) MarkedAttacked.Remove(S);
            foreach ((Square S1, Square S2) A in HighlightedSquares) if (A.S1 == S || A.S2 == S) HighlightedSquares.Remove(A);
            if (Update) Restore(S);
        }



        /// <summary>
        /// Changes square color, but saves other effects
        /// </summary>
        /// <param name="Square"></param>
        /// <param name="Color"></param>
        private void ChangeSquareColor(Square Square, System.Drawing.Color Color)
        {
            //FillSquareWithDefaultColor(Square);
        }


        /// <summary>
        /// Sets position to empty and deletes the effects
        /// </summary>
        private void Reset(bool Update = true)
        {
            SetPosition(new int[64], Update);
            //for (int file = 1; file <= 8; file++)
            //    for (int rank = 1; rank <= 8; rank++)
            //        FillSquareWithDefaultColor(Square.Get(file, rank));
        }
        private void Reset(Square S, bool Update = true)
        {
            Pieces[S.Index] = 0;
            DeleteEffects(S, Update);
        }


        /// <summary>
        /// Visualizes stored data about all squares on the board
        /// </summary>
        private void Restore()
        {
            foreach (Square S in Square.ALL)
            {
                int Piece = Pieces[S.Index];
                //Reset Square Completely
                FillSquareWithDefaultColor(S);
                //Draw a piece if some
                if (Piece != 0) DrawImage(S, ViewModelConnector.PieceImage[Piece]);
            }
            foreach (Square S in MarkedAttacked)
            {
                Effect_MarkAttacked(S);
            }
            foreach ((Square From, Square To) Squares in HighlightedSquares)
                Effect_HighlightLastMove(Squares.From, Squares.To);

            //Active moves are updated always
            Effect_ShowActiveMoves();
        }

        /// <summary>
        /// Visualizes stored data about given square
        /// </summary>
        /// <param name="S"></param>
        private void Restore(Square S)
        {
            int Piece = Pieces[S.Index];

            //Reset Square Completely
            FillSquareWithDefaultColor(S);

            //Draw a piece if some
            if (Piece != 0) DrawImage(S, ViewModelConnector.PieceImage[Piece]);

            //Apply effects if some
            if (MarkedAttacked.Contains(S))
            {
                Effect_MarkAttacked(S);
            }

            foreach ((Square From, Square To) Squares in HighlightedSquares)
                if (Squares.From == S || Squares.To == S) Effect_HighlightLastMove(Squares.From, Squares.To);

            //Active moves are updated always
            Effect_ShowActiveMoves();
        }

        /// <summary>
        /// Marks square as attacked (Visually)
        /// </summary>
        /// <param name="S"></param>
        private void Effect_MarkAttacked(Square S)
        {
            if (Pieces[S.Index] != 0) Effect_MarkAttackedOccupied(S);
            else Effect_MarkAttackedFree(S);
        }
        private void Effect_MarkAttackedOccupied(Square S)
        {
            System.Drawing.Color ColorToFill = (S.Color == Color.White) ? ViewSettings.WHITE_AVAILIBLE_COLOR : ViewSettings.BLACK_AVAILIBLE_COLOR;
            FillSquareWithColor(S, ColorToFill);
            DrawImage(S, SquarePieceImage(S));
        }
        private void Effect_MarkAttackedFree(Square S)
        {
            DrawImageCenter(S, ViewSettings.CIRCLE_FILLED);
        }

        private void Effect_HighlightLastMove(Square From, Square To)
        {
            //205210106
            System.Drawing.Color FromColor = (From.Color == Color.White) ? System.Drawing.Color.FromArgb(205, 210, 106) : System.Drawing.Color.FromArgb(170, 162, 58);
            System.Drawing.Color ToColor = (To.Color == Color.White) ? System.Drawing.Color.FromArgb(205, 210, 106) : System.Drawing.Color.FromArgb(170, 162, 58);

            FillSquareWithColor(From, FromColor);//System.Drawing.Color.FromArgb(170, 162, 58));//Gray);
            if (Pieces[From.Index] != 0) DrawImage(From, SquarePieceImage(From));
            FillSquareWithColor(To, ToColor);//System.Drawing.Color.FromArgb(170, 162, 58));//Orange);
            if (Pieces[To.Index] != 0) DrawImage(To, SquarePieceImage(To));
        }


        private void Effect_ShowActiveMoves()
        {
            //Clearing all the from squares first
            foreach (var Move in ActiveMoves.Values)
            {
                Effect_MarkMovingFrom(Move.S);
            }
            //Drawing the pieces next
            foreach (var Move in ActiveMoves.Values)
            {
                BoardGraphics.DrawImage(SquarePieceImage(Move.S), new Rectangle(Move.CurrentLocation, new Size(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE)));//new Rectangle(Move.CurrentLocation, new Size(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE)));
                //ContinueMove(Move.Key, Move.Value.CurrentLocation);
            }
        }
        private void Effect_MarkMovingFrom(Square S)
        {
            FillSquareWithDefaultColor(S);
            DrawBorder(S, System.Drawing.Color.Green);
        }

        private void DrawBorder(Square Square, System.Drawing.Color C)
        {
            Pen p = new Pen(C);
            p.Alignment = PenAlignment.Inset;
            BoardGraphics.DrawRectangle(p, GetLocationRectangle(Square));
        }


        private void DrawImage(Square Square, Image Image)
        {
            BoardGraphics.DrawImage(Image, GetLocationRectangle(Square));
        }
        private void DrawImageCenter(Square Square, Image Image)
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
        private void FillSquareWithColor(Square Square, System.Drawing.Color Color)
        {
            Bitmap ColorBMP = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics.FromImage(ColorBMP).FillRectangle(new SolidBrush(Color), 0, 0, ColorBMP.Width, ColorBMP.Height);
            DrawImage(Square, ColorBMP);
        }
        private void FillSquareWithDefaultColor(Square S)
        {
            FillSquareWithColor(S, (S.Color == Color.White) ?
                        ViewSettings.WHITE_SQUARE_COLOR : ViewSettings.BLACK_SQUARE_COLOR);
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

        private Graphics BoardGraphics;
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

        public bool Flipped { get; private set; } = false;
        public void Flip(bool Update = true)
        {
            Flipped = !Flipped;
            //SoftReset();
            if (Update) Restore();
        }








        private Dictionary<int, (Square S, Point CurrentLocation)> ActiveMoves = new Dictionary<int, (Square S, Point CurrentLocation)>();
        public Square FromSquare(int Identifier) { return ActiveMoves[Identifier].S; }
        /// <summary>
        /// Returns move identifier to access it in future
        /// </summary>
        /// <param name="From"></param>
        /// <returns></returns>
        public int StartMove(Square From, bool Update = true)
        {
            int Identifier = (ActiveMoves.Keys.Any()) ? ActiveMoves.Keys.Max() + 1 : 1;
            Point InitialLocation = new Point(GetLocationRectangle(From).X, GetLocationRectangle(From).Y);
            ActiveMoves.Add(Identifier, (From, InitialLocation));
            ContinueMove(Identifier, InitialLocation);
            return Identifier;
        }
        public void ContinueMove(int Identifier, Point NewLocation, bool Update = true)
        {
            (Square S, Point OldLocation) OldMove = ActiveMoves[Identifier];
            ActiveMoves[Identifier] = (OldMove.S, NewLocation);
            if (Update) Restore();
        }
        public void DeleteMove(int Identifier, bool Update = true)
        {
            //Save from square and delete move from active
            ActiveMoves.Remove(Identifier);
            if (Update) Restore();
        } 

        public void PerformMove(Square From, Square To, bool Update = true)
        {
            Pieces[To.Index] = Pieces[From.Index];
            Pieces[From.Index] = 0;
            if (Update) Restore();
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
    }






















    /// <summary>
    /// Клетка доски - структура, хранящая столбец и строку. 
    /// </summary>
    public struct Square
    {
        public static Square Get(int index)
        {
            //return Square.Get(index % 8 + 1, index / 8 + 1);
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
            //return new Square(File, Rank);
            return Get((int)File, Rank);//_ALL[(int)File + Rank * 8];
        }
        public static Square Get(int File, int Rank)
        {
            return ALL[(File - 1) + (Rank - 1) * 8];//new Square(File, Rank);
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
