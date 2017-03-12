/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Garbage Collector
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Attributes;

namespace Atomixilc.Lib
{
    internal unsafe static class GC
    {
        static uint mStack;
        static uint mStackSize;

        static int mAllocatedObjectCount;

        static uint[] mAllocatedObjects;        
        static uint[] mAllocatedObjectSize;

        const int MaximumObjectCount = 1024 * 16;

        internal static void Init(uint aStack, uint aStackSize)
        {
            mStack = aStack;
            mStackSize = aStackSize;

            mAllocatedObjectCount = 0;
            mAllocatedObjects = new uint[MaximumObjectCount];
            mAllocatedObjectSize = new uint[MaximumObjectCount];
        }

        [Plug("System_Void_System_GC_Collect__")]
        internal static unsafe void Collect()
        {
            uint pointer = Native.GetStackPointer();
            if (pointer >= mStack || pointer + mStackSize <= mStack)
                return;

            SortObjects();
            UnmarkObjects();

            // trace stack
            uint limit = mStack;
            while (pointer < limit)
            {
                MarkObject(*(uint*)pointer);
                pointer += 4;
            }

            // trace global
        }

        private static void UnmarkObjects()
        {
            int count = mAllocatedObjectCount;
            for (int i = 0; i < count; i++)
                mAllocatedObjectSize[i] &= 0x7FFFFFFF;
        }

        private static void MarkObject(uint Address)
        {
            int index = BinarySearch(Address);
            // no such object found
            if (index == -1) return;

            // mark if not marked
            if ((mAllocatedObjectSize[index] & (1U << 31)) != 0) return;
            mAllocatedObjectSize[index] |= 1U << 31;

            var data = (uint*)Address;
            uint flag = data[1];

            // check object flag
            if ((flag & 0x3) != 0x1) return;

            // find sub-fields and mark them
            uint childrens = flag >> 2;
            data += 3;
            while(childrens > 0)
            {
                MarkObject(*data);
                data++;
                childrens--;
            }
        }
        
        private static int BinarySearch(uint Address)
        {
            if (Address == 0) return -1;
            int left = 0, right = mAllocatedObjectCount - 1;
            while(left <= right)
            {
                int mid = (left + right) >> 1;
                uint found = mAllocatedObjects[mid];

                if (found == Address)
                    return mid;

                if (mAllocatedObjects[mid] > Address)
                    right = mid - 1;
                else
                    left = mid + 1;
            }

            return -1;
        }

        private static void SortObjects()
        {
            int count = mAllocatedObjectCount;
            for (int i = 1; i < count; i++)
            {
                int j = i - 1;
                uint address = mAllocatedObjects[i], length = mAllocatedObjectSize[i];
                while (j >= 0 && mAllocatedObjects[j] > address)
                    j--;
                j++;
                int k = i - 1;
                while (k >= j)
                {
                    mAllocatedObjects[k + 1] = mAllocatedObjects[k];
                    mAllocatedObjectSize[k + 1] = mAllocatedObjectSize[k];
                    k--;
                }
                mAllocatedObjects[j] = address;
                mAllocatedObjectSize[j] = length;
            }
        }
    }
}
