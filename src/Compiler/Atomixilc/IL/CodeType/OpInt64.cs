using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpInt64 : OpCodeType
    {
        internal readonly long Value;

        internal OpInt64(ILCode aCode, int aPosition, int aNextPosition, long aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
