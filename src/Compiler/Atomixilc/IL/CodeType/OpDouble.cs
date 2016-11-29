using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpDouble : OpCodeType
    {
        internal readonly double Value;

        internal OpDouble(ILCode aCode, int aPosition, int aNextPosition, double aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
