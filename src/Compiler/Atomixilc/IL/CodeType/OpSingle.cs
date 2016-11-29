using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpSingle : OpCodeType
    {
        internal readonly float Value;

        internal OpSingle(ILCode aCode, int aPosition, int aNextPosition, float aValue, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
