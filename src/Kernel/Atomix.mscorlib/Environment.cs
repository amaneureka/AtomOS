using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using System.Globalization;

namespace Atomix.mscorlib
{
    public static class Environment
    {
        [Plug("System_String_System_Environment_GetResourceFromDefault_System_String_")]
        public static string GetResourceFromDefault(string aResource)
        {
            return aResource;
        }
        [Plug("System_String_System_Resources_ResourceManager_GetString_System_String__System_Globalization_CultureInfo_")]
        public static string GetString(string aResource, CultureInfo aCultureInfo)
        {
            return aResource;
        }
    }
}
