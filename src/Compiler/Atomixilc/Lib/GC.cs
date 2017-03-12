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

        internal static void Init(uint aStack, uint aStackSize)
        {
            mStack = aStack;
            mStackSize = aStackSize;
        }

        [Plug("System_Void_System_GC_Collect__")]
        internal static void Collect()
        {
            uint pointer = Native.GetStackPointer();
            if (pointer >= mStack || pointer + mStackSize <= mStack)
                return;

            for (uint index = pointer; index < mStack; index += 4)
            {
                
            }
        }
    }
}
