using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardGameKe
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogInfo("++++++++++++ CARD GAME Ke ++++++++++++");
            List<Card> allCards = LogicFunctions.GetStackOfCards();
            Console.WriteLine($"Total Cards: {allCards.Count}");
            allCards.Shuffle();


            Game game = new Game(2);
            game.StartGame();
            while (true)
            {
                Thread.Sleep(1000);
            }
            Console.ReadLine();
        }
    }
}
