using System;

using Atomix.CompilerExt.Attributes;

using libAtomixH.IO.Ports;

namespace libAtomixH.Core
{
    public static class Heap
    {
        private static uint pointer;
        private static uint _start;

        [Label ("Heap")]
        public static uint AllocateMem (uint aLength)
        {
            //This is temp.
            if (_start == 0)
                _start = Native.EndOfKernel ();

            uint xResult = pointer;
            pointer += aLength;
            Memory.Clear (xResult, aLength);

            return xResult;
        }
    }
}
