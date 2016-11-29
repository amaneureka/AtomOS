using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpMethod : OpCodeType
    {
        internal readonly MethodBase Value;

        internal OpMethod(ILCode aCode, int aPosition, int aNextPosition, MethodBase aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
