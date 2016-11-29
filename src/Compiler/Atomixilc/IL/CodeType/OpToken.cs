using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpToken : OpCodeType
    {
        internal readonly int Value;

        internal bool IsType
        {
            get
            {
                if (((Value & 0x02000000) != 0)
                    || ((Value & 0x01000000) != 0)
                    || ((Value & 0x1B000000) != 0))
                    return true;
                return false;
            }
        }

        internal bool IsField
        {
            get
            {
                return ((Value & 0x04000000) != 0);
            }
        }

        internal OpToken(ILCode aCode, int aPosition, int aNextPosition, int aValue, Module aModule, Type[] aTypeGenericArgs, Type[] aMethodGenericArgs, ExceptionHandlingClause aEhc)
            :base(aCode, aPosition, aNextPosition, aEhc)
        {
            Value = aValue;
        }
    }
}
