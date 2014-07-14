using System;

using libAtomixH;
using libAtomixH.Threading;
using libAtomixH.Drivers;

using c = libAtomixH.mscorlib.System.Console;

namespace Kernel_H
{
    public static class Caller
    {
        public static Thread system;

        public static unsafe void Start ()
        {
            // Initialize the Drivers
            Global.Initialize ();

            // Set our wonderful colors
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Cyan;

            // Clear the console
            Console.Clear ();

            // Setup a thread
            Scheduler.CreateTask (pTask1, true);

            // Say hello
            Console.WriteLine ("Hello from AtomixOS!");
            Console.WriteLine ("Type any text to get it echoed back");
        }

        public static void Update ()
        {
            // Display the prompt
            Console.Write ("Atomix> ");

            // Read the input string
            string str = Console.ReadLine ();

            // Echo the input string back
            Console.Write ("  Echo> " + str + "\n");
        }

        public static uint pTask1;
        public static void Task1 ()
        {
            char[] tests = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            while (true)
            {
                for (int i = 0; i < tests.Length; i++)
                {
                    // Backup our current cursor postion
                    int x = Console.CursorLeft;
                    int y = Console.CursorTop;

                    // Set our new cursor position
                    Console.SetCursorPosition (0, 0);

                    // Write a number
                    Console.Write (tests[i]);

                    // Restore our old cursor position
                    Console.SetCursorPosition (x, y);

                    // Sleep for a short time period
                    Thread.Sleep (20);
                }
            }
        }
    }
}
