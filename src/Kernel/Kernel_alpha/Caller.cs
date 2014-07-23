using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers.Input;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha
{
    public static class Caller
    {
        public static unsafe void Start()
        {
            Console.Clear();
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
            Console.WriteLine("Partition Count::" + Global.Parts.Count.ToString());
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
                else if (s.Code == KeyCode.C)
                {
                    Console.Clear();
                }
                else if (s.Code == KeyCode.V)
                {
                    var svga = new Drivers.Video.VMWareSVGAII();
                    svga.SetMode(1024, 768, 32);
                    svga.Clear(0xFFFFFF);
                }
            }
            else if (Global.KBD.Alt)
            {
                //if (s.Code == KeyCode.A)
                //    Test();
                //else if (s.Code == KeyCode.B)
                    //Test2("WIN");
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
                
                var s = ((uint)Global.Mouse.X).ToString();
                var J = ((uint)Global.Mouse.Y).ToString();
                WriteScreen("Y:", 24);
                
                
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
                Thread.Sleep(15);
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
            try
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
                    Thread.Sleep(10);
                }
                while (true);
            }
            catch (Exception e)
            {
                Console.Write("Died::");
                Console.WriteLine(e.Message);
                Thread.Die();
            }
        }
    }
}
