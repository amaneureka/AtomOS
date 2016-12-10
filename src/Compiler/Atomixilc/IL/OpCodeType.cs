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
        internal readonly bool NeedHandler;
        internal readonly string HandlerRef;

        internal OpCodeType(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
        {
            ILCode = aCode;
            Position = aPosition;
            NextPosition = aNextPosition;
            Handler = aEhc;

            NeedHandler = (aEhc != null)
                && ((aEhc.HandlerOffset == Position) || (((aEhc.Flags & ExceptionHandlingClauseOptions.Filter) != 0) && aEhc.FilterOffset == Position))
                && (aEhc.Flags == ExceptionHandlingClauseOptions.Clause);

            HandlerRef = ".Error";
            if (aEhc != null && aEhc.HandlerOffset > Position)
                HandlerRef = Helper.GetLabel(aEhc.HandlerOffset);
        }
    }
}
