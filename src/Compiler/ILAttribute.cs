/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Implemented MSIL Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ILOpAttribute : Attribute
    {
        public readonly ILCode ILCode;

        // Just its presense is necessary
        public ILOpAttribute(ILCode aIL)
        {
            ILCode = aIL;
        }
    }
}
