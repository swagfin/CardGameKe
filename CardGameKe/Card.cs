using System;

namespace CardGameKe
{
    public class Card
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public CardIdentity CardIdentity { get; set; }
        public CardIdentityType CardIdentityType { get; set; }
        public int WinningProbability
        {
            get
            {
                if (SharedLogic.CanStartGamesCards.Contains(CardIdentity))
                    return 100;
                else if (CardIdentity == CardIdentity.King)
                    return 50;
                else if (CardIdentity == CardIdentity.Jack)
                    return 40;
                else if (CardIdentity == CardIdentity.Queen)
                    return 30;
                else if (CardIdentity == CardIdentity.No2)
                    return 20;
                else if (CardIdentity == CardIdentity.No3)
                    return 10;
                else
                    return 0;
            }
        }
    }
    public enum CardIdentity
    {
        Ace,
        No2,
        No3,
        No4,
        No5,
        No6,
        No7,
        No8,
        No9,
        No10,
        Jack,
        Queen,
        King
    }
    public enum CardIdentityType
    {
        Flower, //or Clubs
        Diamond,
        Heart,
        Spade
    }
}
