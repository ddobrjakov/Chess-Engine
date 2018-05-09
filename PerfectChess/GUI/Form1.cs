using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PerfectChess.Piece;
using static PerfectChess.Color;
using static PerfectChess.Move;

namespace PerfectChess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Form Test = new TestForm();
            //Test.Show();


            this.BackColor = ViewSettings.BACKGROUND_COLOR;
            //
            // BoardPanel
            //
            BoardPanel = new BoardPanel();
            BoardPanel.Location = new Point(50, 50);
            this.Controls.Add(BoardPanel);

            //
            // Заполняем фигурками, пока тут и пока так
            //
            BoardPanel.SetSquareImage(Square.Get(2, 1), ViewSettings.WHITE_KNIGHT);
            BoardPanel.SetSquareImage(Square.Get(7, 1), ViewSettings.WHITE_KNIGHT);
            BoardPanel.SetSquareImage(Square.Get(1, 1), ViewSettings.WHITE_ROOK);
            BoardPanel.SetSquareImage(Square.Get(8, 1), ViewSettings.WHITE_ROOK);
            BoardPanel.SetSquareImage(Square.Get(3, 1), ViewSettings.WHITE_BISHOP);
            BoardPanel.SetSquareImage(Square.Get(6, 1), ViewSettings.WHITE_BISHOP);
            BoardPanel.SetSquareImage(Square.Get(4, 1), ViewSettings.WHITE_QUEEN);
            BoardPanel.SetSquareImage(Square.Get(5, 1), ViewSettings.WHITE_KING);

            BoardPanel.SetSquareImage(Square.Get(2, 8), ViewSettings.BLACK_KNIGHT);
            BoardPanel.SetSquareImage(Square.Get(7, 8), ViewSettings.BLACK_KNIGHT);
            BoardPanel.SetSquareImage(Square.Get(1, 8), ViewSettings.BLACK_ROOK);
            BoardPanel.SetSquareImage(Square.Get(8, 8), ViewSettings.BLACK_ROOK);
            BoardPanel.SetSquareImage(Square.Get(3, 8), ViewSettings.BLACK_BISHOP);
            BoardPanel.SetSquareImage(Square.Get(6, 8), ViewSettings.BLACK_BISHOP);
            BoardPanel.SetSquareImage(Square.Get(4, 8), ViewSettings.BLACK_QUEEN);
            BoardPanel.SetSquareImage(Square.Get(5, 8), ViewSettings.BLACK_KING);
            for (int i = 1; i <= 8; i++)
            {
                BoardPanel.SetSquareImage(Square.Get(i, 2), ViewSettings.WHITE_PAWN);
                BoardPanel.SetSquareImage(Square.Get(i, 7), ViewSettings.BLACK_PAWN);
            }

            BoardPanel.MouseDown += BoardPanel_MouseDown;
            BoardPanel.MouseMove += BoardPanel_MouseMove;
            BoardPanel.MouseUp += BoardPanel_MouseUp;
        }
        public BoardPanel BoardPanel { get; private set; }
        public void undoButton_Click(object sender, EventArgs e)
        {
            AskForUndo?.Invoke(this, EventArgs.Empty);
        }

        private Rectangle R = new Rectangle(0, 0, ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
        private bool MoveStartAllowed = false;
        private bool MousePressed = false;
        private Bitmap ImageMoving;
        private Bitmap PreSavedBackground;
        private Bitmap BackgroundWhileMoving;
        private Square MovingFromSquare;

        //Starting the move using the mouse
        public event EventHandler<Square> SquareTapped;
        private void BoardPanel_MouseDown(object sender, MouseEventArgs e)
        {
            MousePressed = true;
            Square TappedSquare;
            try
            {
                TappedSquare = BoardPanel.GetSquare(e.Location);
            }
            catch
            {
                MessageBox.Show("Choose a square to move from");
                return;
            }
            SquareTapped?.Invoke(this, TappedSquare);
        }
        public void StartMove(Square FROM, List<Square> EmptyAvailibleSquares, List<Square> EnemyAvailibleSquares)
        {
            MoveStartAllowed = true;

            //Сохранили изображение что мы будем двигать
            ImageMoving = new Bitmap(BoardPanel.GetSquareImage(FROM));

            //Сохранили изображение, чтобы потом в конце хода его вернуть
            PreSavedBackground = new Bitmap(BoardPanel.BackgroundImage);

            Bitmap FROM_IMAGE = new Bitmap(BoardPanel.GetSquareImage(FROM));
            BoardPanel.ResetSquare(FROM);

            TestOutput.ForeColor = SystemColors.WindowText;
            //Демонстрация тихих ходов
            TestOutput.Text = "Silent moves: ";
            foreach (Square P in EmptyAvailibleSquares)
            {
                TestOutput.Text += P.ToString();
                BoardPanel.SetSquareImageCenter(P, ViewSettings.CIRCLE_FILLED);
            }

            //Демонстрация взятий
            if (TestOutput.Text != String.Empty) TestOutput.Text += "\n";
            TestOutput.Text += "Captures: ";
            foreach (Square EnemySquare in EnemyAvailibleSquares)
            {
                TestOutput.Text += EnemySquare.ToString();
                Bitmap EnemyImage = new Bitmap(BoardPanel.GetSquareImage(EnemySquare));

                if (EnemySquare.Color == Game.Colors.White) BoardPanel.SetSquareColor(EnemySquare, ViewSettings.WHITE_AVAILIBLE_COLOR);
                else BoardPanel.SetSquareColor(EnemySquare, ViewSettings.BLACK_AVAILIBLE_COLOR);

                BoardPanel.SetSquareImage(EnemySquare, EnemyImage);
            }

            //Сохранили бэкграунд как доску без фигуры на этом поле, но с демонстрацией ходов, чтобы затем поверх нее вставлять фигуру
            BackgroundWhileMoving = new Bitmap(BoardPanel.BackgroundImage);

            BoardPanel.SetSquareImage(FROM, FROM_IMAGE);


            //Сохранили квадрат с которого начинается ход
            MovingFromSquare = FROM;
        }

        //Updating the image while mouse moving
        private void BoardPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MoveStartAllowed) return;
            if (BackgroundWhileMoving != null && MousePressed)
            {
                R.Location = new Point(e.X - ViewSettings.SQUARESIZE / 2, e.Y - ViewSettings.SQUARESIZE / 2);

                BoardPanel.Draw(BackgroundWhileMoving);
                BoardPanel.Draw(ImageMoving, R);

                BoardPanel.Invalidate();
            }
        }

        //Finishing the move started by mouse
        public event EventHandler<Tuple<Square, Square>> AskForFinish;
        private void BoardPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (MousePressed == false) return;
            if (!MoveStartAllowed) return;
            MousePressed = false;

            Square MovingToSquare;
            try
            {
                //Выбросит исключение, если вытащат за пределы доски
                MovingToSquare = BoardPanel.GetSquare(e.Location);
            }
            catch
            {
                //Если вытащили за пределы доски, сбросить
                CancelMove();
                return;
            }

            AskForFinish?.Invoke(this, new Tuple<Square, Square>(MovingFromSquare, MovingToSquare));
            MoveStartAllowed = false;
        }      
        public void FinishMove(int Move)
        {
            BoardPanel.BackgroundImage = PreSavedBackground;

            //(PerfectChess.Move.FromPiece(Move) & Color.Mask)
            TestOutput.Text = "You: " + PerfectChess.Move.Details(Move);
            TestOutput.Refresh();
            PerformMove(Move);
            MoveStartAllowed = false;
        }
        public void CancelMove()
        {
            //Рисуем состояние доски до начала хода
            BoardPanel.BackgroundImage = PreSavedBackground;
            BoardPanel.Invalidate();

            MoveStartAllowed = false;
        }

        //Undoing the last move
        public event EventHandler AskForUndo;
        public void UndoMove(int MoveToUndo)
        {
            Square TO = Square.Get(ToSquare(MoveToUndo));
            Square FROM = Square.Get(FromSquare(MoveToUndo));

            if (Castling(MoveToUndo))
            {
                Square ROOK_TO = Square.Get((FromSquare(MoveToUndo) + ToSquare(MoveToUndo)) / 2);
                Square ROOK_FROM = Square.Get((ToSquare(MoveToUndo) > FromSquare(MoveToUndo)) 
                    ? FromSquare(MoveToUndo) + 3 : FromSquare(MoveToUndo) - 4);

                PerformPrimitiveMove(ROOK_TO, ROOK_FROM);
            }

            else if (Promotion(MoveToUndo))
            {
                BoardPanel.ResetSquare(TO);
                if (ToPiece(MoveToUndo) != 0) BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[FromPiece(MoveToUndo)]);
            }

            else if (EnPassant(MoveToUndo))
            {
                Square CapturedPawnSquare = Square.Get(ToSquare(MoveToUndo) - 8 + 16 * (FromPiece(MoveToUndo) & Color.Mask));
                BoardPanel.SetSquareImage(CapturedPawnSquare, ViewModelConnector.PieceImage[Piece.Pawn | ((1 - FromPiece(MoveToUndo)) & Color.Mask)]);
            }

            //Undoing the main part of the move
            PerformPrimitiveMove(TO, FROM);
            if (ToPiece(MoveToUndo) != 0)
            {
                BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[ToPiece(MoveToUndo)]);
            }

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();
        }

        //Does the engine move
        public void PerformComputerMove(int Move)
        {
            TestOutput.Text += "\nEngine: " + PerfectChess.Move.Details(Move);
            PerformMove(Move);
        }

        /// <summary>
        /// Does the whole move without mouse participation
        /// </summary>
        /// <param name="Move"></param>
        private void PerformMove(int Move)
        {
            Square TO = Square.Get(PerfectChess.Move.ToSquare(Move));
            Square FROM = Square.Get(PerfectChess.Move.FromSquare(Move));

            //Does the main part of the move
            PerformPrimitiveMove(FROM, TO);

            //Move the rook if it's castling
            if (PerfectChess.Move.Castling(Move))
            {
                Square ROOK_TO = Square.Get((PerfectChess.Move.FromSquare(Move) + PerfectChess.Move.ToSquare(Move)) / 2);
                Square ROOK_FROM = Square.Get((PerfectChess.Move.ToSquare(Move) > PerfectChess.Move.FromSquare(Move)) ? PerfectChess.Move.FromSquare(Move) + 3 : PerfectChess.Move.FromSquare(Move) - 4);
                PerformPrimitiveMove(ROOK_FROM, ROOK_TO);
            }

            //Places the promotion piece instead of promoted pawn
            else if (PerfectChess.Move.Promotion(Move))
            {
                BoardPanel.ResetSquare(TO);
                BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[PerfectChess.Move.PromotionPiece(Move)]);
            }

            //Deletes the pawn if it's en passant
            else if (PerfectChess.Move.EnPassant(Move))
            {
                BoardPanel.ResetSquare(Square.Get(PerfectChess.Move.ToSquare(Move) - 8 + 16 * (PerfectChess.Move.FromPiece(Move) & Color.Mask)));
            }

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();
        }
        /// <summary>
        /// Does updating square images to perform an elementary move (which consists of simple moving one piece [and capturing])
        /// </summary>
        private void PerformPrimitiveMove(Square From, Square To)
        {
            BoardPanel.ResetSquare(To);
            BoardPanel.SetSquareImage(To, BoardPanel.GetSquareImage(From));
            BoardPanel.ResetSquare(From);
        }


        public void Checkmate(bool Win)
        {
            TestOutput.ForeColor = System.Drawing.Color.Red;
            TestOutput.Text += "\nCheckmate!\n";
            string winloss = Win ? "You won, congratulations!" : "You lost:(\n\n\n\n\n\n (hehehe)";
            TestOutput.Text += winloss;
        }
        public void Check(bool Win)
        {
            TestOutput.ForeColor = System.Drawing.Color.Red;
            TestOutput.Text += "\nCheck!\n";
        }
        public void Title(string Text)
        {
            this.Text = Text;
        }
    }

}
