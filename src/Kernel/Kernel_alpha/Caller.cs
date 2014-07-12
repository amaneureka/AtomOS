using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers.Input;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha
{
    public static class Caller
    {
        public static unsafe void Start()
        {
            Console.WriteLine ("                                         ");

            // Load System Elements
            Global.Init();

            Console.WriteLine ("Welcome to AtomixOS!");
            Console.WriteLine ();

            Console.WriteLine ("Shutdown: Ctrl+S");
            Console.WriteLine ("Reboot: Ctrl+R");

            // Just for mouse testing
            Multitasking.CreateTask(pTask1, true);
            Multitasking.CreateTask(pTask2, true);
            
            DELTESTING a = Test123;
            a(12, "Invoke1");
            a(56, "Invoke2");
            a(78, "Invoke3");
        }
        public delegate void DELTESTING(uint a, string b);
        private static int wow = 1;
        public static void Test123(uint a, string b)
        {
            Console.Write("Hey...");
            Console.Write(wow.ToString());
            Console.Write("...");
            Console.Write(a.ToString());
            Console.Write("...");
            Console.WriteLine(b);
            wow++;
        }

        public static unsafe void Update()
        {
            var s = Global.KBD.ReadKey ();
            if (Global.KBD.Ctrl)
            {
                if (s.Code == KeyCode.S)
                {
                    Console.WriteLine ("Shutdown");
                    Global.ACPI.Shutdown();
                }
                else if (s.Code == KeyCode.R)
                {
                    Console.WriteLine ("Reboot");
                    Global.ACPI.Reboot();
                }
            }
            else if (s != null)
                Console.Write(s.Char);
        }

        private static uint pTask1;
        public static unsafe void Task1()
        {            
            do
            {
                WriteScreen("X:", 6);
                //WriteScreen(((uint)Global.Mouse.X).ToString(), 10);

                WriteScreen("Y:", 24);
                //WriteScreen(((uint)Global.Mouse.Y).ToString(), 28);

                switch (Global.Mouse.Button)
                {
                    case MouseButtons.Left:
                        WriteScreen("L", 40);
                        break;
                    case MouseButtons.Right:
                        WriteScreen("R", 40);
                        break;
                    case MouseButtons.Middle:
                        WriteScreen("M", 40);
                        break;
                    case MouseButtons.None:
                        WriteScreen("N", 40);
                        break;
                    default:
                        WriteScreen("E", 40);
                        break;
                }
                //Thread.Sleep(5);
            }
            while (true);
        }

        public static unsafe void WriteScreen(string s, int p)
        {
            byte* xA = (byte*)0xB8000;
            for (int i = 0; i < s.Length; i++)
            {
                xA[p++] = (byte)s[i];
                xA[p++] = 0x0B;
            }
        }

        private static uint pTask2;
        public static unsafe void Task2()
        {
            byte* xA = (byte*)0xB8000;            
            byte c = 0;
            uint a = 0;
            do
            {
                xA[0] = c;
                xA[1] = 0xd;
                c++;
                if (c >= 255)
                    c = 0;
                a++;
                Thread.Sleep(100);
            }
            while (true);
            Console.WriteLine("My task is finished");
            Thread.Die();
        }
    }
}
