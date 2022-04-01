using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return new List<CardIdentity> { CardIdentity.No4, CardIdentity.No5, CardIdentity.No6, CardIdentity.No7 };
            }
        }
    }
}
