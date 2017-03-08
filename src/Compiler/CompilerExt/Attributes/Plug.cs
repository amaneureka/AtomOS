/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Plug Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlugAttribute : Attribute
    {
        protected CPUArch CPUArch;
        protected string TargetString;

        public PlugAttribute(string aTargetSymbol, CPUArch aCpuArch = CPUArch.x86)
        {
            TargetString = aTargetSymbol;
            CPUArch = aCpuArch;
        }
    }
}
