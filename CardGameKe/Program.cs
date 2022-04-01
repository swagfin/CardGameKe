using System;
using System.Threading;

namespace CardGameKe
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogInfo("++++++++++++ CARD GAME Ke ++++++++++++");

            Game game = new Game(3, 4);
            game.StartGame();
            while (true)
            {
                Thread.Sleep(1000);
            }
            Console.ReadLine();
        }
    }
}
