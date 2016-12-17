/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
