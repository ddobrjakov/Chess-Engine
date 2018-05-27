using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectChess
{  
    public abstract class Player
    {
        public abstract void YourMove(Position P);
        public void MakeMove(int Move)
        {
            MakesMove?.Invoke(this, Move);
        }

        public Stack<int> PreMoves { get; private set; } = new Stack<int>();
        public event EventHandler<int> MakesMove;

        public abstract bool IsThinking { get; }
    }
    public class HumanPlayer : Player
    {
        public override void YourMove(Position P)
        {
            if (PreMoves.Any()) MakeMove(PreMoves.Pop());
            //I don't care else
        }
        public override bool IsThinking => false;
    }

    public class EnginePlayer : Player
    {
        private Engine E = new Engine();
        public override async void YourMove(Position P)
        {
            if (PreMoves.Any()) MakeMove(PreMoves.Pop());
            else
            {
                int Move = -1;
                await Task.Run(() => Move = E.BestMove(P));
                MakeMove(Move);
            }
        }
        public override bool IsThinking => E.IsThinking;

        //Testing
        public int TEST_Evaluated => E.TEST_Evaluated;
        public TimeSpan TEST_ThinkTime => E.TEST_ThinkTime;
        public int TEST_LegalMovesCallCount => E.TEST_LegalMovesCallCount;
        public int TEST_Nodes => E.TEST_Nodes;
        public int TEST_AttacksCallCount => E.TEST_AttacksCallCount;
    }




    public class Presenter
    {
        public Presenter(Form1 BoardView, Player PlayerWhite, Player PlayerBlack, Position StartPosition)
        {
            this.PlayerWhite = PlayerWhite;
            this.PlayerBlack = PlayerBlack;
            this.GamePosition = StartPosition;
            this.BoardView = BoardView;
            BoardView.SetStartPos(StartPosition);

            this.BoardView.AskForFinish += BoardView_AskForFinish;
            this.BoardView.AskForUndo += BoardView_AskForUndo;
            this.BoardView.WantNewGame += BoardView_WantNewGame;
            this.BoardView.SquareTapped += BoardView_SquareTapped;


            this.PlayerWhite.MakesMove += Player_MakesMove;
            this.PlayerBlack.MakesMove += Player_MakesMove;

            if (Global.USE_TEST) Test.Show();
        }

        private void BoardView_WantNewGame(object sender, int Players)
        {
            PlayerWhite = (((Players & 0b10) >> 1) == 1) ? (Player)(new HumanPlayer()) : (new EnginePlayer());
            PlayerBlack = ((Players & 0b01) == 1) ? (Player)(new HumanPlayer()) : (new EnginePlayer());
            this.PlayerWhite.MakesMove += Player_MakesMove;
            this.PlayerBlack.MakesMove += Player_MakesMove;

            GamePosition = new Position();
            BoardView.SetStartPos(GamePosition);

            if (PlayerBlack is HumanPlayer && PlayerWhite is EnginePlayer && !BoardView.BoardPanel.Flipped)
                BoardView.BoardPanel.Flip();
            if (PlayerWhite is HumanPlayer && PlayerBlack is EnginePlayer && BoardView.BoardPanel.Flipped)
                BoardView.BoardPanel.Flip();

            PlayerWhite.YourMove(GamePosition);
        }
        private void BoardView_SquareTapped(object sender, Square S)
        {
            if (PlayerToMove is EnginePlayer && PlayerWaiting is EnginePlayer) MessageBox.Show("Don't bother the comps! Let them play!");
            int Piece = GamePosition[S.X + 8 * S.Y];
            if (Piece == 0) return;
            if (PlayerToMove is EnginePlayer && ((Piece & Color.Mask) == ((PlayerToMove == PlayerWhite) ? Color.White : Color.Black))) return;
            if (PlayerToMove.IsThinking) return;


            List<int> Moves = GamePosition.LegalMoves();

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
            int? Move = ConvertMove(e.Item1, e.Item2, GamePosition);
            if (Move is null) BoardView.CancelMove();
            else
            {
                if (PlayerToMove is HumanPlayer) PlayerToMove.MakeMove((int)Move);
                else if (PlayerWaiting is HumanPlayer) { PlayerWaiting.PreMoves.Push((int)Move); }
                else { MessageBox.Show("Don't bother the comps! Let them play!"); }
            }
        }
        private int? ConvertMove(Square From, Square To, Position P)
        {
            IEnumerable<int> AppropriateMoves = P.LegalMoves().Where(m => PerfectChess.Move.FromSquare(m) == From.X + 8 * From.Y && PerfectChess.Move.ToSquare(m) == To.X + 8 * To.Y);
            if (!AppropriateMoves.Any()) return null;

            if (AppropriateMoves.Count() > 1)
            {
                int ChosenPiece = BoardView.SelectPromotionPiece(GamePosition.ColorToMove);
                if (ChosenPiece == 0) return null;
                AppropriateMoves = AppropriateMoves.Where(move => Move.PromotionPiece(move) == ChosenPiece);
                if (!AppropriateMoves.Any() || AppropriateMoves.Count() > 1) { MessageBox.Show("Произошло что-то странное"); return null; }
            }
            return AppropriateMoves.First();
        }

        private void Player_MakesMove(object sender, int Move)
        {
            //Check for legality
            if (!GamePosition.LegalMoves().Contains(Move))
            {
                return;
            }

            //Make the move
            GamePosition.Make(Move);
            if (PlayerWaiting is HumanPlayer) BoardView.FinishMove(Move);
            else BoardView.PerformComputerMove(Move);

            //Add visual move effects
            ApplyMoveEffects();
            if (GamePosition.GameFinished) return;

            //Tell the other guy he can move
            PlayerToMove.YourMove(GamePosition.DeepCopy());
        }
        private void ApplyMoveEffects()
        {
            //PlayerWaiting is the player who made the last move

            //Check or mate
            if (GamePosition.Check)
            {
                bool MovedIsHuman = PlayerWaiting is HumanPlayer;
                bool LostIsHuman = PlayerToMove is HumanPlayer;
                int ColorWin = (PlayerWaiting == PlayerWhite) ? Color.White : Color.Black;
                if (GamePosition.Checkmate) BoardView.Checkmate(MovedIsHuman, LostIsHuman, ColorWin);
                else BoardView.Check(true);
            }
            //Stalemate
            else if (GamePosition.Stalemate) BoardView.Stalemate();

            if (Global.USE_TEST)
            {
                if (PlayerWaiting is EnginePlayer)
                {
                    EnginePlayer EP = (EnginePlayer)(PlayerWaiting);
                    Test.ShowStats("Time: " + EP.TEST_ThinkTime + "\n");
                    Test.ShowStats("Total nodes considered:    " + EP.TEST_Nodes + " (" + Math.Round(EP.TEST_Nodes / EP.TEST_ThinkTime.TotalSeconds) + " n/s)\n");
                    Test.ShowStats("Total positions evaluated: " + EP.TEST_Evaluated.ToString() + " (" + Math.Round(EP.TEST_Evaluated/EP.TEST_ThinkTime.TotalSeconds) + " n/s)\n");
                    Test.ShowStats("LegalMoves calculated " + EP.TEST_LegalMovesCallCount.ToString() + " times\n");
                    Test.ShowStats("Attacks called " + EP.TEST_AttacksCallCount.ToString() + " times\n");
                    Test.ShowStats("\n");
                }
            }

            int WhiteMaterialAdvantage = CountMaterial();
            BoardView.SetMaterial(WhiteMaterialAdvantage, -WhiteMaterialAdvantage);
        }
        private int CountMaterial()
        {
            int[] Res = new int[2] { 0, 0 };
            for (int Color = PerfectChess.Color.White; Color <= PerfectChess.Color.Black; Color++)
            {
                Res[Color] += BitOperations.PopCount(GamePosition.PieceBitboard[Piece.Pawn | Color]);
                Res[Color] += 3 * BitOperations.PopCount(GamePosition.PieceBitboard[Piece.Knight | Color]);
                Res[Color] += 3 * BitOperations.PopCount(GamePosition.PieceBitboard[Piece.Bishop | Color]);
                Res[Color] += 5 * BitOperations.PopCount(GamePosition.PieceBitboard[Piece.Rook | Color]);
                Res[Color] += 9 * BitOperations.PopCount(GamePosition.PieceBitboard[Piece.Queen | Color]);
            }
            return Res[0] - Res[1];
        }

        private void BoardView_AskForUndo(object sender, EventArgs e)
        {
            if (PlayerToMove.IsThinking) return;
            if (GamePosition.GameFinished) return;

            int HowManyHalfMoves = (PlayerWaiting is EnginePlayer) ? 2 : 1;
            for (int i = 0; i < HowManyHalfMoves; i++)
            {
                int? MoveToUndo = GamePosition.LastMove;
                if (MoveToUndo == null)
                {
                    //No moves handler
                    return;
                }
                GamePosition.UnMake();
                BoardView.UndoMove((int)MoveToUndo);
            }

            int WhiteMaterialAdvantage = CountMaterial();
            BoardView.SetMaterial(WhiteMaterialAdvantage, -WhiteMaterialAdvantage);
        }


        private Player PlayerWhite;
        private Player PlayerBlack;
        private Position GamePosition;
        private Form1 BoardView;

        private Player PlayerToMove => (this.GamePosition.ColorToMove == Color.White) ? this.PlayerWhite : this.PlayerBlack;
        private Player PlayerWaiting => (this.GamePosition.ColorToMove == Color.Black) ? this.PlayerWhite : this.PlayerBlack;

        private TestForm Test = new TestForm();
    }
}
