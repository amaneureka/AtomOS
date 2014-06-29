using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class Enum
    {
        [Plug("System_Void__System_Enum__cctor__")]
        public static void Cctor()
        {
            return;
        }
    }
}
