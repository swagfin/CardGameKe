using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardGameKe
{
    public class Player
    {
        public int PlayerNo { get; set; }
        public Game CurrentGame { get; set; }
        public List<Card> CardsOnHand { get; set; } = new List<Card>();
        public bool IsOnCard { get; set; } = false;
        public Player(Game currentGame, int playerNo, List<Card> carsOnHand)
        {
            CurrentGame = currentGame;
            PlayerNo = playerNo;
            CardsOnHand = carsOnHand ?? new List<Card>();
            StartPlaySession();
        }

        private void StartPlaySession()
        {
            var t = new Thread(() =>
            {
                while (CurrentGame.GameStatus != GameStatus.CLOSED)
                {
                    try
                    {
                        if (CurrentGame.GameStatus == GameStatus.WAITINGPLAYERSCARD && CurrentGame.CurrentPlayerNo == this.PlayerNo)
                        {
                            Logger.LogInfo("Playing....", $"PLAYER-{this.PlayerNo:N0} >> ({CardsOnHand.Count:N0} Cards On Hand) <<");
                            Card onDeckCard = this.CurrentGame.LastCardOnBoard;
                            HandleLogicPlayGame(onDeckCard);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }

                    //Sleep
                    Thread.Sleep(new Random().Next(2000, 8000));
                }
            });
            t.Start();
        }


        private void HandleLogicPlayGame(Card onDeckCard)
        {
            if (onDeckCard == null)
                return;
            //Variables
            int cardsToCollect = 0;
            List<Card> cardsToDeck = new List<Card>();
            //Proceed
            if (onDeckCard.CardIdentity == CardIdentity.No2)
            {
                Card returnCard = CardsOnHand.FirstOrDefault(x => x.CardIdentity == CardIdentity.Ace || x.CardIdentity == CardIdentity.No2);
                if (returnCard == null)
                    cardsToCollect += 2;
                else
                {
                    cardsToDeck.Add(returnCard);
                    //Check if we have other existing cards
                    List<Card> otherNo2Cards = CardsOnHand.Where(x => x.CardIdentity == CardIdentity.No2 && x.Id != returnCard.Id).ToList();
                    if (returnCard.CardIdentity == CardIdentity.No2 && otherNo2Cards.Count > 0)
                        cardsToDeck.AddRange(otherNo2Cards);
                }
            }
            else if (onDeckCard.CardIdentity == CardIdentity.No3)
            {
                Card returnCard = CardsOnHand.FirstOrDefault(x => x.CardIdentity == CardIdentity.Ace || x.CardIdentity == CardIdentity.No3);
                if (returnCard == null)
                    cardsToCollect += 3;
                else
                {
                    cardsToDeck.Add(returnCard);
                    //Check if we have other existing cards
                    List<Card> otherNo3Cards = CardsOnHand.Where(x => x.CardIdentity == CardIdentity.No3 && x.Id != returnCard.Id).ToList();
                    if (returnCard.CardIdentity == CardIdentity.No3 && otherNo3Cards.Count > 0)
                        cardsToDeck.AddRange(otherNo3Cards);
                }
            }
            else
            {
                Card placingCard = CardsOnHand.FirstOrDefault(x => x.CardIdentity == onDeckCard.CardIdentity || x.CardIdentityType == onDeckCard.CardIdentityType);
                if (placingCard != null)
                    cardsToDeck.Add(placingCard);
                else
                    cardsToCollect += 1;
            }

            if (cardsToCollect > 0 && cardsToDeck.Count > 0)
                throw new Exception("Oops, Player has Cards to Pick and Cards to Collect, Logic Error");

            if (cardsToCollect == 0 && cardsToDeck.Count == 0)
                throw new Exception("Oops, Player is of Course Nothing to Decide");

            //Check if User can Win Game
            this.IsOnCard = (CardsOnHand.Count == 1 && CardsOnHand.FirstOrDefault(x => SharedLogic.CanStartGamesCards.Contains(x.CardIdentity)) != null);
            //Proceed
            if (cardsToCollect > 0)
            {
                List<Card> cardsPicked = CurrentGame.PickCard(this.PlayerNo, cardsToCollect);
                CardsOnHand.AddRange(cardsPicked);
            }
            else if (cardsToDeck.Count > 0)
            {
                CurrentGame.PlaceCard(this.PlayerNo, cardsToDeck, this.IsOnCard);
                foreach (var cardDecked in cardsToDeck)
                    CardsOnHand.Remove(cardDecked);
            }



        }
    }
}
