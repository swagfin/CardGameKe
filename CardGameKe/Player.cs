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
                            Logger.LogInfo("Playing....", $"PLAYER-{this.PlayerNo}");

                            Card card = CardsOnHand.FirstOrDefault();
                            if (card != null)
                            {
                                CurrentGame.PlaceCard(this.PlayerNo, card);
                                CardsOnHand.Remove(card);
                            }
                            else
                            {
                                Logger.LogInfo("Cardless, Picking Card....", $"PLAYER-{this.PlayerNo}");
                                Card cardPicked = CurrentGame.PickCard(this.PlayerNo);
                                CardsOnHand.Add(cardPicked);
                            }

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
    }
}
