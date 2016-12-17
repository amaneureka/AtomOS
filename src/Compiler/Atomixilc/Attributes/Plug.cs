/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Method Plug Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PlugAttribute : Attribute
    {
        public readonly string TargetLabel;
        public readonly Architecture Platform;

        public PlugAttribute(string label, Architecture arch = Architecture.x86)
        {
            TargetLabel = label;
            Platform = arch;
        }
    }
}
