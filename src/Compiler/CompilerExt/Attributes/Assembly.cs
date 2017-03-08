/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
