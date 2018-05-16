using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectChess
{
    public class Game
    {
        public Game()
        {

        }
        public Game(IPlayer PlayerWhite, IPlayer PlayerBlack)
        {
            this.GamePosition = new Position();
            this.PlayerWhite = PlayerWhite;
            this.PlayerBlack = PlayerBlack;

            this.PlayerWhite.MovePlayed += Player_MovePlayed;
            this.PlayerBlack.MovePlayed += Player_MovePlayed;
        }
        public Game(IPlayer PlayerWhite, IPlayer PlayerBlack, Position StartPosition)
        {
            this.GamePosition = StartPosition;
            this.PlayerWhite = PlayerWhite;
            this.PlayerBlack = PlayerBlack;

            this.PlayerWhite.MovePlayed += Player_MovePlayed;
            this.PlayerBlack.MovePlayed += Player_MovePlayed;
        }

        private Position GamePosition;
        private IPlayer PlayerWhite;
        private IPlayer PlayerBlack;
        private IPlayer PlayerToMove => (this.GamePosition.ColorToMove == Color.White) ? this.PlayerWhite : this.PlayerBlack;


        private void Player_MovePlayed(object sender, int MovePlayed)
        {
            if (!(sender is IPlayer)) throw new Exception("Sender was not a player");
            IPlayer Player = sender as IPlayer;

            if (IsLegal(MovePlayed))
            {
                GamePosition.Make(MovePlayed);
                PlayerToMove.YourMove(GamePosition.DeepCopy());
            }
            else
            {
                Player.WarnIllegal(MovePlayed);
            }
        }
        private bool IsLegal(int Move)
        {
            return GamePosition.LegalMoves().Contains(Move);
        }        


    }

    public interface IPlayer
    {
        void MakeMove(int Move);
        Stack<int> PreMoves { get; }


        event EventHandler<int> MovePlayed;
        void YourMove(Position P);
        void WarnIllegal(int Move);

        bool Human { get; }
    }
}
