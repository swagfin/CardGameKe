using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameKe
{
    public class Card
    {
        public CardIdentity CardIdentity { get; set; }
        public CardIdentityType CardIdentityType { get; set; }
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
