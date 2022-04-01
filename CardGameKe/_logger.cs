using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameKe
{
    public static class Logger
    {
        public static void LogInfo(string message, string owner = "")
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.IsNullOrEmpty(owner) ? message : $"[{owner}]: {message}");
        }
        public static void LogWarning(string message, string owner = "")
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.IsNullOrEmpty(owner) ? message : $"[{owner}]: {message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void LogError(string message, string owner = "")
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.IsNullOrEmpty(owner) ? message : $"[{owner}]: {message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
