using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpString : OpCodeType
    {
        internal readonly string Value;

        internal OpString(ILCode aCode, int aPosition, int aNextPosition, string aValue, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
