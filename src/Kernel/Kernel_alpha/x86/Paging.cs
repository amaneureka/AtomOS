using System;
using System.Collections.Generic;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;
using Kernel_alpha.x86.Intrinsic;

namespace Kernel_alpha.x86
{
    public static unsafe class Paging
    {
        static uint[] Frames;
        static UInt32 Kernel_Directory;
        static UInt32 Current_Directory;

        /// <summary>
        /// Setup Paging
        /// </summary>
        /// <param name="MemoryEnd">How much of RAM we want to page?</param>
        public static void Setup(uint MemoryEnd)
        {
            Frames = new uint[(MemoryEnd / 0x1000) / 32]; //Each frame is 4KB in Size and we use 1 bit for each frame

            Kernel_Directory = Heap.AllocateMem(0x1000, true);
            Current_Directory = Kernel_Directory;

            uint i = 0;
            while(i < Heap.PlacementAddress + 0x1000)
            {
                //Just roughly
                AllocateFrame(GetPage(i, (UInt32*)Kernel_Directory, true), false, false);
                i += 0x1000;
            }
            SwitchDirectory(Kernel_Directory);
        }

        public static void AllocateFrame(UInt32 Page, bool IsKernel, bool IsWriteable)
        {
            /*
                u32int present    : 1;   // Page present in memory
                u32int rw         : 1;   // Read-only if clear, readwrite if set
                u32int user       : 1;   // Supervisor level only if clear
                u32int accessed   : 1;   // Has the page been accessed since last refresh?
                u32int dirty      : 1;   // Has the page been written to since last refresh?
                u32int unused     : 7;   // Amalgamation of unused and reserved bits
                u32int frame      : 20;  // Frame address (shifted right 12 bits)
            */
            uint Frame = *((UInt32*)Page) >> 12;
            if (Frame != 0)
                return;//Already Allocated
            else
            {
                Frame = FirstFrame();
                *((UInt32*)Page) = (UInt32)((Frame << 12) | 0x1);
                SetFrame(Frame);
            }
        }

        public static void SetFrame(UInt32 page)
        {
            Frames[(page / 32)] |= (uint)(0x1 << ((int)page % 32));
        }

        public static void ClearFrame(UInt32 page)
        {
            Frames[(page / 32)] &= ~(uint)(0x1 << ((int)page % 32));
        }

        public static void FreeFrame(UInt32 page)
        {
            uint Frame = *((UInt32*)page) >> 12;
            if (Frame == 0)
                return;
            else
            {
                ClearFrame(page);
                *((UInt32*)page) &= 0xFFF;
            }
        }

        public static UInt32 FirstFrame()
        {
            for (int i = 0; i < Frames.Length; i++)
            {
                if (Frames[i] != 0xFFFFFFFF)
                {
                    //Atleast one frame is free
                    for (int j = 0; j < 32; j++)
                    {
                        if ((Frames[i] & (0x1 << j)) == 0)
                            return (uint)((uint)(32 * i) + j);
                    }
                }
            }
            Console.WriteLine("NO FREE FRAME");
            while (true) ;
        }

        public static UInt32 GetPage(UInt32 Address, UInt32* PageDirectory, bool Make)
        {
            Address /= 0x1000;

            //Get Index in Page Directory, i.e. address of page table
            uint tableIndex = Address / 1024;

            if (PageDirectory[tableIndex] != 0)//Page Table is present
            {
                return (PageDirectory[tableIndex] & 0xFFFFF000) + (4 * (Address % 1024));
            }
            else if (Make)
            {
                uint tmp = Heap.AllocateMem(0x1000, true);
                PageDirectory[tableIndex] = tmp | 0x7;//Present, RW, US

                return tmp + (4 * (Address % 1024));
            }

            return 0;
        }

        public static void SwitchDirectory(UInt32 PageDirectory)
        {
            Current_Directory = PageDirectory;
            SetRegister(Current_Directory);
        }

        [Assembly(0x4)]
        private static void SetRegister(uint PageDirectory)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3,  SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.CR0 });
            Core.AssemblerCode.Add(new Or { DestinationReg = Registers.EBX, SourceRef = "0x80000000" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR0, SourceReg = Registers.EBX });
        }
    }
}
