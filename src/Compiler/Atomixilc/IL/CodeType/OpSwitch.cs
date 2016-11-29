using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpSwitch : OpCodeType
    {
        internal readonly int[] Value;

        internal OpSwitch(ILCode aCode, int aPosition, int aNextPosition, int[] aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
