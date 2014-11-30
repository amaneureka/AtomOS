using System;
using System.Collections.Generic;
using Kernel_alpha.Drivers;
using Kernel_alpha.Drivers.Input;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.x86.Intrinsic;

using Kernel_alpha.FileSystem.FAT.Lists;
using Kernel_alpha.FileSystem.FAT;

using Kernel_alpha.Lib.Encoding;

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
            Console.Clear();
            Console.WriteLine();
            Multitasking.CreateTask(pFAT32test, true);
            Multitasking.CreateTask(pSerialTest, true);
            Multitasking.CreateTask(pIdleTask, true);
        }

        private static uint pIdleTask;
        private static void IdleTask()
        {
            while (true)
            {
                x86.Intrinsic.Native.Halt();
            }
        }

        private static uint pSerialTest;
        private static void SerialTest()
        {
            while (true)
            {
                var xRAM = x86.Heap.AllocateMem(0);
                x86.Serials.Write(0xFA);
                x86.Serials.Write((byte)(xRAM >> 0));
                x86.Serials.Write((byte)(xRAM >> 8));
                x86.Serials.Write((byte)(xRAM >> 16));
                x86.Serials.Write((byte)(xRAM >> 24));
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
                    Console.WriteLine("<File>    " + Entry.EntryName + "    " + ((File)Entry).EntryDetails.FileSize.ToString());
                }
            }
            Console.WriteLine();
            Console.WriteLine("#   " + xEntries.Count.ToString() + " " + "Entry(s)");
            Console.WriteLine("#   " + filecount.ToString() + " " + "File(s)");
            Console.WriteLine("#   " + dircount.ToString() + " " + "Dir(s)");
            Console.WriteLine();
        }

        public static unsafe void Update()
        {
            if (Global.KBD.Ctrl)
            {
                var s = Global.KBD.ReadKey();
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
                    svga.Update(0, 0, 1024, 768);
                }
                else if (s.Code == KeyCode.G)
                {
                    var vga = new Drivers.Video.VGAScreen();
                    vga.SetMode0();
                    byte c = 0;
                    /*for (uint i = 0; i < vga.Width; i ++)
                    {
                        for (uint j = 0; j < vga.Height; j++)
                        {
                            vga.SetPixel_640_480(i, j, 0);
                        }
                    }*/
                    for (uint i = 0; i < 10; i++)
                    {
                        for (uint j = 0; j < 400; j++)
                        {
                            vga.SetPixel_640_480(i, j, 0x0);
                            vga.SetPixel_640_480(i + 10, j, 0x1);
                            vga.SetPixel_640_480(i + 20, j, 0x2);
                            vga.SetPixel_640_480(i + 30, j, 0x3);
                            vga.SetPixel_640_480(i + 40, j, 0x4);
                            vga.SetPixel_640_480(i + 50, j, 0x5);
                            vga.SetPixel_640_480(i + 60, j, 0x6);
                            vga.SetPixel_640_480(i + 70, j, 0x7);
                            vga.SetPixel_640_480(i + 80, j, 0x8);
                            vga.SetPixel_640_480(i + 90, j, 0x9);
                            vga.SetPixel_640_480(i + 100, j, 0xA);
                            vga.SetPixel_640_480(i + 110, j, 0xB);
                            vga.SetPixel_640_480(i + 120, j, 0xC);
                            vga.SetPixel_640_480(i + 130, j, 0xD);
                            vga.SetPixel_640_480(i + 140, j, 0xE);
                            vga.SetPixel_640_480(i + 150, j, 0xF);
                        }
                    }
                }
                else if (s.Code == KeyCode.B)
                {
                    var bochs = new Drivers.Video.VBE.Bochslfb();
                    bochs.SetMode(1024, 768, 24);                    
                    for (uint i = 0; i < 1024; i++)
                    {
                        for (uint j = 0; j < 768; j++)
                        {
                            bochs.SetPixel(i, j, 0xFFFFFF);
                        }
                    }
                }
                else if (s.Code == KeyCode.P)
                {
                    uint* p = (uint*)0xA0000000;
                    uint t = *p;
                }
            }
        }

        private static uint pTask1;
        public static unsafe void Task1()
        {
            do
            {
                WriteScreen("X:", 6);                
                //var s = ((uint)Global.Mouse.X).ToString();
                //var J = ((uint)Global.Mouse.Y).ToString();
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

        private static uint pFAT32test;
        public static void FAT32test()
        {
            if (Global.Devices.Count >= 3)
            {
                var xFAT = new FileSystem.FatFileSystem(Global.Devices[2]);
                if (xFAT.IsValid)
                {
                    for (; ; )
                    {
                        string xTemp = Console.ReadLine();
                        string[] xStrName = xTemp.Split(' ');
                        string xCommand = xStrName[0].Trim('\0');

                        string xDirName = null;
                        if (xStrName.Length > 1)
                            xDirName = xStrName[1].Trim('\0');

                        switch (xCommand.ToLower())
                        {
                            case "cd":
                                {
                                    try
                                    {
                                        xFAT.ChangeDirectory(xDirName);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    break;
                                }
                            case "dir":
                                {
                                    try
                                    {
                                        PrintEntries(xFAT.ReadFATDirectory(xDirName));
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    break;
                                }
                            case "open":
                                {
                                    var xData = xFAT.ReadFile(xDirName);
                                    Console.WriteLine(ASCII.GetString(xData, 0, xData.Length));
                                    Console.WriteLine();
                                    break;
                                }
                            case "mkdir":
                                {
                                    try
                                    {
                                        xFAT.MakeDirectory(xDirName);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    break;
                                }
                            case "run":
                                {
                                    try
                                    {
                                        var xData = xFAT.ReadFile(xDirName);
                                        unsafe
                                        {
                                            //Okay this code is working fine
                                            /* ; Example code
                                            * use32
                                            * main:
                                            *  push dword EBP
                                            *  mov dword EBP, ESP
                                            *  mov dword EAX, 0xB8000
                                            *  mov dword [EAX + 0x2], 0x0A41
                                            *  leave
                                            *  ret 0x0
                                            *  
                                            * ; Command Line 
                                            * nasm -fbin test.asm -o test.atm
                                            */
                                            var len = xData.Length;
                                            var xAdd = x86.Heap.AllocateMem((uint)len);
                                            var Mem = (byte*)xAdd;
                                            for (int i = 0; i < len; i++)
                                            {
                                                Mem[i] = xData[i];
                                            }
                                            CallExecutableFile(xAdd);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                }
                                break;
                            default:
                                Console.WriteLine("No such command exist");
                                break;
                        }
                    }
                }
            }
            Console.WriteLine("FAT32 Thread died :(");
            while(true)
            {
                Console.ReadLine();
            }
            Thread.Die();
        }

        [Assembly(0x4)]
        private static void CallExecutableFile(uint pos)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Call("EAX"));        
        }
    }
}
