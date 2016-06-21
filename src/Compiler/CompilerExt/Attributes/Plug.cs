/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
