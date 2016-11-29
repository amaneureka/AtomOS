using System;
using System.Reflection;

namespace Atomixilc.IL
{
    internal abstract class OpCodeType
    {
        internal readonly ILCode ILCode;
        internal readonly int Position;
        internal readonly int NextPosition;
        internal readonly ExceptionHandlingClause Handler;

        internal OpCodeType(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
        {
            ILCode = aCode;
            Position = aPosition;
            NextPosition = aNextPosition;
            Handler = aEhc;
        }
    }
}
