using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class SRImpl
    {
        [Plug("System_String_System_SR_GetString_System_String_")]
        public static string GetString(string aString)
        {
            return aString;
        }
    }
}
