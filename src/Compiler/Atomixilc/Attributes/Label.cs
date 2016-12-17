/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
