using System;
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
    }
}
