using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpNone : OpCodeType
    {
        internal OpNone(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {

        }
    }
}
