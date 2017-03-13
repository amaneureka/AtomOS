/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          kernel Heap Memory Manager
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;
using Atomixilc.Attributes;

namespace Atomix.Kernel_H.Core
{
    internal static unsafe class Heap
    {
        static uint HeapStart;
        static uint HeapCurrent;
        static uint HeapEnd;

        /// <summary>
        /// Keep track of free block address and there size (contigous) in the memory
        /// These array should not free up, and hence resides in old heap space
        /// </summary>
        static uint[] BlockAddress;
        static uint[] BlockSize;

#warning [Heap] : "HeapManagerSize" is a constant
        static bool HeapManagerSetup;
        static int HeapManagerPosition;
        // ~16K items, complete assumption, so should take care of this
        const int HeapManagerSize = 1024 * 16;

        static uint HeapLock;

        internal static void Init(uint InitHeap)
        {
            HeapStart = InitHeap;
            HeapCurrent = InitHeap;
#warning [Heap] : "HeapEnd" and "HeapStart" are based on assumption for initial heap setup.
            HeapEnd = HeapStart + 0x100000;
            HeapManagerSetup = false;

            Debug.Write("Heap Initialized!!\n");
            Debug.Write("       Start Address::%d\n", InitHeap);
            Debug.Write("       End Address  ::%d\n", HeapEnd);

            // Allocate memory for future heap manager
            BlockSize = new uint[HeapManagerSize];
            BlockAddress = new uint[HeapManagerSize];
        }

        internal static void Setup(uint Start, uint End)
        {
            HeapStart = Start;
            HeapCurrent = Start;
            HeapEnd = End;

            Debug.Write("New Heap Setup!!\n");
            Debug.Write("       Start Address::%d\n", Start);
            Debug.Write("       End Address  ::%d\n", End);

            // Assign rest of memory as free
            BlockAddress[0] = HeapStart;
            BlockSize[0] = HeapEnd - HeapStart;
            HeapManagerPosition = 1;
            HeapManagerSetup = true;
        }

        [Label(Atomixilc.Helper.Heap_Label)]
        internal static uint kmalloc(uint len)
        {
            if (!HeapManagerSetup)
            {
                if (HeapCurrent + len > HeapEnd)
                {
                    Debug.Write("Memory out of run before real heap :(");
                }
                uint tmp = HeapCurrent;
                HeapCurrent += len;
                Memory.FastClear(tmp, len);
                return tmp;
            }
            else
            {
                var address = kmalloc(len, false);
                var CurrentTask = Scheduler.RunningThread;
                if (CurrentTask != null)
                {
                    var GC = CurrentTask.GC;
                    if (GC != null)
                        GC.Notify(address, len);
                }
                return address;
            }
        }

        internal static uint kmalloc(uint len, bool Aligned)
        {
            // If Heap Manager is not setup then use our old heap alogrithm -- Basically paging is calling this
            if (Aligned && !HeapManagerSetup)
            {
                if ((HeapCurrent & 0xFFFFF000) != HeapCurrent)
                {
                    HeapCurrent = (HeapCurrent & 0xFFFFF000) + 0x1000;
                }
                return kmalloc(len);
            }

            Monitor.AcquireLock(ref HeapLock);

            // Find a suitable hole
            int iterator;
            for (iterator = 0; iterator < HeapManagerPosition; iterator++)
            {
                if (Aligned)
                {
                    uint pos = BlockAddress[iterator];
                    uint size = BlockSize[iterator];
                    uint offset = 0;
                    if ((pos & 0xFFFFF000) != pos)
                    {
                        // Not aligned
                        offset = (pos & 0xFFFFF000) - pos + 0x1000;
                    }

                    // Check if we fit?
                    if (size >= len + offset)
                        break; // Yes :)
                }
                else if (BlockSize[iterator] >= len)
                    break; // Yes :)
            }

            if (iterator == HeapManagerPosition || HeapManagerPosition == HeapManagerSize) //No block to allocate :(
            {
                Debug.Write("Memory out of run :(\n");
                while (true) ;
            }

            // So, memory need to be aligned?
            if (Aligned)
                return malloc_aligned(iterator, len);

            // So we have a block, right?
            uint Address = BlockAddress[iterator];
            uint Size = BlockSize[iterator];
            if (Size > len) // we have to split the block
            {
                uint Add2 = Address + len;
                uint Size2 = Size - len;
                for (int i = 0; i <= iterator; i++)
                {
                    if (BlockSize[i] > Size2)
                    {
                        // here we have to put this
                        // Shift everything else
                        for (int j = iterator; j >= i + 1; j--)
                        {
                            BlockSize[j] = BlockSize[j - 1];
                            BlockAddress[j] = BlockAddress[j - 1];
                        }
                        BlockSize[i] = Size2; // Now put this element and come out
                        BlockAddress[i] = Add2;
                        break;
                    }
                }
            }
            else // we find a perfect size
            {
                // Remove this from free and return it
                iterator++;
                for (; iterator < HeapManagerPosition; iterator++)
                {
                    BlockAddress[iterator - 1] = BlockAddress[iterator];
                    BlockSize[iterator - 1] = BlockSize[iterator];
                }
                HeapManagerPosition--; // Reduce size of array, no need to clear last empty because we never read it
            }
            Monitor.ReleaseLock(ref HeapLock);
            Memory.FastClear(Address, len); // Clear the memory and return
            return Address;
        }

        private static uint malloc_aligned(int iterator, uint len)
        {
            uint Address = BlockAddress[iterator];
            uint Size = BlockSize[iterator];

            uint pos = Address;
            if ((Address & 0xFFFFF000) != Address)
            {
                pos = (Address & 0xFFFFF000) + 0x1000; // Align it first
            }

            uint NewSize = (pos - Address);
            if (NewSize != 0) // Maybe it is not aligned left, so mark left part free
            {
                for (int i = 0; i <= iterator; i++)
                {
                    if (BlockSize[i] > NewSize)
                    {
                        for (int j = iterator; j >= i + 1; j--)
                        {
                            BlockSize[j] = BlockSize[j - 1];
                            BlockAddress[j] = BlockAddress[j - 1];
                        }
                        BlockSize[i] = NewSize;
                        BlockAddress[i] = Address;
                        break;
                    }
                }
            }

            NewSize = Size - len - NewSize; // End block
            Address = pos + len;
            if (NewSize != 0) // Free up end part of this too
            {
                int i;
                for (i = 0; i <= iterator; i++)
                {
                    if (BlockSize[i] > NewSize)
                    {
                        for (int j = iterator; j >= i + 1; j--)
                        {
                            BlockSize[j] = BlockSize[j - 1];
                            BlockAddress[j] = BlockAddress[j - 1];
                        }
                        BlockSize[i] = NewSize;
                        BlockAddress[i] = Address;
                        break;
                    }
                }

                if (i > iterator)
                {
                    // we are at the end
                    BlockSize[HeapManagerPosition] = NewSize;
                    BlockAddress[HeapManagerPosition] = Address;
                    HeapManagerPosition++;
                }
            }
            Monitor.ReleaseLock(ref HeapLock);
            Memory.FastClear(pos, len);
            return pos;
        }

        [Label("__Heap_Free__")]
        internal static void Free(uint Address, uint len)
        {
            if (len == 0)
                return;

            Monitor.AcquireLock(ref HeapLock);

            // Check if any block can fit to left/Right of this
            int iterator, left = -1, right = -1;
            for (iterator = 0; iterator < HeapManagerPosition; iterator++)
            {
                uint Add = BlockAddress[iterator];
                uint Size = BlockSize[iterator];
                if (Add + Size == Address)
                {
                    left = iterator;
                }
                else if (Add == Address + len)
                {
                    right = iterator;
                }
            }

            // Compute new address and new size of block
            uint NewAddress = Address;
            uint NewSize = len;
            if (left != -1)
            {
                NewAddress = BlockAddress[left];
                NewSize += BlockSize[left];
            }

            if (right != -1)
            {
                NewSize += BlockSize[right];
            }

            // Remove left and right blocks
            int lastempty = 0;
            for (iterator = 0; iterator < HeapManagerPosition; iterator++)
            {
                if (iterator == left ||
                    iterator == right)
                    continue;
                BlockAddress[lastempty] = BlockAddress[iterator];
                BlockSize[lastempty] = BlockSize[iterator];
                lastempty++;
            }
            HeapManagerPosition = lastempty;

            // Add our new block to memory
            for (iterator = 0; iterator < HeapManagerPosition; iterator++)
            {
                if (BlockSize[iterator] > NewSize)
                {
                    // here we have to put this
                    // Shift everything else
                    for (int j = HeapManagerPosition; j >= iterator + 1; j--)
                    {
                        BlockSize[j] = BlockSize[j - 1];
                        BlockAddress[j] = BlockAddress[j - 1];
                    }
                    BlockSize[iterator] = NewSize; // Now put this element and come out
                    BlockAddress[iterator] = NewAddress;
                    HeapManagerPosition++;
                    iterator = -1; // Flag
                    break;
                }
            }
            if (iterator != -1) // End of loop
            {
                BlockSize[HeapManagerPosition] = NewSize;
                BlockAddress[HeapManagerPosition] = NewAddress;
                HeapManagerPosition++;
            }
            Monitor.ReleaseLock(ref HeapLock);
        }
    }
}
