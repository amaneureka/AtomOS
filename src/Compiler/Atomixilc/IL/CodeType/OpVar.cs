using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpVar : OpCodeType
    {
        internal readonly ushort Value;

        internal OpVar(ILCode aCode, int aPosition, int aNextPosition, ushort aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
