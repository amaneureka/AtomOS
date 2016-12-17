/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Field Value MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpField : OpCodeType
    {
        internal readonly FieldInfo Value;

        internal OpField(ILCode aCode, int aPosition, int aNextPosition, FieldInfo aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }

        public override string ToString()
        {
            return string.Format("{0} [0x{1}-0x{2}] {3}", GetType().Name, Position.ToString("X4"), NextPosition.ToString("X4"), Value);
        }
    }
}
