/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Kernel Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelAttribute : Attribute
    {
        public readonly CPUArch CPUArch;
        public readonly string Organize;

        public KernelAttribute(CPUArch aCpuArch, string aOrganize)
        {
            CPUArch = aCpuArch;
            Organize = aOrganize;
        }
    }
}
