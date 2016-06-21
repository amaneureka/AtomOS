/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
