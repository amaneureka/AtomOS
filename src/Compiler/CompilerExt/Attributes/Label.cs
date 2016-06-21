/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Label Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.All , AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        public readonly string Label;
        
        public LabelAttribute(string aLabel)
        {
            Label = aLabel;
        }
    }
}
