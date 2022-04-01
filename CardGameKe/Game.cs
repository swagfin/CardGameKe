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
        public List<Card> CurrentStackCards { get; }

        public Game(int noOfPlayers, int startingNoOfCardsPerPlayer = 4)
        {
            Logger.LogInfo("Starting Game...");
            if (noOfPlayers <= 0 || noOfPlayers > 20)
                throw new Exception("Min Number of Players should more than 1 and less than 20");

            Logger.LogInfo("Generating Players...");
            Players = new List<Player>();
            for (int i = 0; i < noOfPlayers; i++)
                Players.Add(new Player(this, i + 1, new List<Card>()));
            Logger.LogWarning($"TOTAL PLAYERS PLAYING: {noOfPlayers:N0}");
            Logger.LogInfo("Generating Game Cards...");
            this.CurrentStackCards = LogicFunctions.GetStackOfCards();
            this.CurrentStackCards.ShuffleCards();

            Logger.LogInfo("Assigning Game Cards to Players...");
            for (int c = 0; c < startingNoOfCardsPerPlayer; c++)
                foreach (Player player in this.Players)
                {
                    Card playerCard = CurrentStackCards.FirstOrDefault();
                    player.CardsOnHand.Add(playerCard);
                    CurrentStackCards.Remove(playerCard); //Remove Card from Stack
                }
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
            Logger.LogInfo(string.Format(@"
 -----------
|           |
    {0}  
    {1}  
|           |
 -----------", card.CardIdentityType, card.CardIdentity));
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
            Card pickingCard = CurrentStackCards.FirstOrDefault();
            if (pickingCard == null)
            {
                Logger.LogError("BOARD IS OUT OF CARDS >> TERMINATING");
                GameStatus = GameStatus.CLOSED;
            }
            else
                CurrentStackCards.Remove(pickingCard);
            return pickingCard;
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
