using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib.Globalization
{
    public static unsafe class CultureInfo
    {
        [Plug("System_Void__System_Globalization_CultureInfo__cctor__")]
        public static void Cctor(byte* aAddress)
        {
            return;
        }
    }
}
