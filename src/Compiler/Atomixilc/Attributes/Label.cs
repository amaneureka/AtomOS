/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Method Label Identifier Attribute
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomixilc.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class LabelAttribute : Attribute
    {
        public readonly string RefLabel;

        public LabelAttribute(string label)
        {
            RefLabel = label;
        }
    }
}
