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
using System.Drawing.Drawing2D;

namespace PerfectChess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //this.BackColor = System.Drawing.Color.Yellow;
            //this.BackgroundImage = Image.FromFile("../../../images/Хрень.jpg");
            //SetGradientBackground(this, new Point(0, 0), new Point(100, 100), System.Drawing.Color.Yellow, System.Drawing.Color.Green);

            SetGradientBackground(this, new Point(0, 0), new Point(0, this.Height), System.Drawing.Color.FromArgb(0x2c, 0x2c, 0x2c), System.Drawing.Color.FromArgb(0x1a, 0x1a, 0x1a));
            Init_Buttons_Style();
            Init_Material_Style();

            //Form Test = new TestForm();
            //Test.Show();
            //SetGradientBackground(TestOutput, new Point(0, 0), new Point(0, 50), System.Drawing.Color.Black, System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));

            TestOutput.BackColor = System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5);

            BoardPanel = new BoardPanel();
            BoardPanel.Location = new Point(50, 50);
            this.Controls.Add(BoardPanel);

            BoardPanel.MouseDown += BoardPanel_MouseDown;
            BoardPanel.MouseMove += BoardPanel_MouseMove;
            BoardPanel.MouseUp += BoardPanel_MouseUp;

            BoardPanel.MouseEnter += BoardPanel_MouseEnter;
            BoardPanel.MouseLeave += BoardPanel_MouseLeave;

            SetStartPos(new Position());
        }
        private void Init_Buttons_Style()
        {
            //buttonUndo.BackgroundImage = back;
            //newGameButton.ForeColor = System.Drawing.Color.Black;

            SetGradientBackground(buttonUndo, new Point(0, 0), new Point(0, buttonUndo.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));//System.Drawing.Color.FromArgb(0x81, 0xa8, 0xcb), System.Drawing.Color.FromArgb(0x44, 0x77, 0xa1)); //#81a8cb
            SetGradientBackground(newGameButton, new Point(0, 0), new Point(0, newGameButton.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));//System.Drawing.Color.FromArgb(0xbb, 0xaa, 0xaa), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f)); //#3D4850 3%, #313d45 4%, #232B30 100%
            SetGradientBackground(buttonFlip, new Point(0, 0), new Point(buttonFlip.Width, buttonFlip.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));//System.Drawing.Color.Gray, System.Drawing.Color.White);

            //newGameButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;// BorderSize = 0;
            //newGameButton.FlatStyle = FlatStyle.Popup;

            newGameButton.FlatStyle = FlatStyle.Flat;
            newGameButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            newGameButton.FlatAppearance.BorderSize = 1;

            buttonUndo.FlatStyle = FlatStyle.Flat;
            buttonUndo.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            buttonUndo.FlatAppearance.BorderSize = 1;

            buttonFlip.FlatStyle = FlatStyle.Flat;
            buttonFlip.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            buttonFlip.FlatAppearance.BorderSize = 1;
        }
        private void Init_Material_Style()
        {
            Material1.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
            Material2.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
        }
        private void SetGradientBackground(Control C, Point From, Point To, System.Drawing.Color FromC, System.Drawing.Color ToC)
        {
            Bitmap back = new Bitmap(C.Width, C.Height);
            Graphics G = Graphics.FromImage(back);
            LinearGradientBrush backbrush = new LinearGradientBrush(From, To, FromC, ToC);
            G.FillRectangle(backbrush, 0, 0, back.Width, back.Height);
            C.BackgroundImage = back;
            C.BackgroundImageLayout = ImageLayout.Stretch;
        }




        public BoardPanel BoardPanel { get; private set; }
        public void undoButton_Click(object sender, EventArgs e)
        {
            AskForUndo?.Invoke(this, EventArgs.Empty);
        }

        private Rectangle R = new Rectangle(0, 0, ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
        private bool MoveStartAllowed = false;
        private bool MousePressed = false;

        private string _cachedText = String.Empty;

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

            _cachedText = TestOutput.Text;
            BoardPanel.MarkAttacked(EmptyAvailibleSquares);
            BoardPanel.MarkAttacked(EnemyAvailibleSquares);
            Identifier = BoardPanel.StartMove(FROM);//MousePosition);

            Point e = BoardPanel.PointToClient(MousePosition);//new Point(100,100);
            BoardPanel.ContinueMove(Identifier, new Point(e.X - ViewSettings.SQUARESIZE / 2, e.Y - ViewSettings.SQUARESIZE / 2));

            BoardPanel.Refresh();
        }
        private int Identifier = 0;
        //Updating the image while mouse moving
        private void BoardPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MoveStartAllowed) return;
            if (MousePressed)
            {
                /*R.Location = new Point(e.X - ViewSettings.SQUARESIZE / 2, e.Y - ViewSettings.SQUARESIZE / 2);

                BoardPanel.Draw(BackgroundWhileMoving);
                BoardPanel.Draw(ImageMoving, R);

                BoardPanel.Invalidate();*/

                BoardPanel.ContinueMove(Identifier, new Point(e.X - ViewSettings.SQUARESIZE / 2, e.Y - ViewSettings.SQUARESIZE / 2));
                BoardPanel.Invalidate();
                BoardPanel.Refresh();
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


            AskForFinish?.Invoke(this, new Tuple<Square, Square>(BoardPanel.FromSquare(Identifier), MovingToSquare));
            MoveStartAllowed = false;
        }      
        public void FinishMove(int Move)
        {
            //BoardPanel.BackgroundImage = PreSavedBackground;

            //(PerfectChess.Move.FromPiece(Move) & Color.Mask)
            TestOutput.ForeColor = SystemColors.WindowText;
            TestOutput.Text = "";//_cachedText;
            TestOutput.Text += "You: " + PerfectChess.Move.Details(Move) + "\n";
            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();
            TestOutput.Refresh();

            BoardPanel.DeleteMove(Identifier, false);
            PerformMove(Move);
            BoardPanel.DeleteEffects(false);
            BoardPanel.ShowLastMove(Square.Get(PerfectChess.Move.FromSquare(Move)), Square.Get(PerfectChess.Move.ToSquare(Move)));
            BoardPanel.Refresh();

            MoveStartAllowed = false;
        }
        public void CancelMove()
        {
            //Рисуем состояние доски до начала хода
            //BoardPanel.BackgroundImage = PreSavedBackground;
            //BoardPanel.Invalidate();
            BoardPanel.DeleteMove(Identifier);
            BoardPanel.DeleteMarkedEffects();

            BoardPanel.Refresh();

            TestOutput.Text = _cachedText;

            MoveStartAllowed = false;
        }

        public int SelectPromotionPiece(int Color)
        {
            PromotionForm Prom = new PromotionForm(Color);
            Prom.StartPosition = FormStartPosition.Manual;
            Prom.Location = new Point(MousePosition.X - ViewSettings.SQUARESIZE * 4, MousePosition.Y - ViewSettings.SQUARESIZE);
            Prom.ShowDialog();
            return Prom.PieceChosen;
        }

        //Undoing the last move
        public event EventHandler AskForUndo;
        public void UndoMove(int MoveToUndo)
        {
            BoardPanel.DeleteEffects();

            Square TO = Square.Get(ToSquare(MoveToUndo));
            Square FROM = Square.Get(FromSquare(MoveToUndo));      
            if (Castling(MoveToUndo))
            {
                Square ROOK_TO = Square.Get((FromSquare(MoveToUndo) + ToSquare(MoveToUndo)) / 2);
                Square ROOK_FROM = Square.Get((ToSquare(MoveToUndo) > FromSquare(MoveToUndo)) 
                    ? FromSquare(MoveToUndo) + 3 : FromSquare(MoveToUndo) - 4);

                BoardPanel.PerformMove(ROOK_TO, ROOK_FROM);
            }

            else if (Promotion(MoveToUndo))
            {
                //BoardPanel.ResetSquare(TO);
                //if (ToPiece(MoveToUndo) != 0) BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[FromPiece(MoveToUndo)]);
                BoardPanel.SetPiece(TO, FromPiece(MoveToUndo));
            }

            else if (EnPassant(MoveToUndo))
            {
                Square CapturedPawnSquare = Square.Get(ToSquare(MoveToUndo) - 8 + 16 * (FromPiece(MoveToUndo) & Color.Mask));
                //BoardPanel.SetSquareImage(CapturedPawnSquare, ViewModelConnector.PieceImage[Piece.Pawn | ((1 - FromPiece(MoveToUndo)) & Color.Mask)]);
                BoardPanel.SetPiece(CapturedPawnSquare, Piece.Pawn | ((1 - FromPiece(MoveToUndo)) & Color.Mask));
            }

            //Undoing the main part of the move
            BoardPanel.PerformMove(TO, FROM);
            BoardPanel.ShowLastMove(FROM, TO);
            if (ToPiece(MoveToUndo) != 0)
            {
                //BoardPanel.SetSquareImage(TO, ViewModelConnector.PieceImage[ToPiece(MoveToUndo)]);
                BoardPanel.SetPiece(TO, ToPiece(MoveToUndo));
            }

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();

            TestOutput.Text = String.Empty;
        }

        //Does the engine move
        public void PerformComputerMove(int Move)
        {
            TestOutput.ForeColor = SystemColors.WindowText;
            TestOutput.Text += "Engine: " + PerfectChess.Move.Details(Move) + "\n";
            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();

            //BoardPanel.Restore();
            PerformMove(Move);
            BoardPanel.DeleteEffects(false);
            BoardPanel.ShowLastMove(Square.Get(PerfectChess.Move.FromSquare(Move)), Square.Get(PerfectChess.Move.ToSquare(Move)));

            BoardPanel.Refresh();
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
            BoardPanel.PerformMove(FROM, TO);

            //Move the rook if it's castling
            if (PerfectChess.Move.Castling(Move))
            {
                Square ROOK_TO = Square.Get((PerfectChess.Move.FromSquare(Move) + PerfectChess.Move.ToSquare(Move)) / 2);
                Square ROOK_FROM = Square.Get((PerfectChess.Move.ToSquare(Move) > PerfectChess.Move.FromSquare(Move)) ? PerfectChess.Move.FromSquare(Move) + 3 : PerfectChess.Move.FromSquare(Move) - 4);
                BoardPanel.PerformMove(ROOK_FROM, ROOK_TO);
            }

            //Places the promotion piece instead of promoted pawn
            else if (PerfectChess.Move.Promotion(Move))
                BoardPanel.SetPiece(TO, PerfectChess.Move.PromotionPiece(Move));

            //Deletes the pawn if it's en passant
            else if (PerfectChess.Move.EnPassant(Move))
                BoardPanel.DeletePiece(Square.Get(PerfectChess.Move.ToSquare(Move) - 8 + 16 * (PerfectChess.Move.FromPiece(Move) & Color.Mask)));

            BoardPanel.Invalidate(true);
            BoardPanel.Refresh();
        }


        public void Checkmate(bool MovedIsHuman, bool LostIsHuman, int ColorWin)
        {
            string winloss = "Checkmate!\n";
            if (MovedIsHuman)
            {
                if (!LostIsHuman) winloss += "You won, congratulations!";
                else winloss += (ColorWin == Color.White) ? "White wins!" : "Black wins!";
            }
            else
            {
                if (LostIsHuman) winloss += "You lost:(\n\n\n\n\n\n (hehehe)";
                else winloss += (ColorWin == Color.White) ? "White wins!" : "Black wins!";
            }
            TestOutput.ForeColor = System.Drawing.Color.Red;
            TestOutput.Text += winloss;
            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();
        }
        public void Checkmate(bool Win)
        {
            TestOutput.ForeColor = System.Drawing.Color.Red;
            TestOutput.Text += "Checkmate!\n";
            string winloss = Win ? "You won, congratulations!" : "You lost:(\n\n\n\n\n\n (hehehe)";
            TestOutput.Text += winloss;

            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();

        }
        public void Check(bool Win)
        {
            TestOutput.ForeColor = System.Drawing.Color.Red;
            TestOutput.Text += "Check!\n";

            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();
        }
        public void Stalemate()
        {
            TestOutput.ForeColor = System.Drawing.Color.Green;
            TestOutput.Text += "Stalemate! It's a draw\n";

            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();
        }
        public void Title(string Text)
        {
            this.Text = Text;

            TestOutput.SelectionStart = TestOutput.TextLength;
            TestOutput.ScrollToCaret();
        }

        /// <summary>
        /// Параметр кодирует тип игры 2 битами: первый бит на белого игрока, второй на черного - 1 если игрок есть, 0 если компьютер
        /// </summary>
        public event EventHandler<int> WantNewGame;
        private void newGameButton_Click(object sender, EventArgs e)
        {
            NewGameForm GameForm = new NewGameForm();
            GameForm.StartPosition = FormStartPosition.CenterParent;
            GameForm.ShowDialog();
            if (GameForm.DialogResult == DialogResult.OK)
            {
                int State = (GameForm.WhiteHuman ? 1 : 0) * 2 + (GameForm.BlackHuman ? 1 : 0);
                WantNewGame?.Invoke(this, State);
            }
            else
            {
                //Do nothing
            }
        }
        public void SetStartPos(Position P)
        {
            BoardPanel.Reset();
            BoardPanel.SetPosition(P.SquarePiece);
            BoardPanel.Refresh();
            TestOutput.Text = "";          

            MoveStartAllowed = false;
            MousePressed = false;

            SetMaterial(0, 0);
        }

        private void buttonFlip_Click(object sender, EventArgs e)
        {
            BoardPanel.Flip();
            BoardPanel.Refresh();
            string tmp = Material1.Text;
            Material1.Text = Material2.Text;
            Material2.Text = tmp;
        }

        public void SetMaterial(int White, int Black)
        {
            if (!BoardPanel.Flipped)
            {
                Material1.Text = ((White > 0) ? "+" : "") + White.ToString();
                Material2.Text = ((Black > 0) ? "+" : "") + Black.ToString();
            }
            else
            {
                Material2.Text = ((White > 0) ? "+" : "") + White.ToString();
                Material1.Text = ((Black > 0) ? "+" : "") + Black.ToString();
            }
        }

        private void newGameButton_MouseEnter(object sender, EventArgs e)
        {
            //newGameButton.UseVisualStyleBackColor = false;

            SetGradientBackground(newGameButton, new Point(0, 0), new Point(0, newGameButton.Height), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5));
            //newGameButton.ForeColor = System.Drawing.Color.GhostWhite;
            newGameButton.FlatAppearance.BorderColor = System.Drawing.Color.GhostWhite;
            this.Cursor = Cursors.Hand;
        }
        private void newGameButton_MouseLeave(object sender, EventArgs e)
        {
            SetGradientBackground(newGameButton, new Point(0, 0), new Point(0, newGameButton.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));
            //newGameButton.UseVisualStyleBackColor = true;
            //newGameButton.ForeColor = SystemColors.WindowText;
            newGameButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Cursor = Cursors.Default;
        }

        private void buttonUndo_MouseEnter(object sender, EventArgs e)
        {
            SetGradientBackground(buttonUndo, new Point(0, 0), new Point(0, newGameButton.Height), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5));
            buttonUndo.FlatAppearance.BorderColor = System.Drawing.Color.GhostWhite;
            this.Cursor = Cursors.Hand;
        }
        private void buttonUndo_MouseLeave(object sender, EventArgs e)
        {
            SetGradientBackground(buttonUndo, new Point(0, 0), new Point(0, buttonUndo.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));
            buttonUndo.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Cursor = Cursors.Default;
        }

        private void buttonFlip_MouseEnter(object sender, EventArgs e)
        {
            SetGradientBackground(buttonFlip, new Point(0, 0), new Point(buttonFlip.Width, buttonFlip.Height), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5));
            buttonFlip.FlatAppearance.BorderColor = System.Drawing.Color.GhostWhite;
            this.Cursor = Cursors.Hand;
        }
        private void buttonFlip_MouseLeave(object sender, EventArgs e)
        {
            SetGradientBackground(buttonFlip, new Point(0, 0), new Point(buttonFlip.Width, buttonFlip.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));
            buttonFlip.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.Cursor = Cursors.Default;
        }

        private void BoardPanel_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void BoardPanel_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }

}
