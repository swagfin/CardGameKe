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

            Game game = new Game(10, 4);
            game.StartGame();
            while (true)
            {
                Thread.Sleep(1000);
            }
            Console.ReadLine();
        }
    }
}
