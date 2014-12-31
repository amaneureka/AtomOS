/* Copyright (C) Atomix Development, Inc - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2014
 * 
 * Heap.cs
 *      Heap Memory Manager
 *      
 *      History:
 *          20-12-14    File Created    Aman Priyadarshi
 */

using System;
using System.Collections.Generic;

using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.core
{
    public static class Heap
    {
        public static uint HeapStart = 0;
        public static uint HeapCurrent = 0;
        public static uint HeapEnd = 0;

        public static void Init(uint InitHeap)
        {
            HeapStart = InitHeap;
            HeapCurrent = InitHeap;
            HeapEnd = HeapStart + 0x100000;//Completely Assumption
            Debug.Write("Heap Initialized!!\n");
            Debug.Write("       Start Address::%d\n", InitHeap);
            Debug.Write("       End Address  ::%d\n", HeapEnd);
        }

        public static void Setup(uint Start, uint End)
        {
            HeapStart = Start;
            HeapCurrent = Start;
            HeapEnd = End;
            Debug.Write("New Heap Setup!!\n");
            Debug.Write("       Start Address::%d\n", Start);
            Debug.Write("       End Address  ::%d\n", End);
        }

        [Label("Heap")]
        public static uint kmalloc(uint aLength)
        {
            /*if (HeapCurrent + aLength > HeapEnd)
            {
                //Allocate more memory
            }*/
            uint tmp = HeapCurrent;
            HeapCurrent += aLength;
            Clear(tmp, aLength);
            return tmp;
        }

        public static uint kmalloc(uint aLength, bool Align)
        {
            if (Align && ((HeapCurrent & 0xFFFFF000) != HeapCurrent))
            {
                HeapCurrent = (HeapCurrent & 0xFFFFF000) + 0x1000;
            }
            return kmalloc(aLength);
        }

        public static unsafe void Clear(uint Address, uint ByteCount)
        {
            var xAddress = (uint*)Address;
            for (uint i = 0; i <= ByteCount/4; i++)
            {
                xAddress[i] = 0x0;
            }
        }
    }
}
