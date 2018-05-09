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

            Form Test = new TestForm();
            Test.Show();


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
        public void button1_Click(object sender, EventArgs e)
        {
            AskForUndo?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler AskForUndo;

        private Rectangle R = new Rectangle(0, 0, ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
        private bool MoveStartAllowed = false;
        private bool MousePressed = false;
        private Bitmap ImageMoving;
        private Bitmap PreSavedBackground;
        private Bitmap SavedBackground;
        private Square MovingFrom;

        public event EventHandler<Square> SquareTapped;
        private void BoardPanel_MouseDown(object sender, MouseEventArgs e)
        {
            MousePressed = true;
            Square S;
            try { S = BoardPanel.GetSquare(e.Location); }
            catch { MessageBox.Show("Choose a square to move from"); return; }

            SquareTapped.Invoke(this, S);


            //StartMove(S, new List<Square>(), new List<Square>());
        }
        private void BoardPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MoveStartAllowed) return;
            if (SavedBackground != null && MousePressed)
            {
                R.Location = new Point(e.X - ViewSettings.SQUARESIZE / 2, e.Y - ViewSettings.SQUARESIZE / 2);

                BoardPanel.Draw(SavedBackground);
                BoardPanel.Draw(ImageMoving, R);

                BoardPanel.Invalidate();
            }
        }
        public event EventHandler<Tuple<Square, Square>> AskForFinish;
        private void BoardPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (!MoveStartAllowed) return;
            MousePressed = false;

            Square S;
            try
            {
                //Выбросит исключение, если вытащат за пределы доски
                S = BoardPanel.GetSquare(e.Location);
            }
            catch
            {
                //Если вытащили за пределы доски, сбросить
                CancelMove();
                return;
            }

            //Можно мне сюда походить той фигурой что вы уже разрешили походить? Как там по правилам, ребят?
            //MoveFinishRequested?.Invoke(this, new Tuple<Square, Square>(MovingFrom, S));
            AskForFinish?.Invoke(this, new Tuple<Square, Square>(MovingFrom, S));
            //FinishMove(0);
            MoveStartAllowed = false;
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
            SavedBackground = new Bitmap(BoardPanel.BackgroundImage);

            BoardPanel.SetSquareImage(FROM, FROM_IMAGE);


            //Сохранили квадрат с которого начинается ход
            MovingFrom = FROM;
        }
        public void FinishMove(int Move)
        {
            BoardPanel.BackgroundImage = PreSavedBackground;

            //int ToX = PerfectChess.Move.ToSquare(Move) % 8;
            //int ToY = PerfectChess.Move.ToSquare(Move) / 8;
            Square TO = Square.Get(PerfectChess.Move.ToSquare(Move));

            //int FromX = PerfectChess.Move.FromSquare(Move) % 8;
            //int FromY = PerfectChess.Move.FromSquare(Move) / 8;
            Square FROM = Square.Get(PerfectChess.Move.FromSquare(Move));

            BoardPanel.ResetSquare(TO);
            BoardPanel.SetSquareImage(TO, BoardPanel.GetSquareImage(FROM));
            BoardPanel.ResetSquare(FROM);
            BoardPanel.Invalidate();

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();

            if (PerfectChess.Move.Castling(Move))
            {
                Square ROOK_TO = Square.Get((PerfectChess.Move.FromSquare(Move) + PerfectChess.Move.ToSquare(Move)) / 2);
                Square ROOK_FROM = Square.Get((PerfectChess.Move.ToSquare(Move) > PerfectChess.Move.FromSquare(Move)) ? PerfectChess.Move.FromSquare(Move) + 3 : PerfectChess.Move.FromSquare(Move) - 4);
                BoardPanel.ResetSquare(ROOK_TO);
                BoardPanel.SetSquareImage(ROOK_TO, BoardPanel.GetSquareImage(ROOK_FROM));
                BoardPanel.ResetSquare(ROOK_FROM);
                BoardPanel.Invalidate();

                BoardPanel.Invalidate(true);
                BoardPanel.Refresh();
            }
            if (PerfectChess.Move.Promotion(Move))
            {
                BoardPanel.ResetSquare(TO);
                BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[PerfectChess.Move.PromotionPiece(Move)]);
                BoardPanel.Invalidate();

                BoardPanel.Invalidate(true);
                BoardPanel.Refresh();
            }
            if (PerfectChess.Move.EnPassant(Move))
            {
                BoardPanel.ResetSquare(Square.Get(PerfectChess.Move.ToSquare(Move) - 8 + 16 * (PerfectChess.Move.FromPiece(Move) & Color.Mask)));
                BoardPanel.Invalidate();

                BoardPanel.Invalidate(true);
                BoardPanel.Refresh();
            }

            MoveStartAllowed = false;
        }
        public void CancelMove()
        {
            //Рисуем состояние доски до начала хода
            BoardPanel.BackgroundImage = PreSavedBackground;
            BoardPanel.Invalidate();

            MoveStartAllowed = false;
        }

        public void UndoMove(int MoveToUndo)
        {
            Square TO = Square.Get(PerfectChess.Move.ToSquare(MoveToUndo));
            Square FROM = Square.Get(PerfectChess.Move.FromSquare(MoveToUndo));

            if (Castling(MoveToUndo))
            {
                Square ROOK_TO = Square.Get((PerfectChess.Move.FromSquare(MoveToUndo) + PerfectChess.Move.ToSquare(MoveToUndo)) / 2);
                Square ROOK_FROM = Square.Get((PerfectChess.Move.ToSquare(MoveToUndo) > PerfectChess.Move.FromSquare(MoveToUndo)) 
                    ? PerfectChess.Move.FromSquare(MoveToUndo) + 3 : PerfectChess.Move.FromSquare(MoveToUndo) - 4);

                BoardPanel.ResetSquare(ROOK_FROM);
                BoardPanel.SetSquareImage(ROOK_FROM, BoardPanel.GetSquareImage(ROOK_TO));
                BoardPanel.ResetSquare(ROOK_TO);
                BoardPanel.Invalidate();

                BoardPanel.Invalidate(true);
                BoardPanel.Refresh();
            }

            if (Promotion(MoveToUndo))
            {
                //BoardPanel.SetSquareImage(FROM, ViewModelConnector.PieceImage[FROM_PIECE(MoveToUndo)]);
                BoardPanel.ResetSquare(TO);
                if (PerfectChess.Move.ToPiece(MoveToUndo) != 0)
                    BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[PerfectChess.Move.FromPiece(MoveToUndo)]);
                BoardPanel.Invalidate();

                BoardPanel.Invalidate(true);
                BoardPanel.Refresh();
            }

            if (EnPassant(MoveToUndo))
            {
                Square CapturedPawnSquare = Square.Get(PerfectChess.Move.ToSquare(MoveToUndo) - 8 + 16 * (PerfectChess.Move.FromPiece(MoveToUndo) & Color.Mask));
                BoardPanel.SetSquareImage(CapturedPawnSquare, ViewModelConnector.PieceImage[Piece.Pawn | ((1 - PerfectChess.Move.FromPiece(MoveToUndo)) & Color.Mask)]);
            }

            BoardPanel.ResetSquare(FROM);
            BoardPanel.SetSquareImage(FROM, BoardPanel.GetSquareImage(TO));
            BoardPanel.ResetSquare(TO);

            if (PerfectChess.Move.ToPiece(MoveToUndo) != 0)
            {
                BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[PerfectChess.Move.ToPiece(MoveToUndo)]);
            }

            BoardPanel.Invalidate();

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();
        }
    }


    public class Presenter
    {
        private Form1 BoardView;
        private Position Pos;

        public Presenter(Form1 BoardView)
        {
            Pos = new Position();
            this.BoardView = BoardView;

            this.BoardView.SquareTapped += BoardView_SquareTapped;
            this.BoardView.AskForFinish += BoardView_AskForFinish;
            this.BoardView.AskForUndo += BoardView_AskForUndo;
        }

        private void BoardView_AskForUndo(object sender, EventArgs e)
        {
            int? MoveToUndo = Pos.LastMove;
            if (MoveToUndo == null)
            {
                //No moves handler
                return;
            }
            Pos.UnMake();
            BoardView.UndoMove((int)MoveToUndo);
        }

        private void BoardView_AskForFinish(object sender, Tuple<Square, Square> e)
        {
            List<int> Moves = Pos.LegalMoves();
            Square From = e.Item1;
            Square To = e.Item2;

            IEnumerable<int> MovesWhere = Moves.Where(m => PerfectChess.Move.FromSquare(m) == From.X + 8 * From.Y && PerfectChess.Move.ToSquare(m) == To.X + 8 * To.Y);
            if (MovesWhere.Any())
            {
                int MoveToMake = MovesWhere.First();

                Pos.Make(MoveToMake);
                BoardView.FinishMove(MovesWhere.First());
            }
            else { BoardView.CancelMove(); }
        }

        private void BoardView_SquareTapped(object sender, Square S)
        {
            int Piece = Pos[S.X + 8 * S.Y];
            if (Piece == 0 || (Piece & Color.Mask) != Pos.ColorToMove) { return; }// BoardView.CancelMove();

            List<int> Moves = Pos.LegalMoves();

            List<Square> EmptyAvailibleSquares = new List<Square>();
            List<Square> EnemyAvailibleSquares = new List<Square>();

            foreach (int Move in Moves.Where(m => PerfectChess.Move.FromSquare(m) == S.X + 8 * S.Y))
            {
                if (PerfectChess.Move.ToPiece(Move) == 0)
                    EmptyAvailibleSquares.Add(Square.Get(PerfectChess.Move.ToSquare(Move) % 8 + 1, PerfectChess.Move.ToSquare(Move) / 8 + 1));
                else EnemyAvailibleSquares.Add(Square.Get(PerfectChess.Move.ToSquare(Move) % 8 + 1, PerfectChess.Move.ToSquare(Move) / 8 + 1));
            }
            BoardView.StartMove(S, EmptyAvailibleSquares, EnemyAvailibleSquares);
        }





    }



}
