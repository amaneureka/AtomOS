using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.plugs
{
    public static class NativeException
    {
        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException_System_ExceptionArgument__System_ExceptionResource_")]
        public static void ThrowHelper(int Argument, int Resource)
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException__")]
        public static void ThrowArgumentOutOfRangeException()
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowInvalidOperationException_System_ExceptionResource_")]
        public static void ThrowInvalidOperationException(int ExceptionResource)
        {
            return;
        }
    }
}
