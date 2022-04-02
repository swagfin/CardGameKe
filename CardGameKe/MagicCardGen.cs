namespace CardGameKe
{
    public static class MagicCardGen
    {
        public static string GetCardImage(Card card)
        {
            GetAbbrSymbol(card, out string abbr, out string symbol);
            return string.Format(@"
 -----------
| {0}       {1} |
|           |    
|     {0}     |
|     {1}     |
|           |
|           |
| {1}       {0} |
 -----------", abbr, symbol);
        }

        private static void GetAbbrSymbol(Card card, out string abbrv, out string symbol)
        {
            abbrv = card.CardIdentity.ToString().Replace("No", string.Empty)
                                                .Replace("ce", string.Empty)
                                                .Replace("ueen", string.Empty)
                                                .Replace("ing", string.Empty)
                                                .Replace("ack", string.Empty).ToUpper();
            //Symbol
            if (card.CardIdentityType == CardIdentityType.Spade)
                symbol = "♠";
            else if (card.CardIdentityType == CardIdentityType.Diamond)
                symbol = "♦";
            else if (card.CardIdentityType == CardIdentityType.Heart)
                symbol = "♥";
            else if (card.CardIdentityType == CardIdentityType.Flower)
                symbol = "♣";
            else
                symbol = string.Empty;
        }
    }
}
