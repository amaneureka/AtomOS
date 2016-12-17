/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
