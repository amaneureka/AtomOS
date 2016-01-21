/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Paging.cs
 *      Setting up x86-paging
 *      
 *      History:
 *          20-12-14    File Created    Aman Priyadarshi
 */


using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.arch.x86
{
    public static unsafe class Paging
    {
        public static UInt32* KernelDirectory;
        public static UInt32* CurrentDirectory;
        private static uint[] Frames;
                
        public static void Setup(uint aKernelDirectory)
        {
            KernelDirectory = (UInt32*)aKernelDirectory;
            Frames = new uint[Multiboot.RAM / 0x20000];
            
            //Tell Frame Allocator that we have already used first 4MB
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
            
            //Lets Map the new Heap; Just to the end of kernel
            uint HeapSize = 0x2000000, HeapStart = virt;//32MB
            end = virt + HeapSize;
            
            while (virt < end)
            {
                AllocateFrame(GetPage(KernelDirectory, virt, true), 0, true);
                virt += 0x1000;
            }
            
            //Setup our New Heap
            Heap.Setup(HeapStart, end);
            CurrentDirectory = KernelDirectory;
            Debug.Write("@Paging:: Directory: %d\n", (uint)CurrentDirectory);
        }
        
        public static uint AllocateMainBuffer(uint phybase)
        {
            //4MB * 1 => 4MB
            uint VirtLocation = 0xE0000000, VirtEnd = VirtLocation + 0x400000;
            while(VirtLocation < VirtEnd)
            {
                AllocateFrame(GetPage(KernelDirectory, VirtLocation, true), phybase, false);
                phybase += 0x1000;
                VirtLocation += 0x1000;
            }

            return 0xE0000000;
        }

        public static uint AllocateSecondayBuffer()
        {
            //4MB * 1 => 4MB
            uint VirtLocation = 0xE0400000, VirtEnd = VirtLocation + 0x400000;
            while (VirtLocation < VirtEnd)
            {
                AllocateFrame(GetPage(KernelDirectory, VirtLocation, true), 0, true);
                VirtLocation += 0x1000;
            }

            return 0xE0400000;
        }

        public static void AllocateFrame(UInt32 Page, UInt32 PhyPage, bool Allocate, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            Page += 0xC0000000;
            var Add = *((UInt32*)Page);
            if (Add != 0)
                return;//We don't want to overwrite anything
            else
            {                
                if (Allocate)
                    PhyPage = FirstFreeFrame() * 0x1000;
                *((UInt32*)Page) = PhyPage | flags;
                SetFrame(PhyPage / 0x1000);
            }
        }

        public static UInt32 FirstFreeFrame()
        {
            int Length = Frames.Length;
            var MemoryFrames = Frames;
            for (int i = 0; i < Length; i++)
            {
                if (MemoryFrames[i] != 0xFFFFFFFF)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((MemoryFrames[i] & (0x1 << j)) == 0)
                            return (uint)((i << 5) + j);
                    }
                }
            }
            Debug.Write("No Free Frame! :(");
            while (true) ;
        }

        public static UInt32 GetPage(UInt32* Directory, UInt32 VirtAddress, bool Make, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            VirtAddress /= 0x1000;//Align it to page
            uint index = VirtAddress / 1024;

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

        public static UInt32* CloneKernelDirectory()
        {
            UInt32* NewDirectory = (UInt32*)(Heap.kmalloc(0x1000, true));
            for (uint Table = 768; Table < 1024; Table++)
            {
                NewDirectory[Table] = KernelDirectory[Table];
            }
            return NewDirectory;
        }

        public static void FreeDirectory(UInt32* Directory)
        {
            for (uint Table = 0; Table < 768; Table++)
            {
                ClearFrame(Directory[Table] / 0x1000);
            }
            ClearFrame((uint)Directory / 0x1000);
        }
        
        public static void SetFrame(UInt32 page)
        {
            Frames[(page >> 5)] |= (uint)(0x1 << ((int)page & 31));
        }

        public static void ClearFrame(UInt32 page)
        {
            Frames[(page >> 5)] &= ~(uint)(0x1 << ((int)page & 31));
        }

        [Assembly(0x0)]
        public static void RefreshTLB()
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR3 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.EAX });
        }

        [Assembly(0x4)]
        public static void InvalidatePageAt(uint xAddress)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Literal("invlpg [EAX]"));
        }

        [Assembly(0x4)]
        public static void SwitchDirectory(uint Directory)
        {            
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationRef = "static_Field__System_UInt32__Atomix_Kernel_H_arch_x86_Paging_CurrentDirectory", DestinationIndirect = true, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.EAX });
        }
    }
}
