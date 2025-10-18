using Narod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SteamGameFinder2
{
    internal static class ConsoleLogger
    {
        public static void printToConsole(LogLevel printLevel, LogLevel messageLevel, String message)
        {
            if (messageLevel <= printLevel)
            {
                Console.WriteLine(message);
            }
        }
    }
}
