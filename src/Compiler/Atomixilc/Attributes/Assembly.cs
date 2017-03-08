/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Method Assembly Stub Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AssemblyAttribute : Attribute
    {
        public readonly bool CalliHeader;

        public AssemblyAttribute(bool requiredCallingHeader = false)
        {
            CalliHeader = requiredCallingHeader;
        }
    }
}
