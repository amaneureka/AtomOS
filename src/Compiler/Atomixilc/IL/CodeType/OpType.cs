using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpType : OpCodeType
    {
        internal readonly Type Value;

        internal OpType(ILCode aCode, int aPosition, int aNextPosition, Type aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
