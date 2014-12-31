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
using System.Collections.Generic;

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
        private static UInt32* KernelDirectory;        
        private static uint[] Frames;

        public static void Setup(uint aKernelDirectory)
        {
            KernelDirectory = (UInt32*)aKernelDirectory;
            Frames = new uint[Multiboot.RAM / 0x20000];

            //Tell Frame Allocator that we have already used first 4MB
            uint i = 0;
            while (i < 32)
                Frames[i] = 0xFFFFFFFF;

            /*
             * First 4MB of BIOS is mapped
             * [0x0 - 0x400000) -> [0xc0000000 - 0xc0400000)
             * So, we have to first map rest of kernel code + Heap
             */            
            uint phy = 0x400000, virt = 0xc0400000, end = Native.EndOfKernel();            
            while (virt < end)
            {
                AllocateFrame(GetPage(KernelDirectory, virt, true), phy);
                virt += 0x1000;
                phy += 0x1000;
            }
                        
            //Lets Map the new Heap; Just to the end of kernel
            uint HeapSize = 0x100000, HeapStart = virt;//1MB
            end = virt + HeapSize;
            while (virt < end)
            {
                AllocateFrame(GetPage(KernelDirectory, virt, true), phy);
                virt += 0x1000;
                phy += 0x1000;
            }

            //Setup our New Heap
            Heap.Setup(HeapStart, HeapSize);

            RefreshTLB();
        }
        
        private static void AllocateFrame(UInt32 Page, UInt32 PhyPage, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            Page += 0xC0000000;
            var Add = *((UInt32*)Page);
            if (Add != 0)
                return;//We don't want to overwrite anything
            else
            {                
                *((UInt32*)Page) = PhyPage | flags;
                SetFrame(PhyPage);
            }
        }

        public static UInt32 FirstFreeFrame()
        {
            for (int i = 0; i < Frames.Length; i++)
            {
                if (Frames[i] != 0xFFFFFFFF)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((Frames[i] & (0x1 << j)) == 0)
                            return (uint)((uint)(32 * i) + j);
                    }
                }
            }
            return 0;//No Free Frame =(
        }

        private static UInt32 GetPage(UInt32* Directory, UInt32 VirtAddress, bool Make, uint flags = 0x3)//Present, ReadWrite, Supervisor
        {
            VirtAddress /= 0x1000;//Align it to page
            uint index = VirtAddress / 1024;

            if (Directory[index] != 0)
            {
                return (Directory[index] & 0xFFFFF000) + (VirtAddress % 1024) * 4;
            }
            else if (Make)
            {
                var tmp = Heap.kmalloc(0x1000, true);//Allocate space for a new page
                tmp -= 0xC0000000;
                Directory[index] = tmp | flags;
                return tmp + ((VirtAddress % 1024) * 4);
            }
            return 0;
        }
        
        private static void SetFrame(UInt32 page)
        {
            Frames[(page / 32)] |= (uint)(0x1 << ((int)page % 32));
        }

        public static void ClearFrame(UInt32 page)
        {
            Frames[(page / 32)] &= ~(uint)(0x1 << ((int)page % 32));
        }

        [Assembly(0x0)]
        private static void RefreshTLB()
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.CR3 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.EAX });
        }

        [Assembly(0x4)]
        private static void SwitchDirectory(uint Directory)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.CR3, SourceReg = Registers.EAX });
        }
    }
}
