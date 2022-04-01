using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameKe
{
    public class Game
    {
        public GameStatus GameStatus { get; set; } = GameStatus.READY;
        public int CurrentPlayerNo { get; internal set; }
        public List<Player> Players { get; }


        public Game(int noOfPlayers)
        {
            Logger.LogInfo("Starting Game...");
            if (noOfPlayers <= 0)
            {
                Logger.LogWarning($"No Players can't be {noOfPlayers}, Game Terminated");
                return;
            }

            Logger.LogInfo("Generating Players...");
            Players = new List<Player>();
            for (int i = 0; i < noOfPlayers; i++)
                Players.Add(new Player(this, i + 1, new List<Card> { new Card { CardIdentity = CardIdentity.No4, CardIdentityType = CardIdentityType.Heart } }));

        }

        public void StartGame()
        {
            Logger.LogInfo("Starting Game with Player 1...");
            CurrentPlayerNo = 1;
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
        }
        public void StopGame()
        {
            Logger.LogInfo("Stopping Game...");
            CurrentPlayerNo = 0;
            GameStatus = GameStatus.CLOSED;
        }
        public void PlaceCard(int playerNo, Card card)
        {
            GameStatus = GameStatus.OPEN;
            Logger.LogInfo($"Card [{card.CardIdentity} {card.CardIdentityType}] Placed on Deck");
            CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            Logger.LogWarning($"Next Player is: PLAYER-{CurrentPlayerNo}");
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
        }

        public Card PickCard(int playerNo)
        {
            GameStatus = GameStatus.OPEN;
            CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            Logger.LogWarning($"Next Player is: PLAYER-{CurrentPlayerNo}");
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
            return new Card { CardIdentity = CardIdentity.No9, CardIdentityType = CardIdentityType.Spade };
        }
    }
    public enum GameStatus
    {
        READY,
        CLOSED,
        OPEN,
        CARDSHUFFLING,
        WAITINGPLAYERSCARD
    }
}
