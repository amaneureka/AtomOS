/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          OpCodeType Abstract Type
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Reflection;

namespace Atomixilc.IL
{
    internal abstract class OpCodeType : IComparable<OpCodeType>
    {
        internal readonly ILCode ILCode;
        internal readonly int Position;
        internal readonly int NextPosition;
        internal readonly ExceptionHandlingClause Handler;
        internal readonly bool NeedHandler;
        internal readonly int HandlerPosition;
        internal readonly string HandlerRef;

        internal bool IsLastIL;

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
            HandlerPosition = -1;
            if (aEhc != null && aEhc.HandlerOffset > Position)
            {
                HandlerPosition = aEhc.HandlerOffset;
                HandlerRef = Helper.GetLabel(aEhc.HandlerOffset);
            }
        }

        public int CompareTo(OpCodeType aOther)
        {
            return Position.CompareTo(aOther.Position);
        }
    }
}
