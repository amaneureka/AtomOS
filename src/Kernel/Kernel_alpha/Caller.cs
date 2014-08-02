using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers.Input;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.x86.Intrinsic;

using Kernel_alpha.FileSystem.FAT.Lists;
using Kernel_alpha.FileSystem.FAT;

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
            Console.WriteLine("Block Device Count::" + Global.Devices.Count.ToString());
            uint c = 0;
            for (int i = 0; i < Global.Devices.Count; i++)
            {
                if (Global.Devices[i] is Drivers.Partition)
                    c++;
            }
            Console.WriteLine("Partition Count::" + c.ToString());            
            var xFAT = new FileSystem.FatFileSystem(Global.Devices[2]);
            Console.Clear();
            Console.WriteLine();
            //xFAT.FlushDetails();
            
            var xEntries = xFAT.ReadDirectory(2).GetEntries;
            PrintEntries(xEntries);
            var location = xFAT.FindEntry(new FileSystem.Find.WithName("AMANP"), 2);
            if (location != null)
            {
                PrintEntries(xFAT.ReadDirectory(location.FirstCluster).GetEntries);
            }
        }

        public static void PrintEntries(List<Base> xEntries)
        {
            int filecount = 0;
            int dircount = 0;
            for (int i = 0; i < xEntries.Count; i++)
            {
                var Entry = xEntries[i];
                if (Entry is Directory)
                {
                    dircount++;
                    Console.WriteLine("<DIR>    " + Entry.EntryName);
                }
                else if (Entry is File)
                {
                    filecount++;
                    Console.WriteLine("<File>    " + Entry.EntryName + "    ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("#   " + xEntries.Count.ToString() + " " + "Entry(s)");
            Console.WriteLine("#   " + filecount.ToString() + " " + "File(s)");
            Console.WriteLine("#   " + dircount.ToString() + " " + "Dir(s)");
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
                else if (s.Code == KeyCode.G)
                {
                    var vga = new Drivers.Video.VGAScreen();
                    vga.SetMode0();
                    byte c = 0;
                    for (uint i = 0; i < vga.Width; i ++)
                    {
                        for (uint j = 0; j < vga.Height; j++)
                        {
                            vga.SetPixel_8(i, j, c);
                        }
                        c++;
                    }
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
