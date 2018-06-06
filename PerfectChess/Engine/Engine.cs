using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Color;
using static PerfectChess.Piece;

namespace PerfectChess
{
    public partial class Engine
    {
        public Engine()
        {
            //Test.Show();
        }

        /// <summary>
        /// Current position engine analyzes
        /// </summary>
        private Position Pos;

        /// <summary>
        /// Used for avoiding repeating the same moves each game
        /// </summary>
        private Random R = new Random();

        /// <summary>
        /// Returns best move in given position
        /// If no moves are possible, returns special value
        /// </summary>
        /// <param name="PositionToAnalyze">Position to analyze</param>
        /// <returns></returns>
        public int BestMove(Position PositionToAnalyze)
        {
            //Meta-data and testing set-up
            TEST_Nodes = 0;
            TEST_Evaluated = 0;
            this.IsThinking = true;
            DateTime TimeBefore = DateTime.Now;
            MovesWithScore = new Dictionary<int, List<int>>();

            //Finding the best move using Alpha-Beta approach
            int Depth = Engine.Depth;
            int Res = BestMove_AlphaBetaApproach(PositionToAnalyze, Depth);

            //Meta-data and testing set-up
            DateTime TimeAfter = DateTime.Now;
            this.IsThinking = false;
            TEST_ThinkTime = TimeAfter - TimeBefore;

            return Res;
        }

        /// <summary>
        /// Returns if engine is evaluating position at the moment
        /// </summary>
        public bool IsThinking { get; private set; }

        /// <summary>
        /// Returns the best move in position or special value if no moves, using Alpha-Beta pruning approach and looking until given depth
        /// </summary>
        /// <param name="PositionToAnalyze">Position to analyze</param>
        /// <param name="Depth">Depth to search to</param>
        /// <returns></returns>
        private int BestMove_AlphaBetaApproach(Position PositionToAnalyze, int Depth)
        {
            //Copying the position (so that sender can use it while we're busy with evaluating)
            this.Pos = PositionToAnalyze.DeepCopy();

            //Early evaluation finish in case of fifty-move rule
            if (Pos.MovesFiftyRuleCount >= 50) return NoMovesDrawFiftyRule;

            int alpha = Evaluation.Min;
            int beta = Evaluation.Max;

            if (Pos.ColorToMove == White)
            {
                foreach (int Move in SortedMoves(Pos))
                {
                    Pos.Make(Move);
                    //Pruning happens with taking into account rating deviation for move randomisitation
                    //-1 so that positions that are just as good as the current alpha are not pruned
                    int score = AlphaBetaMin(Depth - 1, alpha - 1 - RandomisationMaxDifference, beta);
                    Pos.UnMake();

                    if (score > alpha) alpha = score;
                    AddMove(score, Move);
                }


                //No moves
                if (!MovesWithScore.Keys.Contains(alpha))
                {
                    return NO_MOVES;
                    //switch (Pos.Status)
                    //{
                    //    case Position.GameResult.BlackWinCheckmate:
                    //    case Position.GameResult.WhiteWinCheckmate:
                    //        return NoMovesCheckmate;
                    //    case Position.GameResult.DrawStalemate
                    //        return 
                    //}
                }
                else
                {
                    //Getting all the potential moves - moves with score which suits us
                    IEnumerable<int> PotentialMoves = Enumerable.Empty<int>();
                    foreach (int Key in MovesWithScore.Keys.OrderBy(key => key).Where(key => ((key >= alpha - RandomisationMaxDifference) && key >= 0 && key <= Evaluation.CheckmateCost - 1000) || key == alpha))
                        PotentialMoves = PotentialMoves.Concat(MovesWithScore[Key]);

                    List<int> ListMoves = PotentialMoves.ToList();
                    return ListMoves[R.Next(ListMoves.Count)];
                    //bestmove = BestMoves[alpha][R.Next(BestMoves[alpha].Count())];
                }
            }
            else
            {
                foreach (int Move in SortedMoves(Pos))
                {
                    Pos.Make(Move);
                    //Pruning happens with taking into account rating deviation for move randomisitation
                    //+1 so that positions that are just as good as the current beta are not pruned
                    int score = AlphaBetaMax(Depth - 1, alpha, beta + 1 + RandomisationMaxDifference);
                    Pos.UnMake();

                    if (score < beta) beta = score;
                    AddMove(score, Move);
                }




                //No moves
                if (!MovesWithScore.Keys.Contains(beta)) return NO_MOVES;
                else
                {
                    IEnumerable<int> PotentialMoves = Enumerable.Empty<int>();
                    foreach (int Key in MovesWithScore.Keys.Where(key => ((key <= beta + RandomisationMaxDifference) && key <= 0 && key >= -Evaluation.CheckmateCost + 1000) || key == beta))
                        PotentialMoves = PotentialMoves.Concat(MovesWithScore[Key]);

                    List<int> ListMoves = PotentialMoves.ToList();
                    return ListMoves[R.Next(ListMoves.Count)];
                    //bestmove = BestMoves[beta][R.Next(BestMoves[beta].Count())];
                    ///Test.ShowStats("\nFinal Best: " + Move.Details(bestmove) + " (" + beta + ")" + "\n");
                    ///string other = String.Empty;
                    ///other += "Randomly choosed from (" + BestMoves[beta].Count + "): \n";
                    ///foreach (int move in BestMoves[beta])
                    ///    other += "  " + Move.Details(move) + "\n";
                    ///Test.ShowStats(other);
                }
            }
        }

        //Dictionary to store all the moves with it's evaluation as a key
        private Dictionary<int, List<int>> MovesWithScore = new Dictionary<int, List<int>>();
        private void AddMove(int Score, int Move)
        {
            if (MovesWithScore.Keys.Contains(Score)) MovesWithScore[Score].Add(Move);
            else MovesWithScore.Add(Score, new List<int> { Move });
        }



        private int Evaluate()
        {
            TEST_Evaluated++;
            int[] value = new int[2];
            for (int Color = White; Color <= Black; Color++)
            {
                //Pawns
                bool EndingPawns = (Pos.PieceBitboard[Queen | Black] | Pos.PieceBitboard[Queen | White]) == 0;
                int[] PawnPositionTable = EndingPawns ? Evaluation.PawnPositionTableEnding[Color] : Evaluation.PawnPositionTable[Color];
                UInt64 pawnBitboard = Pos.PieceBitboard[Pawn | Color];
                while (pawnBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref pawnBitboard);
                    value[Color] += Evaluation.PawnCost + PawnPositionTable[Index];
                }

                //Knights
                UInt64 knightBitboard = Pos.PieceBitboard[Knight | Color];
                while (knightBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref knightBitboard);
                    value[Color] += Evaluation.KnightCost + Evaluation.KnightPositionTable[Color][Index];
                }

                //Bishops
                UInt64 bishopBitboard = Pos.PieceBitboard[Bishop | Color];
                while (bishopBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref bishopBitboard);
                    value[Color] += Evaluation.BishopCost + Evaluation.BishopPositionTable[Color][Index];
                }

                //Rooks
                UInt64 rookBitboard = Pos.PieceBitboard[Rook | Color];
                while (rookBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref rookBitboard);
                    value[Color] += Evaluation.RookCost + Evaluation.RookPositionTable[Color][Index];
                }


                value[Color] += Evaluation.QueenCost * BitOperations.PopCount(Pos.PieceBitboard[Queen | Color]);


                //Kings
                bool Ending = ((Pos.PieceBitboard[Queen | Black] | Pos.PieceBitboard[Queen | White]) == 0);
                int[] PositionTable = Ending ? Evaluation.KingPositionEnding[Color] : Evaluation.KingPositionOpening[Color];
                UInt64 kingBitboard = Pos.PieceBitboard[King | Color];
                while (kingBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref kingBitboard);
                    value[Color] += Evaluation.KingCost + PositionTable[Index];
                }
            }

            return value[White] - value[Black];
        }

        #region Minimax
        private int BestMoveMiniMaxApproach(Position P)
        {
            this.Pos = P.DeepCopy();
            int depth = 3;

            int alpha = (Pos.ColorToMove == White) ? int.MinValue : int.MaxValue;
            int bestmove = -1;
            int bestalpha = alpha;
            R = new Random();

            Dictionary<int, List<int>> BestMoves = new Dictionary<int, List<int>>();
            foreach (int move in Pos.LegalMoves())
            {
                if (Pos.ColorToMove == White)
                {
                    Pos.Make(move);

                    int newalpha = MiniMax(depth, Black);
                    //Test.ShowStats("\n" + Move.Details(move) + " — " + newalpha + " (best: " + alpha + ")");
                    if (alpha <= newalpha)
                    {
                        //Test.ShowStats(" — New Best!");
                        bestmove = move;
                        if (BestMoves.Keys.Contains(newalpha)) BestMoves[newalpha].Add(move);
                        else BestMoves.Add(newalpha, new List<int> { move });
                        alpha = newalpha;
                    }
                    Pos.UnMake();
                }
                else if (Pos.ColorToMove == Black)
                {
                    Pos.Make(move);

                    int newalpha = MiniMax(depth, White);
                    //Test.ShowStats("\n" + Move.Details(move) + " — " + newalpha + " (best: " + alpha + ")");
                    if (alpha >= newalpha)
                    {
                        //Test.ShowStats(" — New Best!");
                        bestmove = move;
                        if (BestMoves.Keys.Contains(newalpha)) BestMoves[newalpha].Add(move);
                        else BestMoves.Add(newalpha, new List<int> { move });
                        alpha = newalpha;
                    }
                    Pos.UnMake();
                }
            }

            try
            {
                bestmove = BestMoves[alpha][R.Next(BestMoves[alpha].Count)];
                //Test.ShowStats("\nFinal Best: " + Move.Details(bestmove) + " (" + alpha + ")");

                //string other = String.Empty;
                //other += "Randomly choosed from (" + BestMoves[alpha].Count + "): \n";
                //foreach (int move in BestMoves[alpha])
                //    other += "  " + Move.Details(move) + "\n";
                //Test.ShowStats(other);
            }
            catch { this.IsThinking = false; return -1; /*throw*/ }

            this.IsThinking = false;
            
            return bestmove;
        }
        private int MiniMax(int depth, int Player)
        {
            TEST_Nodes++;
            if (depth <= 0) return Evaluate();
            //int alpha = int.MinValue;
            int alpha = (Pos.ColorToMove == White) ? int.MinValue : int.MaxValue;
            int alphasaved = alpha;
            foreach (int move in Pos.LegalMoves())
            {
                if (Player == White)
                {
                    Pos.Make(move);
                    alpha = Math.Max(alpha, MiniMax(depth - 1, Black));
                    Pos.UnMake();
                }
                else if (Player == Black)
                {
                    Pos.Make(move);
                    alpha = Math.Min(alpha, MiniMax(depth - 1, White));
                    Pos.UnMake();
                }
            }

            //No moves there
            if (alpha == alphasaved)
            {
                //Checkmate or Stalemate
                if (Pos.IsInCheck(Pos.ColorToMove))
                    return (Pos.ColorToMove == Black) ? 100000 : -100000;
                else return 0;
            }

            return alpha;
        }
        #endregion


        public const int NO_MOVES = -1;
        public const int NoMovesStalemate = -5;
        public const int NoMovesCheckmate = -2;
        public const int NoMovesDrawFiftyRule = -3;

        private int AlphaBetaMax(int Depth, int alpha, int beta)
        {
            TEST_Nodes++;
            if (Pos.MovesFiftyRuleCount >= 50) return 0;
            if (Depth == 0) return Evaluate();


            bool Moves = false;
            foreach (int Move in SortedMoves(Pos))//Pos.LegalMoves())
            {
                Moves = true;
                Pos.Make(Move);
                int score = AlphaBetaMin(Depth - 1, alpha, beta);
                Pos.UnMake();

                //We've found the move for white which gives them position better or equal to one they could at best get on the other black's move
                if (score >= beta) return beta;
                if (score > alpha) alpha = score;
            }
            if (!Moves)
            {
                //Checkmate or Stalemate
                if (Pos.IsInCheck(Pos.ColorToMove))
                    return (Pos.ColorToMove == Black) ? Evaluation.CheckmateCost - (Engine.Depth - Depth) : -Evaluation.CheckmateCost + (Engine.Depth - Depth);
                else return 0;
            }
            return alpha;
        }
        private int AlphaBetaMin(int Depth, int alpha, int beta)
        {
            TEST_Nodes++;
            if (Pos.MovesFiftyRuleCount >= 50) return 0;
            if (Depth == 0) return Evaluate();

            bool Moves = false;
            foreach (int Move in SortedMoves(Pos))
            {
                Moves = true;
                Pos.Make(Move);
                int score = AlphaBetaMax(Depth - 1, alpha, beta);
                Pos.UnMake();
                if (score <= alpha) return alpha;
                if (score < beta) beta = score;
            }

            if (!Moves)
            {
                //Checkmate or Stalemate
                if (Pos.IsInCheck(Pos.ColorToMove))
                    return (Pos.ColorToMove == Black) ? Evaluation.CheckmateCost - (Engine.Depth - Depth) : -Evaluation.CheckmateCost + (Engine.Depth - Depth);
                else return 0;
            }


            return beta;
        }

        private IEnumerable<int> SortedMoves(Position P)
        {
            int[] Moves = P.LegalMoves().ToArray();
            if (!Moves.Any()) return Moves;
            int[] MovesRating = new int[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
                MovesRating[i] = MoveRating(Moves[i], P);

            SortMoves(Moves, MovesRating, 0);
            return Moves;
        }
        private void SortMoves(int[] Moves, int[] Values, int Count)
        {
            Array.Sort(Values, Moves, Comparer<int>.Create(new Comparison<int>((a, b) => (a <= b) ? 1 : -1)));
        }

        private int MoveRating(int M, Position P)
        {
            //Good captures first
            if (Move.ToPiece(M) != 0)
            {
                int LVA_MVV = Move.ToPiece(M) & Piece.Mask - Move.FromPiece(M) & Piece.Mask;
                if (LVA_MVV >= 0)
                {
                    if (P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M)) return (LVA_MVV + 2) * 5 + 200;
                    return (LVA_MVV + 2) * 5;
                }
                return (LVA_MVV - 2) * 5;
            }
            return 0;
        }




        #region TESTING
        public int TEST_Evaluated = 0;
        public TimeSpan TEST_ThinkTime = new TimeSpan();
        public int TEST_LegalMovesCallCount => Pos.TEST_LegalMovesCallCount;
        public int TEST_AttacksCallCount => Pos.TEST_AttacksCallCount;
        public int TEST_Nodes = 0;
        #endregion
    }
}
