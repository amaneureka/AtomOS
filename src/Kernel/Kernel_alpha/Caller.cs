using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.Drivers.Input;

namespace Kernel_alpha
{
    public static class Caller
    {        
        public static Keyboard KBD;
        public static PS2Mouse Mouse;
        public static Drivers.acpi ACPI;
        public static unsafe void Start()
        {
            Console.WriteLine ("                                         ");

            Console.Write ("Creating Task1... ");
            Multitasking.CreateTask(pTask1, true);
            Console.WriteLine ("OK");

            Console.Write ("Creating Task2... ");
            Multitasking.CreateTask(pTask2, true);
            Console.WriteLine ("OK");
            
            // Setup Keyboard
            Console.Write ("Setting up Keyboard... ");
            KBD = new Keyboard();
            Console.WriteLine ("OK");

            // Setup Mouse
            Console.Write ("Setting up PS/2 Mouse... ");
            Mouse = new PS2Mouse ();
            Mouse.Initialize();
            Console.WriteLine ("OK");

            // Setup PCI
            Console.Write ("Setting up PCI... ");
            PCI.Setup();
            Console.WriteLine ("OK");

            // Start ACPI
            // Initializes and enables itself
            Console.Write ("Setting up ACPI... ");
            ACPI = new Drivers.acpi (true, true);
            Console.WriteLine ("OK");

            Console.WriteLine ("Welcome to AtomixOS!");
            Console.WriteLine ();

            Console.WriteLine ("Shutdown: Ctrl+S");
            Console.WriteLine ("Reboot: Ctrl+R");
        }

        public static unsafe void Update()
        {
            Keys s = KBD.Read();
            if (KBD.Ctrl)
            {
                if (s.Code == KeyCode.S)
                {
                    Console.WriteLine ("Shutdown");
                    ACPI.Shutdown ();
                }
                else if (s.Code == KeyCode.R)
                {
                    Console.WriteLine ("Reboot");
                    ACPI.Reboot ();
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
                WriteScreen(((uint)Mouse.X).ToString(), 10);

                WriteScreen("Y:", 24);
                WriteScreen(((uint)Mouse.Y).ToString(), 28);

                switch (Mouse.Button)
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
                Thread.Sleep(5);
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
                //Thread.Sleep(100);
            }
            while (true);
            Console.WriteLine("My task is finished");
            Thread.Die();
        }
    }
}
