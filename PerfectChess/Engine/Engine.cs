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
        private Position Pos;
        private Random R = new Random();
        private TestForm Test = new TestForm();

        public int BestMove(Position P)
        {
            //Testing
            //if (Test != null) Test.Show();
            //Test.Reset();

            this.IsThinking = true;
            int Res = BestMoveAlphaBetaApproach(P);
            this.IsThinking = false;
            return Res;
        }
        public bool IsThinking { get; private set; }

        private int Evaluate()
        {
            int[] value = new int[2];
            for (int Color = White; Color <= Black; Color++)
            {
                //Pawns
                UInt64 pawnBitboard = Pos.PieceBitboard[Pawn | Color];
                while (pawnBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref pawnBitboard);
                    value[Color] += Evaluation.PawnCost + Evaluation.PawnPositionTable[Color][Index];
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
                bool Ending = (Pos.PieceBitboard[Queen] == 0);
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

        private int BestMoveAlphaBetaApproach(Position P)
        {
            this.Pos = P.DeepCopy();
            int Depth = Engine.Depth;

            int alpha = Evaluation.Min;
            int beta = Evaluation.Max;
            int bestmove;

            //Dictionary used to store all the moves with it's evaluation as a key
            Dictionary<int, List<int>> BestMoves = new Dictionary<int, List<int>>();

            if (Pos.ColorToMove == White)
            {
                foreach (int Move in SortMoves(Pos))
                {
                    Pos.Make(Move);
                    int score = AlphaBetaMin(Depth - 1, alpha - 1 - RandomisationMaxDifference, beta);
                    Pos.UnMake();

                    ///Test.ShowStats("\n" + PerfectChess.Move.Details(Move) + " — " + score + " (best: " + alpha + ")");
                    if (score > alpha)
                    {
                        ///Test.ShowStats(" — New Best!");
                        alpha = score;
                    }
                    if (BestMoves.Keys.Contains(score)) BestMoves[score].Add(Move);
                    else BestMoves.Add(score, new List<int> { Move });
                }
                //No moves
                if (!BestMoves.Keys.Contains(alpha)) bestmove = -1;
                else
                {
                    IEnumerable<int> PotentialMoves = Enumerable.Empty<int>();
                    foreach (int Key in BestMoves.Keys.Where(key => ((key >= alpha - RandomisationMaxDifference) && key >= 0 && key <= Evaluation.CheckmateCost - 1000) || key == alpha))
                        PotentialMoves = PotentialMoves.Concat(BestMoves[Key]);

                    List<int> ListMoves = PotentialMoves.ToList();
                    bestmove = ListMoves[R.Next(ListMoves.Count)];
                    //bestmove = BestMoves[alpha][R.Next(BestMoves[alpha].Count())];
                }
            }
            else
            {
                foreach (int Move in SortMoves(Pos))
                {
                    Pos.Make(Move);
                    //+1 so that positions that are just as good as the current beta are counted to random generator
                    int score = AlphaBetaMax(Depth - 1, alpha, beta + 1 + RandomisationMaxDifference);
                    Pos.UnMake();

                    ///Test.ShowStats("\n" + PerfectChess.Move.Details(Move) + " — " + score + " (best: " + beta + ")");
                    if (score < beta)
                    {
                        ///Test.ShowStats(" — New Best!");
                        beta = score;
                    }
                    if (BestMoves.Keys.Contains(score)) BestMoves[score].Add(Move);
                    else BestMoves.Add(score, new List<int> { Move });
                }




                //No moves
                if (!BestMoves.Keys.Contains(beta)) bestmove = -1;
                else
                {
                    IEnumerable<int> PotentialMoves = Enumerable.Empty<int>();
                    foreach (int Key in BestMoves.Keys.Where(key => ((key <= beta + RandomisationMaxDifference) && key <= 0 && key >= -Evaluation.CheckmateCost + 1000) || key == beta))
                        PotentialMoves = PotentialMoves.Concat(BestMoves[Key]);

                    List<int> ListMoves = PotentialMoves.ToList();
                    bestmove = ListMoves[R.Next(ListMoves.Count)];
                    //bestmove = BestMoves[beta][R.Next(BestMoves[beta].Count())];
                    ///Test.ShowStats("\nFinal Best: " + Move.Details(bestmove) + " (" + beta + ")" + "\n");
                    ///string other = String.Empty;
                    ///other += "Randomly choosed from (" + BestMoves[beta].Count + "): \n";
                    ///foreach (int move in BestMoves[beta])
                    ///    other += "  " + Move.Details(move) + "\n";
                    ///Test.ShowStats(other);
                }
            }
            
            return bestmove;
        }



        private int AlphaBetaMax(int Depth, int alpha, int beta)
        {
            if (Depth == 0) return Evaluate();
            bool Moves = false;
            foreach (int Move in SortMoves(Pos))//Pos.LegalMoves())
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
            if (Depth == 0) return Evaluate();

            bool Moves = false;
            foreach (int Move in SortMoves(Pos))
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

        private IEnumerable<int> SortMoves(Position P)
        {
            //List<int> SortedMoves = P.LegalMoves();
            int[] Moves = P.LegalMoves().ToArray();
            int[] MovesRating = new int[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                MovesRating[i] = MoveRating(Moves[i], P);
            }
            Array.Sort(MovesRating, Moves);
            //List<int> MovesRating = new List<int>
            //foreach (int Move in SortedMoves)
            //{
            //
            //    SortedMoves.Sort()
            //    SortedMoves.Insert()
            //}
            //return SortedMoves;
            return Moves.Reverse();
        }
        private IEnumerable<int> FirstPlySortMoves(Position P)
        {
            int[] Moves = P.LegalMoves().ToArray();
            int[] MovesRating = new int[Moves.Length];
            for (int i = 0; i < Moves.Length; i++)
            {
                MovesRating[i] = FirstPlyMoveRating(Moves[i], P);
            }
            Array.Sort(MovesRating, Moves);
            return Moves.Reverse();
        }

        /*private class MoveComparer : IComparer<int>
        {
            public MoveComparer(List<int> Moves)
            {
                foreach (int Move in Moves)
                {
                    MovesRating.Add(Move, MoveRating(Move));
                }
            }
            public int Compare(int x, int y)
            {
                MovesRating.
                throw new NotImplementedException();
            }
            Dictionary<int, int> MovesRating = new Dictionary<int, int>();
        }*/
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
                //int LastSquare = ((P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M)) ? LVA_MVV > 0 ? 20 : 2)
                //if (LVA_MVV < 0)
                //{
                //
                //}
            }
            //if (P.Attacks(Move.ToSquare((int)(P.LastMove)), Move.FromSquare(M))) return 5;
            return 0;



            /*int add = 0;
            if (P.Attacks(Move.ToSquare((int)(P.LastMove)), Move.FromSquare(M))) add += 2;
            //if (P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M))  
            //Encourage capturing, capturing piece should be smaller, captured - bigger
            if (Move.ToPiece(M) != 0)
            {
                return Move.ToPiece(M) & Piece.Mask - Move.FromPiece(M) & Piece.Mask + 1 + 
                        ((P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M)) ? ((Move.ToPiece((int)(P.LastMove)) != 0) ? 20 : 2) : 0) + add;
            }       
            return 0;*/
        }
        private int FirstPlyMoveRating(int M, Position P)
        {
            int addon = 0;
            if (P.AnyMoves && P.Attacks(Move.ToSquare((int)(P.LastMove)), Move.FromSquare(M))) addon = 500;
            if (Move.ToPiece(M) != 0)
            {
                int LVA_MVV = Move.ToPiece(M) & Piece.Mask - Move.FromPiece(M) & Piece.Mask;
                if (LVA_MVV >= 0)
                {
                    if (P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M)) return (LVA_MVV + 2) * 5 + 200 + addon;
                    return (LVA_MVV + 2) * 5 + addon;
                }
                return (LVA_MVV - 2) * 5 + addon;
                //int LastSquare = ((P.AnyMoves && Move.ToSquare((int)(P.LastMove)) == Move.ToSquare(M)) ? LVA_MVV > 0 ? 20 : 2)
                //if (LVA_MVV < 0)
                //{
                //
                //}
            }
            //if (P.Attacks(Move.ToSquare((int)(P.LastMove)), Move.FromSquare(M))) return 5;
            return addon;
        }
    }
}
