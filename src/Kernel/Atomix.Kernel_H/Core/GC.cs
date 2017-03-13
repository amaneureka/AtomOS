/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Garbage Collector
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

namespace Atomix.Kernel_H.Core
{
    internal unsafe class GC
    {
        static uint mStack;

        static int mAllocatedObjectCount;

        static uint[] mAllocatedObjects;
        static uint[] mAllocatedObjectSize;

        internal GC(uint aStack, uint aMaximumObjectCount = 1024)
        {
            mStack = aStack;

            mAllocatedObjectCount = 0;
            mAllocatedObjects = new uint[aMaximumObjectCount];
            mAllocatedObjectSize = new uint[aMaximumObjectCount];
        }

        internal bool Notify(uint aAddress, uint aLength)
        {
            if (mAllocatedObjectCount == mAllocatedObjects.Length)
                return false;

            Debug.Write("Allocate: %d ", aAddress);
            Debug.Write(" %d\n", aLength);

            // add the object to pool and update counter
            int index = mAllocatedObjectCount;
            mAllocatedObjects[index] = aAddress;
            mAllocatedObjectSize[index] = aLength;
            mAllocatedObjectCount = index + 1;

            return true;
        }

        internal void Collect()
        {
            Debug.Write("Collect\n");
            uint pointer = Native.GetStackPointer();

            SortObjects();

            // umark objects
            int count = mAllocatedObjectCount;
            for (int i = 0; i < count; i++)
                mAllocatedObjectSize[i] &= 0x7FFFFFFF;

            // trace stack
            uint limit = mStack;
            while (pointer < limit)
            {
                MarkObject(*(uint*)pointer);
                pointer += 4;
            }

            // trace global
            uint start = Native.GlobalVarStart();
            uint end = Native.GlobalVarEnd();
            while (start < end)
            {
                MarkObject(*(uint*)start);
                start += 4;
            }

            // free unmarked objects
            int index = 0;
            for (int i = 0; i < count; i++)
            {
                if ((mAllocatedObjectSize[i] & (1U << 31)) == 0)
                {
                    Debug.Write("Free: %d ", mAllocatedObjects[i]);
                    Debug.Write(" %d\n", mAllocatedObjectSize[i]);
                    Heap.Free(mAllocatedObjects[i], mAllocatedObjectSize[i]);
                }
                else
                {
                    mAllocatedObjects[index] = mAllocatedObjects[i];
                    mAllocatedObjectSize[index] = mAllocatedObjectSize[i];
                    index++;
                }
            }

            mAllocatedObjectCount = index;
        }

        public void Dump()
        {
            Debug.Write("GC Dump()\n");
            int count = mAllocatedObjectCount;
            for (int i = 0; i < count; i++)
            {
                Debug.Write("%d ", mAllocatedObjects[i]);
                Debug.Write("%d\n", mAllocatedObjectSize[i] & 0x7fffffff);
            }
        }

        private void MarkObject(uint Address)
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
            if ((flag & 0x3) != 0x1)
            {
                if ((flag & 0x3) == 0x3)
                {
                    int lastIndex = (int)data[2] + 4;
                    for (int i = 4; i < lastIndex; i++)
                        MarkObject(data[i]);
                }
                return;
            }

            // find sub-fields and mark them
            uint childrens = flag >> 2;
            data += 3;
            while (childrens > 0)
            {
                MarkObject(*data);
                data++;
                childrens--;
            }
        }

        private int BinarySearch(uint Address)
        {
            if (Address == 0) return -1;
            int left = 0, right = mAllocatedObjectCount - 1;
            while (left <= right)
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

        private void SortObjects()
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
