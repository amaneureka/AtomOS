/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Paging Setup
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/


using System;

using Atomix.Kernel_H.Core;

using Atomixilc.Lib;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

namespace Atomix.Kernel_H.Arch.x86
{
    internal static unsafe class Paging
    {
        public static uint* KernelDirectory;
        public static uint* CurrentDirectory;
        private static uint[] Frames;

        internal const uint PageSize = 0x1000;

        internal static void Setup(uint aKernelDirectory)
        {
            KernelDirectory = (uint*)aKernelDirectory;
            Frames = new uint[Multiboot.RAM / 0x20000];

            // Tell Frame Allocator that we have already used first 4MB
            uint i = 0;
            while (i < 32)
                Frames[i++] = 0xFFFFFFFF;

            //Detect memory that can we use and mark rest as already used
            Multiboot.DetectMemory();

            /*
             * First 4MB of BIOS is mapped
             * [0x0 - 0x400000) -> [0xc0000000 - 0xc0400000)
             * So, we have to first map rest of kernel code + Heap
             */
            uint phy = 0x400000, virt = 0xc0400000, end = Multiboot.RamDiskEnd;
            if (end == 0)
                end = Native.EndOfKernel();

            while (virt < end)
            {
                AllocateFrame(GetPage(KernelDirectory, virt, true), phy, false);
                virt += 0x1000;
                phy += 0x1000;
            }

            // Lets Map the new Heap; Just to the end of kernel
            uint HeapSize = 0x2000000, HeapStart = virt;//32MB
            end = virt + HeapSize;

            while (virt < end)
            {
                AllocateFrame(GetPage(KernelDirectory, virt, true), 0, true);
                virt += 0x1000;
            }

            // Setup our New Heap
            Heap.Setup(HeapStart, end);
            CurrentDirectory = KernelDirectory;
            Debug.Write("@Paging:: Directory: %d\n", (uint)CurrentDirectory);
        }

        internal static uint AllocateMainBuffer(uint phybase)
        {
            // 4MB * 1 => 4MB
            uint VirtLocation = 0xE0000000, VirtEnd = VirtLocation + 0x400000;
            while(VirtLocation < VirtEnd)
            {
                AllocateFrame(GetPage(KernelDirectory, VirtLocation, true), phybase, false);
                phybase += 0x1000;
                VirtLocation += 0x1000;
            }

            return 0xE0000000;
        }

        internal static uint AllocateSecondayBuffer()
        {
            // 4MB * 1 => 4MB
            uint VirtLocation = 0xE0400000, VirtEnd = VirtLocation + 0x400000;
            while (VirtLocation < VirtEnd)
            {
                AllocateFrame(GetPage(KernelDirectory, VirtLocation, true), 0, true);
                VirtLocation += 0x1000;
            }

            return 0xE0400000;
        }

        internal static void AllocateFrame(uint Page, uint PhyPage, bool Allocate, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            Page += 0xC0000000;
            var Add = *((uint*)Page);
            if (Add != 0)
                return; // We don't want to overwrite anything
            else
            {
                if (Allocate)
                    PhyPage = FirstFreeFrame() * 0x1000;
                *((uint*)Page) = PhyPage | flags;
                SetFrame(PhyPage / 0x1000);
            }
        }

        internal static uint FirstFreeFrame()
        {
            int Length = Frames.Length;
            var MemoryFrames = Frames;
            for (int i = 0; i < Length; i++)
            {
                if (MemoryFrames[i] != 0xFFFFFFFF)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        uint index = (uint)(1 << j);
                        if ((MemoryFrames[i] & index) == 0)
                            return (uint)((i << 5) + j);
                    }
                }
            }
            Debug.Write("No Free Frame! :(");
            while (true) ;
        }

        internal static uint GetPage(uint* Directory, uint VirtAddress, bool Make, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            VirtAddress /= 0x1000; // Align it to page
            int index = (int)(VirtAddress / 1024);

            if (Directory[index] != 0)
            {
                return (Directory[index] & 0xFFFFF000) + (VirtAddress % 1024) * 4;
            }
            else if (Make)
            {
                var tmp = Heap.kmalloc(0x1000, true);
                tmp -= 0xC0000000;
                Directory[index] = tmp | flags;
                return tmp + ((VirtAddress % 1024) * 4);
            }
            return 0;
        }

        internal static uint* CloneKernelDirectory()
        {
            uint* NewDirectory = (uint*)(Heap.kmalloc(0x1000, true));
            for (uint Table = 768; Table < 1024; Table++)
            {
                NewDirectory[Table] = KernelDirectory[Table];
            }
            return NewDirectory;
        }

        internal static void FreeDirectory(uint* Directory)
        {
            for (uint Table = 0; Table < 768; Table++)
            {
                ClearFrame(Directory[Table] / 0x1000);
            }
            ClearFrame((uint)Directory / 0x1000);
        }

        internal static void SetFrame(uint page)
        {
            Frames[(page >> 5)] |= (uint)(0x1 << ((int)page & 31));
        }

        internal static void ClearFrame(uint page)
        {
            Frames[(page >> 5)] &= ~(uint)(0x1 << ((int)page & 31));
        }

        [Assembly(true)]
        internal static void RefreshTLB()
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.CR3 };
            new Mov { DestinationReg = Register.CR3, SourceReg = Register.EAX };
        }

        [Assembly(true)]
        internal static void InvalidatePageAt(uint xAddress)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Literal ("invlpg [EAX]");
        }

        [Assembly(true)]
        public static void SwitchDirectory(uint Directory)
        {
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Mov { DestinationRef = "static_Field__System_UInt32__Atomix_Kernel_H_Arch_x86_Paging_CurrentDirectory", DestinationIndirect = true, SourceReg = Register.EAX };
            new Mov { DestinationReg = Register.CR3, SourceReg = Register.EAX };
        }
    }
}
