/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Label Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix.CompilerExt.Attributes
{
    [AttributeUsage(AttributeTargets.Method , AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        public readonly string Label;

        public LabelAttribute(string aLabel)
        {
            Label = aLabel;
        }
    }
}
