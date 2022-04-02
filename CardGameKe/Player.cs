using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
                            string isOnCardNotice = (this.IsOnCard) ? "=== [ !! ON CARD !!] ===" : string.Empty;
                            Logger.LogInfo("Playing....", $"PLAYER-{this.PlayerNo:N0} {isOnCardNotice} >> ({CardsOnHand.Count:N0} Cards On Hand) <<");
                            Card onDeckCard = this.CurrentGame.LastCardOnBoard;
                            if (this.PlayerNo == 1)
                                HandleOwnersGamePlay(onDeckCard);
                            else
                                HandleLogicPlayGame(onDeckCard);

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }

                    //Sleep
                    if (this.PlayerNo == 1)
                        Thread.Sleep(1000);
                    else
                        Thread.Sleep(new Random().Next(2000, 8000));
                }
            });
            t.Start();
        }

        private void HandleOwnersGamePlay(Card onDeckCard)
        {
            Logger.LogInfo("Its your Turn to Play;\n");
            int startAt = 0;
            foreach (var card in CardsOnHand)
            {
                startAt++;
                Logger.LogInfo($"{startAt}. {card.CardIdentity} | {card.CardIdentityType}");
            }
            startAt++;
            Logger.LogInfo($"{startAt}. PICK CARD");
            if (!int.TryParse(Console.ReadLine().Trim(), out int selectedVal) || selectedVal > startAt)
                throw new Exception("Player selected value  was invalid...");
            if (selectedVal == startAt)
            {
                List<Card> cardsPicked = CurrentGame.PickCard(this.PlayerNo, 1);
                CardsOnHand.AddRange(cardsPicked);
            }
            else
            {
                Card selectedCard = CardsOnHand[selectedVal - 1];
                CurrentGame.PlaceCard(this.PlayerNo, new List<Card> { selectedCard }, this.IsOnCard);
                CardsOnHand.Remove(selectedCard);
            }
            this.IsOnCard = (CardsOnHand.Count == 1 && CardsOnHand.FirstOrDefault(x => SharedLogic.CanStartGamesCards.Contains(x.CardIdentity)) != null);
            if (this.IsOnCard)
            {
                Logger.LogWarning($"PLAYER {this.PlayerNo} ON CARD");
            }
        }

        private void HandleLogicPlayGame(Card onDeckCard)
        {
            if (onDeckCard == null)
                return;
            //Variables
            int cardsToCollect = 0;
            List<Card> cardsToDeck = new List<Card>();
            //Proceed
            if (onDeckCard.CardIdentity == CardIdentity.No2 && CurrentGame.LastGamePlayAction == LastGamePlayAction.CARDDECKED)
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
            else if (onDeckCard.CardIdentity == CardIdentity.No3 && CurrentGame.LastGamePlayAction == LastGamePlayAction.CARDDECKED)
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
                List<Card> cardsWithSameIdentity = CardsOnHand.Where(x => x.CardIdentity == onDeckCard.CardIdentity).OrderBy(x => x.CardIdentityType).ToList() ?? new List<Card>();
                Card cardWithSameIdentityType = CardsOnHand.OrderBy(x => x.WinningProbability).FirstOrDefault(x => x.CardIdentityType == onDeckCard.CardIdentityType); //Ordering removing cards that can't WIN

                if (cardsWithSameIdentity.Count > 0)
                    cardsToDeck.AddRange(cardsWithSameIdentity);
                else if (cardWithSameIdentityType != null)
                    cardsToDeck.Add(cardWithSameIdentityType);
                else
                    cardsToCollect += 1;
            }

            if (cardsToCollect == 0 && cardsToDeck.Count == 0)
                throw new Exception("Oops, Player is of Course Nothing to Decide");
            //Place Cards First
            if (cardsToDeck.Count > 0)
            {
                //Check if Last No if 8
                Card lastFrontCard = cardsToDeck[cardsToDeck.Count - 1];
                if (lastFrontCard.CardIdentity == CardIdentity.Queen || lastFrontCard.CardIdentity == CardIdentity.No8)
                {
                    //Have an Answer to That
                    List<string> alreadyMarkedToDeck = cardsToDeck.Select(y => y.Id).ToList();
                    //Answers with Numbers
                    List<Card> answersWithNo = CardsOnHand.Where(x => SharedLogic.CanStartGamesCards.Contains(x.CardIdentity)).Where(x => !alreadyMarkedToDeck.Contains(x.Id) && x.CardIdentity == lastFrontCard.CardIdentity).ToList();
                    //Answer By Type
                    Card answersWithType = CardsOnHand.Where(x => SharedLogic.CanStartGamesCards.Contains(x.CardIdentity)).Where(x => !alreadyMarkedToDeck.Contains(x.Id) && x.CardIdentityType == lastFrontCard.CardIdentityType).FirstOrDefault();
                    //Check Answers
                    if (answersWithNo != null && answersWithNo.Count > 0)
                        cardsToDeck.AddRange(answersWithNo.OrderByDescending(x => x.CardIdentity));
                    else if (answersWithType != null)
                        cardsToDeck.Add(answersWithType);
                    else
                    {
                        Logger.LogInfo($"Player-{this.PlayerNo} Has no Answer, Opted to Pick Card");
                        List<Card> noAnswerCardsPicked = CurrentGame.PickCard(this.PlayerNo, 1, false);
                        CardsOnHand.AddRange(noAnswerCardsPicked);
                    }

                }
                //Proceed With Place Cards on Deck
                CurrentGame.PlaceCard(this.PlayerNo, cardsToDeck, this.IsOnCard);
                foreach (var cardDecked in cardsToDeck)
                    CardsOnHand.Remove(cardDecked);
            }
            //Pick Cards Later
            if (cardsToCollect > 0)
            {
                List<Card> cardsPicked = CurrentGame.PickCard(this.PlayerNo, cardsToCollect);
                CardsOnHand.AddRange(cardsPicked);
            }

            //if all cards left can finish game
            this.IsOnCard = CardsOnHand.CardsCanFinishGame();
        }
    }

}
