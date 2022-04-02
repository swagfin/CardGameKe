namespace CardGameKe
{
    public static class MagicCardGen
    {
        public static string GetCardImage(Card card)
        {
            GetAbbrSymbol(card, out string abbr, out string symbol, out string label);
            return string.Format(@"
 -----------
| {0}      {3}{1} |
|    {3}{0}     |
|     {1}     |
|{2}|
|           |
|{3}{1}       {0} |
 -----------", abbr, symbol, label, (card.CardIdentity == CardIdentity.No10) ? string.Empty : " ");
        }

        private static void GetAbbrSymbol(Card card, out string abbrv, out string symbol, out string label)
        {
            abbrv = card.CardIdentity.ToString().Replace("No", string.Empty)
                                                .Replace("ce", string.Empty)
                                                .Replace("ueen", string.Empty)
                                                .Replace("ing", string.Empty)
                                                .Replace("ack", string.Empty).ToUpper();
            //Symbol
            if (card.CardIdentityType == CardIdentityType.Spade)
            {
                symbol = "♠";
                label = ">  Spade  <";
            }
            else if (card.CardIdentityType == CardIdentityType.Diamond)
            {
                symbol = "♦";
                label = "> Diamond <";
            }
            else if (card.CardIdentityType == CardIdentityType.Heart)
            {
                symbol = "♥";
                label = ">  Heart  <";
            }
            else if (card.CardIdentityType == CardIdentityType.Flower)
            {
                symbol = "♣";
                label = "> Flower  <";
            }
            else
            {
                symbol = string.Empty;
                label = "           ";
            }
        }
    }
}
