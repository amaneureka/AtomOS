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
            Multitasking.CreateTask(pTask1, true);
            Multitasking.CreateTask(pTask2, true);
            
            // Setup Keyboard
            KBD = new Keyboard();

            // Setup Mouse
            Mouse = new PS2Mouse ();
            Mouse.Initialize ();

            // Setup PCI
            PCI.Setup();

            // Start ACPI
            // Initializes and enables itself
            ACPI = new Drivers.acpi (true, true);

            Console.WriteLine("WELCOME TO MY ATOMIX BUILDER");            
        }

        public static unsafe void Update()
        {
            var s = KBD.Read();
            if (KBD.Alt)
            {
                Console.WriteLine("Shutdown");
                ACPI.Shutdown();
            }
            else if (KBD.Ctrl)
            {
                Console.WriteLine("Reboot");
                ACPI.Reboot();
            }
            else if (s.Code == KeyCode.Esc)
            {
                Console.Write("X:");
                Console.WriteLine(((uint)Mouse.X).ToString());
                Console.Write("Y:");
                Console.WriteLine(((uint)Mouse.Y).ToString());
            }
            else if (s != null)
                Console.Write(s.Char);
        }

        private static uint pTask1;
        public static unsafe void Task1()
        {
            byte* xA = (byte*)0xB8000;            
            byte c = 0;
            do
            {
                xA[4] = c;
                xA[5] = 0xb;
                c++;
                if (c >= 255)
                    c = 0;
            }
            while (true);
        }

        private static uint pTask2;
        public static unsafe void Task2()
        {
            byte* xA = (byte*)0xB8000;            
            byte c = 0;
            uint a = 0;
            do
            {
                xA[6] = c;
                xA[7] = 0xd;
                c++;
                if (c >= 255)
                    c = 0;
                a++;
                Thread.Sleep(100);
            }
            while (a != 10);
            Console.WriteLine("My task is finished");
            Thread.Die();
        }
    }
}
