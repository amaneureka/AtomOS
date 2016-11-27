using System;
using System.Collections.Generic;

namespace Atomixilc
{
    internal static class Verbose
    {
        internal static void Warning(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(message, args));
        }

        internal static void Error(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(message, args));
        }

        internal static void Message(string message, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format(message, args));
        }
    }
}
