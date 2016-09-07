/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
