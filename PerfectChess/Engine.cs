using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PerfectChess.Color;
using static PerfectChess.Piece;

namespace PerfectChess
{
    class Engine
    {
        private Position Pos;
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
        private int Evaluate()
        {
            int[] value = new int[2];
            for (int Color = White; Color <= Black; Color++)
            {
                value[Color] += EngineConsts.PawnCost * BitOperations.PopCount(Pos.GetPieces(Piece.Pawn | Color));
                UInt64 pawnBitboard = Pos.PieceBitboard[Pawn | Color];
                while (pawnBitboard != 0)
                {
                    int Index = BitOperations.PopLS(ref pawnBitboard);
                    value[Color] += Evaluation.PawnPositionTable[Color][Index];
                }


                value[Color] += EngineConsts.KnightCost * BitOperations.PopCount(Pos.GetPieces(Piece.Knight | Color));
                value[Color] += EngineConsts.BishopCost * BitOperations.PopCount(Pos.GetPieces(Piece.Bishop | Color));
                value[Color] += EngineConsts.RookCost * BitOperations.PopCount(Pos.GetPieces(Piece.Rook | Color));
                value[Color] += EngineConsts.QueenCost * BitOperations.PopCount(Pos.GetPieces(Piece.Queen | Color));
                value[Color] += EngineConsts.KingCost * BitOperations.PopCount(Pos.GetPieces(Piece.King | Color));
                if (Pos.CastleLongIndex[Color] != 1 && Pos.CastleShortIndex[Color] != 1)
                    value[Color] -= EngineConsts.CastleAbilityCost;
            }

            return value[White] - value[Black];//WhiteMaterial - BlackMaterial;
        }

        public bool IsThinking { get; private set; }
        public int BestMove(Position P)
        {
            this.IsThinking = true;
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
                    if (alpha <= newalpha)
                    {
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
                    if (alpha >= newalpha)
                    {
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
            }
            catch { this.IsThinking = false; return -1; /*throw*/ }

            this.IsThinking = false;
            return bestmove;
        }
        Random R = new Random();
    }

    static class EngineConsts
    {
        public const int PawnCost = 100;
        public const int KnightCost = 300;
        public const int BishopCost = 325;
        public const int RookCost = 500;
        public const int QueenCost = 900;
        public const int KingCost = 0;

        public const int CastleAbilityCost = 50;
    }    
}
