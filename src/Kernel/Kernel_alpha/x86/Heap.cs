using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Kernel_alpha.x86.Intrinsic;
using Atomix.CompilerExt.Attributes;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Kernel_alpha.x86
{
    public static class Heap
    {
        public const uint kHeap_Start = 0x50000000;
        public const uint kHeap_Initial_Size = 0x500000;

        public static uint PlacementAddress = 0;
        
        [Label("Heap")]
        public static uint kmalloc(uint aLength)
        {
            return AllocateMem(aLength);
        }

        public static uint AllocateMem(uint aLength, bool Align = false)
        {
            if (Align && ((PlacementAddress & 0xFFFFF000) != 0))//If we have to align and the placement address is not aligned
            {
                PlacementAddress &= 0xFFFFF000;
                PlacementAddress += 0x1000;
            }

            uint xResult = PlacementAddress;
            PlacementAddress += aLength;
            Memory.Clear(xResult, aLength);

            return xResult;
        }

        public static void CreateHeap()
        {
            PlacementAddress = kHeap_Start;
        }
    }
}
