using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

            this.DoubleBuffered = true;
            this.Width = ViewSettings.SQUARESIZE * 8;
            this.Height = ViewSettings.SQUARESIZE * 8;

            this.BackgroundImage = new Bitmap(this.Width, this.Height);
            this.Reset();
        }

        #region Data
        /// <summary>
        /// Stores the pieces on their locations
        /// </summary>
        private int[] Pieces = new int[64];

        /// <summary>
        /// List of squares marked as attacked
        /// </summary>
        private List<Square> MarkedAttacked = new List<Square>();

        /// <summary>
        /// List of all highlited pairs of squares
        /// </summary>
        private List<(Square From, Square To)> HighlightedSquares = new List<(Square From, Square To)>();

        /// <summary>
        /// Active moves (Happenning at the moment)
        /// </summary>
        private Dictionary<int, (Square S, Point CurrentLocation)> ActiveMoves = new Dictionary<int, (Square S, Point CurrentLocation)>();

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
        /// Returns if the board is flipped
        /// </summary>
        public bool Flipped { get; private set; } = false;

        /// <summary>
        /// Returns square located on given location (or throws exception if none)
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns From Square of active move with certain identifier
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public Square FromSquare(int Identifier) { return ActiveMoves[Identifier].S; }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Flips the board
        /// </summary>
        /// <param name="Update"></param>
        public void Flip(bool Update = true)
        {
            Flipped = !Flipped;
            if (Update) Restore();
        }

        /// <summary>
        /// Sets new position
        /// </summary>
        /// <param name="Pieces"></param>
        public void SetPosition(int[] Pieces, bool Update = true)
        {
            this.Pieces = new int[64];
            for (int i = 0; i < 64; i++) this.Pieces[i] = Pieces[i];
            if (Update) Restore();
        }

        /// <summary>
        /// Sets position to empty and deletes the effects
        /// </summary>
        public void Reset(bool Update = true)
        {
            SetPosition(new int[64], false);
            DeleteEffects(Update);
        }

        /// <summary>
        /// Sets square to empty and deletes it's effects
        /// </summary>
        /// <param name="S"></param>
        /// <param name="Update"></param>
        private void Reset(Square S, bool Update = true)
        {
            Pieces[S.Index] = 0;
            DeleteEffects(S, Update);
        }

        /// <summary>
        /// Sets new piece on square
        /// </summary>
        /// <param name="S"></param>
        /// <param name="Piece"></param>
        public void SetPiece(Square S, int Piece, bool Update = true)
        {
            Pieces[S.Index] = Piece;
            if (Update) Restore(S);
        }

        /// <summary>
        /// Deletes piece (but saves effects)
        /// </summary>
        /// <param name="S"></param>
        public void DeletePiece(Square S, bool Update = true)
        {
            Pieces[S.Index] = 0;
            if (Update) Restore(S);
        }

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
        /// Deletes all the effects
        /// </summary>
        /// <param name="Update"></param>
        public void DeleteEffects(bool Update = true)
        {
            this.MarkedAttacked.Clear();
            this.HighlightedSquares.Clear();
            if (Update) Restore();
        }

        /// <summary>
        /// Deletes all effects related with given square
        /// </summary>
        /// <param name="S"></param>
        /// <param name="Update"></param>
        public void DeleteEffects(Square S, bool Update = true)
        {
            while (MarkedAttacked.Contains(S)) MarkedAttacked.Remove(S);
            foreach ((Square S1, Square S2) A in HighlightedSquares) if (A.S1 == S || A.S2 == S) HighlightedSquares.Remove(A);
            if (Update) Restore(S);
        }

        /// <summary>
        /// Deletes all "mark attacked" effects
        /// </summary>
        /// <param name="Update"></param>
        public void DeleteMarkedEffects(bool Update = true)
        {
            this.MarkedAttacked.Clear();
            if (Update) Restore();
        }

        /// <summary>
        /// Starts new move
        /// </summary>
        /// <param name="From"></param>
        /// <param name="Update"></param>
        /// <returns></returns>
        public int StartMove(Square From, bool Update = true)
        {
            int Identifier = (ActiveMoves.Keys.Any()) ? ActiveMoves.Keys.Max() + 1 : 1;
            Point InitialLocation = new Point(GetLocationRectangle(From).X, GetLocationRectangle(From).Y);
            ActiveMoves.Add(Identifier, (From, InitialLocation));
            ContinueMove(Identifier, InitialLocation);
            return Identifier;
        }

        /// <summary>
        /// Continues existing move
        /// </summary>
        /// <param name="Identifier"></param>
        /// <param name="NewLocation"></param>
        /// <param name="Update"></param>
        public void ContinueMove(int Identifier, Point NewLocation, bool Update = true)
        {
            (Square S, Point OldLocation) OldMove = ActiveMoves[Identifier];
            ActiveMoves[Identifier] = (OldMove.S, NewLocation);
            if (Update) Restore();
        }

        /// <summary>
        /// Deletes existing move
        /// </summary>
        /// <param name="Identifier"></param>
        /// <param name="Update"></param>
        public void DeleteMove(int Identifier, bool Update = true)
        {
            if (!ActiveMoves.Keys.Contains(Identifier)) throw new Exception("There is no active move with such identifier");
            ActiveMoves.Remove(Identifier);
            if (Update) Restore();
        }

        /// <summary>
        /// Performs simple chess move of relocating one piece
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="Update"></param>
        public void PerformMove(Square From, Square To, bool Update = true)
        {
            Pieces[To.Index] = Pieces[From.Index];
            Pieces[From.Index] = 0;
            if (Update) Restore();
        }
        #endregion

        #region DataToVisual
        /// <summary>
        /// Visualizes stored data about all squares on the board (updates board with up-to-date state)
        /// </summary>
        private void Restore()
        {
            foreach (Square S in Square.ALL)
            {
                //Reset Square Completely
                FillSquareWithDefaultColor(S);
                //Draw a piece if some
                int Piece = Pieces[S.Index];
                if (Piece != 0) DrawImage(S, ViewModelConnector.PieceImage[Piece]);
            }
            
            //Effects
            //Attacked
            foreach (Square S in MarkedAttacked) Effect_MarkAttacked(S);
            //Highlighted
            foreach ((Square From, Square To) Squares in HighlightedSquares)
                Effect_HighlightLastMove(Squares.From, Squares.To);

            //Active moves are updated always
            Effect_ShowActiveMoves();
        }

        /// <summary>
        /// Visualizes stored data about given square (updates square (and related) with up-to-date state)
        /// </summary>
        /// <param name="S"></param>
        private void Restore(Square S)
        {
            //Reset Square Completely
            FillSquareWithDefaultColor(S);

            //Draw a piece if some
            int Piece = Pieces[S.Index];
            if (Piece != 0) DrawImage(S, ViewModelConnector.PieceImage[Piece]);

            //Apply effects if some
            //Attacked
            if (MarkedAttacked.Contains(S)) Effect_MarkAttacked(S);
            //Highlighted
            foreach ((Square From, Square To) Squares in HighlightedSquares)
                if (Squares.From == S || Squares.To == S) Effect_HighlightLastMove(Squares.From, Squares.To);

            //Active moves are updated always
            Effect_ShowActiveMoves();
        }
        #endregion

        #region ApplyingEffects
        ///These methods are called from Restore method, where the order of them is very important,
        ///if we want them to display effects correctly

        /// <summary>
        /// Visually marks square as attacked
        /// </summary>
        /// <param name="S"></param>
        private void Effect_MarkAttacked(Square S)
        {
            if (Pieces[S.Index] != 0) Effect_MarkAttackedOccupied(S);
            else Effect_MarkAttackedFree(S);
        }

        /// <summary>
        /// Visually marks occupied square as attacked
        /// </summary>
        /// <param name="S"></param>
        private void Effect_MarkAttackedOccupied(Square S)
        {
            System.Drawing.Color ColorToFill = (S.Color == Color.White) ? ViewSettings.WHITE_AVAILIBLE_COLOR : ViewSettings.BLACK_AVAILIBLE_COLOR;
            FillSquareWithColor(S, ColorToFill);
            DrawImage(S, SquarePieceImage(S));
        }

        /// <summary>
        /// Visually marks empty square as attacked
        /// </summary>
        /// <param name="S"></param>
        private void Effect_MarkAttackedFree(Square S)
        {
            DrawImageCenter(S, ViewSettings.CIRCLE_FILLED);
        }

        /// <summary>
        /// Visually highlights squares with different colors
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
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

        /// <summary>
        /// Updates the board with active moves on places
        /// </summary>
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
                BoardGraphics.DrawImage(SquarePieceImage(Move.S), new Rectangle(Move.CurrentLocation, new Size(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE)));
            }
        }

        /// <summary>
        /// Visually marks square as moving from during the move (used to show active moves) 
        /// </summary>
        /// <param name="S"></param>
        private void Effect_MarkMovingFrom(Square S)
        {
            FillSquareWithDefaultColor(S);

            ColorMatrix cm = new ColorMatrix();
            cm.Matrix33 = 0.60f;
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            BoardGraphics.DrawImage(SquarePieceImage(S), GetLocationRectangle(S), 0, 0, ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE, GraphicsUnit.Pixel, ia);

            //DrawBorder(S, System.Drawing.Color.Green);
        }
        #endregion

        #region DrawingCore
        /// <summary>
        /// Graphics we are always drawing on (graphics of background image)
        /// </summary>
        private Graphics BoardGraphics;

        /// <summary>
        /// Overriding the BackgroundImage set property in order to maintain BoardGraphics up-to-date
        /// </summary>
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

        /// <summary>
        /// Draws image on square
        /// </summary>
        /// <param name="Square"></param>
        /// <param name="Image"></param>
        private void DrawImage(Square Square, Image Image)
        {
            BoardGraphics.DrawImage(Image, GetLocationRectangle(Square));
        }

        /// <summary>
        /// Draws image in the center of square using its original size
        /// </summary>
        /// <param name="Square"></param>
        /// <param name="Image"></param>
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

        /// <summary>
        /// Fills the square with one color
        /// </summary>
        /// <param name="Square"></param>
        /// <param name="Color"></param>
        private void FillSquareWithColor(Square Square, System.Drawing.Color Color)
        {
            Bitmap ColorBMP = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics.FromImage(ColorBMP).FillRectangle(new SolidBrush(Color), 0, 0, ColorBMP.Width, ColorBMP.Height);
            DrawImage(Square, ColorBMP);
        }

        /// <summary>
        /// Fills the square with color defined as default in settings
        /// </summary>
        /// <param name="S"></param>
        private void FillSquareWithDefaultColor(Square S)
        {
            FillSquareWithColor(S, (S.Color == Color.White) ?
                        ViewSettings.WHITE_SQUARE_COLOR : ViewSettings.BLACK_SQUARE_COLOR);
        }

        /// <summary>
        /// Returns rectangle which is covering position of chess square
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Draws border of certain color around square
        /// </summary>
        /// <param name="Square"></param>
        /// <param name="C"></param>
        private void DrawBorder(Square Square, System.Drawing.Color C)
        {
            Pen p = new Pen(C);
            p.Alignment = PenAlignment.Inset;
            BoardGraphics.DrawRectangle(p, GetLocationRectangle(Square));
        }
        #endregion
    }
}
