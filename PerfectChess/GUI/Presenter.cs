using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    public class Presenter
    {
        private Form1 BoardView;
        private Position Pos;

        private TestForm Test;
        public Presenter(Form1 BoardView)
        {
            Test = new TestForm();
            //Test.Show();

            this.Pos = new Position();
            this.BoardView = BoardView;
            this.SuperSmartEngine = new Engine();

            this.BoardView.SquareTapped += BoardView_SquareTapped;
            this.BoardView.AskForFinish += BoardView_AskForFinish;
            this.BoardView.AskForUndo += BoardView_AskForUndo;
            this.BoardView.WantNewGame += BoardView_WantNewGame;
        }

        private void BoardView_WantNewGame(object sender, EventArgs e)
        {
            this.Pos = new Position();
            this.SuperSmartEngine = new Engine();
            BoardView.SetStartPos(Pos);
        }

        private void BoardView_SquareTapped(object sender, Square S)
        {
            int Piece = Pos[S.X + 8 * S.Y];
            if (Piece == 0 || (Piece & Color.Mask) != Pos.ColorToMove) { return; }// BoardView.CancelMove();
            if (SuperSmartEngine.IsThinking) return;

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

                //Test.ShowStats(Pos);

                if (Pos.Check)
                {
                    if (Pos.Checkmate) { BoardView.Checkmate(true); return; }
                    else BoardView.Check(true);
                }


                StartEngineWork();
            }
            else { BoardView.CancelMove(); }
        }
        private void BoardView_AskForUndo(object sender, EventArgs e)
        {
            if (SuperSmartEngine.IsThinking) return;
            if (!Pos.LegalMoves().Any()) return;

            for (int i = 0; i <= 0; i++)
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
            //Test.ShowStats(Pos);
        }

        private async void StartEngineWork()
        {
            BoardView.Text = "PerfectChess Engine - Thinking...";

            int Move = -2;
            await Task.Run(() => Move = SuperSmartEngine.BestMove(Pos));
            //Move = SuperSmartEngine.BestMove(Pos);
            if (Move < 0)
            {
                BoardView.Stalemate();//throw new Exception("Нет ходов");
                BoardView.Text = "PerfectChess Engine";
                return;
            }

            Pos.Make(Move);
            BoardView.PerformComputerMove(Move);

            if (Pos.Check)
            {
                if (Pos.Checkmate) { BoardView.Checkmate(false); return; }
                else BoardView.Check(false);
            }

            BoardView.Text = "PerfectChess Engine";
        }

        private Engine SuperSmartEngine;
    }
}
