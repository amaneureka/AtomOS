using System;
using System.Collections.Generic;

namespace Atomixilc
{
    internal static class Verbose
    {
        internal static void Warning(string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(message, args));
            Console.ForegroundColor = oldColor;
        }

        internal static void Error(string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(message, args));
            Console.ForegroundColor = oldColor;
        }

        internal static void Message(string message, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(string.Format(message, args));
            Console.ForegroundColor = oldColor;
        }
    }
}
