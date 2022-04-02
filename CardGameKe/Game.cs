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
        public bool IsGamePlayingForward { get; set; } = true;

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

            //CHECK WINNING || DON'T WIN IF CARDLESS
            if (onCard && cards.CardsCanFinishGameBasedOnCard(this.LastCardOnBoard))
                if (this.Players.Where(x => x.IsOnCard)?.Count() == 0)
                    Logger.LogInfo("Can't Win Game Now, Looks Like there is a Player in Cardless Mode (Try Second Attempt)");
                else
                {
                    foreach (Card card in cards)
                        Logger.LogInfo(MagicCardGen.GetCardImage(card));
                    EndGame(playerNo);
                    return;
                }
            //Proceed
            foreach (Card card in cards)
            {
                this.CurrentStackCardsOnBoard.Add(card);
                //Notify
                Logger.LogInfo(MagicCardGen.GetCardImage(card));
            }
            //CHECK FOR JAMP
            //Reverse Cards
            cards.Reverse();
            //!!KICKBACK FOR K
            List<Card> cardsWithK = cards.Where(x => x.CardIdentity == CardIdentity.King).ToList();
            if (cardsWithK != null && cardsWithK.Count > 0)
                foreach (var kCard in cardsWithK?.Take(4))
                {
                    Logger.LogWarning($"!!KICKBACK INITIATED, OPP DIRECTION PLAY!!");
                    IsGamePlayingForward = !IsGamePlayingForward;
                }

            //NEXT PLAYER
            if (this.IsGamePlayingForward)
                CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
            else
                CurrentPlayerNo = (CurrentPlayerNo - 1 <= 0) ? Players.Count : CurrentPlayerNo += -1;

            //!!JUMP FOR J
            List<Card> cardsWithJ = cards.Where(x => x.CardIdentity == CardIdentity.Jack).ToList();
            if (cardsWithJ != null && cardsWithJ.Count > 0)
                foreach (var jCard in cardsWithJ?.Take(4))
                {
                    Logger.LogWarning($"PLAYER-{CurrentPlayerNo} JUMPED");
                    CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
                }

            Logger.LogWarning(string.Format("Next Player is: PLAYER-{0}", CurrentPlayerNo, this.IsGamePlayingForward ? string.Empty : " {<Active Kick Back>}"));
            GameStatus = GameStatus.WAITINGPLAYERSCARD;
            LastGamePlayAction = LastGamePlayAction.CARDDECKED;
        }

        public List<Card> PickCard(int playerNo, int takeCount = 1, bool moveNext = true)
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
            if (moveNext)
            {
                //NEXT PLAYER
                if (this.IsGamePlayingForward)
                    CurrentPlayerNo = (CurrentPlayerNo + 1 > Players.Count) ? 1 : CurrentPlayerNo += 1;
                else
                    CurrentPlayerNo = (CurrentPlayerNo - 1 <= 0) ? Players.Count : CurrentPlayerNo += -1;
                //Proceed
                Logger.LogWarning(string.Format("Next Player is: PLAYER-{0}", CurrentPlayerNo, this.IsGamePlayingForward ? string.Empty : " {<Active Kick Back>}"));
                LastGamePlayAction = LastGamePlayAction.CARDPICKED;
                GameStatus = GameStatus.WAITINGPLAYERSCARD;
            }
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
