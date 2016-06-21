/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
