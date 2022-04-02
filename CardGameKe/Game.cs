using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGameKe
{
    public class Game
    {
        public GameStatus GameStatus { get; set; } = GameStatus.READY;
        public int CurrentPlayerNo { get; internal set; }
        public List<Player> Players { get; }
        public List<Card> CurrentStackCardsAvailable { get; } = new List<Card>();
        public List<Card> CurrentStackCardsOnBoard { get; } = new List<Card>();
        public Card LastCardOnBoard
        {
            get { return CurrentStackCardsOnBoard.LastOrDefault(); }
        }
        public LastGamePlayAction LastGamePlayAction { get; set; }

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
            this.CurrentStackCardsAvailable = SharedLogic.GetStackOfCards();
            this.CurrentStackCardsAvailable.ShuffleCards();
            this.CurrentStackCardsOnBoard = new List<Card>(); //reset Board

            Logger.LogInfo("Assigning Game Cards to Players...");
            for (int c = 0; c < startingNoOfCardsPerPlayer; c++)
                foreach (Player player in this.Players)
                {
                    Card playerCard = CurrentStackCardsAvailable.FirstOrDefault();
                    player.CardsOnHand.Add(playerCard);
                    CurrentStackCardsAvailable.Remove(playerCard); //Remove Card from Stack
                }
        }

        public void StartGame()
        {
            Logger.LogInfo("Finding Starting Card...");

            Card startingCard = this.CurrentStackCardsAvailable.FirstOrDefault(x => SharedLogic.CanStartGamesCards.Contains(x.CardIdentity));
            if (startingCard == null)
                throw new Exception("Starting Card was not Found from Stack");
            //Add Cards
            CurrentStackCardsAvailable.Remove(startingCard);
            CurrentStackCardsOnBoard.Add(startingCard);
            Logger.LogInfo(string.Format(@"
| START CARD |
    {0}    
", MagicCardGen.GetCardImage(startingCard)));

            Logger.LogInfo("Starting Game with Player 1...");
            CurrentPlayerNo = 1;
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
            LastGamePlayAction = LastGamePlayAction.CARDDECKED;
        }
        public void StopGame()
        {
            Logger.LogInfo("Stopping Game...");
            CurrentPlayerNo = 0;
            GameStatus = GameStatus.CLOSED;
        }
        public void PlaceCard(int playerNo, List<Card> cards, bool onCard = false)
        {
            if (cards == null || cards.Count == 0)
                throw new Exception("Null cards Placed");
            GameStatus = GameStatus.OPEN;

            foreach (Card card in cards)
            {
                this.CurrentStackCardsOnBoard.Add(card);
                //Notify
                Logger.LogInfo(MagicCardGen.GetCardImage(card));
            }
            //CHECK PLAYER
            if ((onCard && this.LastCardOnBoard?.CardIdentity == cards[0].CardIdentity) || (onCard && this.LastCardOnBoard?.CardIdentityType == cards[0].CardIdentityType))
            {
                EndGame(playerNo);
                return;
            }
            CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            //CHECK FOR JAMP
            if (this.LastCardOnBoard?.CardIdentity == CardIdentity.Jack && this.LastGamePlayAction == LastGamePlayAction.CARDDECKED)
            {
                Logger.LogWarning($"PLAYER-{CurrentPlayerNo} JUMPED");
                CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            }
            Logger.LogWarning($"Next Player is: PLAYER-{CurrentPlayerNo}");
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
            LastGamePlayAction = LastGamePlayAction.CARDDECKED;
        }

        public List<Card> PickCard(int playerNo, int takeCount = 1)
        {
            if (takeCount <= 0 || takeCount > 10)
                throw new Exception("Min Number of Take Count should more than 1 and less than 10");
            Logger.LogInfo($"----- Player-{playerNo} Picking ({takeCount:N0}) Card(s) ------ :-(");
            List<Card> pickingCards = CurrentStackCardsAvailable.Take(takeCount).ToList();
            if (pickingCards == null || pickingCards.Count < takeCount)
            {
                Logger.LogWarning("BOARD IS OUT OF CARDS >> RESHUFFLING CARDS ON BOARD");
                GameStatus = GameStatus.CARDSHUFFLING;
                //Logic
                //Last Card
                Card lstCard = LastCardOnBoard;
                List<Card> rmCards = CurrentStackCardsOnBoard.Where(x => x.Id != lstCard.Id).ToList();
                CurrentStackCardsAvailable.AddRange(rmCards);
                CurrentStackCardsOnBoard.Clear();
                CurrentStackCardsOnBoard.Add(lstCard);
                CurrentStackCardsAvailable.ShuffleCards();
                Logger.LogInfo(string.Format(@"
|  TOP CARD |
    {0}    
|A. SHUFFLE|
 -----------", MagicCardGen.GetCardImage(lstCard)));
                //Proceed
                pickingCards = CurrentStackCardsAvailable.Take(takeCount).ToList();
                Logger.LogWarning("RESHUFFLING COMPLETED");

            }
            //Removed all Picked Cards from Board
            if (pickingCards != null && pickingCards.Count > 0)
                foreach (Card rec in pickingCards)
                    CurrentStackCardsAvailable.Remove(rec);

            GameStatus = GameStatus.OPEN;
            CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            Logger.LogWarning($"Next Player is: PLAYER-{CurrentPlayerNo}");
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
            LastGamePlayAction = LastGamePlayAction.CARDPICKED;
            return pickingCards;
        }

        private void EndGame(int playerNo)
        {
            Logger.LogWarning(string.Format(@"
 -----------------
           
 //////   PLAYER \\\\\\  
           [{0:N0}] 
        WON THE GAME
 
------------------------", playerNo));
            StopGame();
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
    public enum LastGamePlayAction
    {
        CARDDECKED,
        CARDPICKED,
    }
}
