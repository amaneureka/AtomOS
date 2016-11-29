using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpSig : OpCodeType
    {
        internal readonly int Value;

        internal OpSig(ILCode aCode, int aPosition, int aNextPosition, int aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
