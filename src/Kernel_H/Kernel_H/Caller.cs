using System;

using libAtomixH.Threading;
using sys = libAtomixH.mscorlib.System;

namespace Kernel_H
{
    public static class Caller
    {
        public static Thread system;

        public static unsafe void Start ()
        {
            Console.Clear ();
            Console.WriteLine ("Hey Aman!");
            Console.WriteLine ("The new Console can clear the whole Background :P");
            Console.WriteLine ();
            Console.WriteLine ("I got rid of the pointer and rewrote the Console" +
                " to work with X and Y coordinates, which works pretty good :)");
            Console.WriteLine ();
            Console.WriteLine ("I also plugged the SetCursorPosition method ^^");
            Console.WriteLine ();
            Console.WriteLine ("Hope you enjoy :P");

            //Scheduler.CreateTask (pTask1, true);
        }

        public static uint pTask1;
        public static void Task1 ()
        {
            char[] tests = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            while (true)
            {
                for (int i = 0; i < tests.Length; i++)
                {
                    Console.SetCursorPosition (10, 20);
                    Console.Write (tests[i]);
                    Thread.Sleep (100);
                }
            }
        }

        public static void Update ()
        {

        }

        public static unsafe void WriteScreen (string s, int p)
        {
            byte* xA = (byte*)0xB8000;
            for (int i = 0; i < s.Length; i++)
            {
                xA[p++] = (byte)s[i];
                xA[p++] = 0x0B;
            }
        }
    }
}
