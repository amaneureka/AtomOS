using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    internal static class NativeException
    {
        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException_System_ExceptionArgument__System_ExceptionResource_")]
        internal static void ThrowHelper(int Argument, int Resource)
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowArgumentOutOfRangeException__")]
        internal static void ThrowArgumentOutOfRangeException()
        {
            return;
        }

        [Plug("System_Void_System_ThrowHelper_ThrowInvalidOperationException_System_ExceptionResource_")]
        internal static void ThrowInvalidOperationException(int ExceptionResource)
        {
            return;
        }
    }
}
