using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGameKe
{
    public static class SharedLogic
    {
        public static List<Card> GetStackOfCards()
        {
            return (from CardIdentity cardIdentity in Enum.GetValues(typeof(CardIdentity))
                    from CardIdentityType cardIdentityType in Enum.GetValues(typeof(CardIdentityType))
                    select new Card
                    {
                        CardIdentity = cardIdentity,
                        CardIdentityType = cardIdentityType,
                        Id = Guid.NewGuid().ToString()
                    }).ToList();
        }
        public static void ShuffleCards(this List<Card> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static List<CardIdentity> CanStartGamesCards
        {
            get
            {
                return new List<CardIdentity> { CardIdentity.No4, CardIdentity.No5, CardIdentity.No6, CardIdentity.No7, CardIdentity.No9, CardIdentity.No10 };
            }
        }

        public static bool CardsCanFinishGame(this List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return false;
            return cards.Where(x => CanStartGamesCards.Contains(x.CardIdentity)).ToList()?.Count() == cards.Count;
        }
        public static bool CardsCanFinishGameBasedOnCard(this List<Card> cards, Card lastCardOnDeck)
        {
            if (!CardsCanFinishGame(cards))
                return false;
            //Can Finish with all Numbers
            bool canFinishWithAllNumbers = cards.Where(x => x.CardIdentity == lastCardOnDeck.CardIdentity).ToList()?.Count() == cards.Count;
            if (canFinishWithAllNumbers)
                return true;
            //Can Finish with Only One Card
            if (cards.Count == 1 && cards[0]?.CardIdentityType == lastCardOnDeck.CardIdentityType)
                return true;
            return false;
        }
    }
}
