using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpInt : OpCodeType
    {
        internal readonly int Value;

        internal OpInt(ILCode aCode, int aPosition, int aNextPosition, int aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
