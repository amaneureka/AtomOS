using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace libAtomixH.mscorlib.System
{
    public static class ConsoleKeyInfo
    {
        private static char keychar;

        [Plug ("System_Char_System_ConsoleKeyInfo_get_KeyChar__", CPUArch.x86)]
        public static char KeyChar ()
        {
            return keychar;
        }

        [Plug ("System_Void__System_ConsoleKeyInfo__ctor_")]
        public static void ctor ()
        {

        }
    }
}
