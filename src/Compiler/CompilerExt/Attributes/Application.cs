/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Application Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ApplicationAttribute : Attribute
    {
        public readonly CPUArch CPUArch;

        public ApplicationAttribute(CPUArch aCpuArch = CPUArch.x86)
        {
            CPUArch = aCpuArch;
        }
    }
}
