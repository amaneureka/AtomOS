/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler MSIL Implementation Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class ILImplAttribute : Attribute
    {
        internal readonly ILCode OpCode;

        internal ILImplAttribute(ILCode aOpCode)
        {
            OpCode = aOpCode;
        }
    }
}
