/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Assembly Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AssemblyAttribute : Attribute
    {
        public readonly bool NeedCalliHeader;

        public AssemblyAttribute(bool aNeedCalliHeader = false)
        {
            NeedCalliHeader = aNeedCalliHeader;
        }
    }
}
