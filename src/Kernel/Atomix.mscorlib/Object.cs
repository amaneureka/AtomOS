using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.mscorlib
{
    public static class Object
    {
        [Plug("System_Void__System_Object__ctor__")]
        public static void Cctor(object obj)
        {
            return;
        }
        [Plug("System_Type_System_Object_GetType__")]
        public static Type GetType(object obj)
        {
            return null;
        }
    }
}
