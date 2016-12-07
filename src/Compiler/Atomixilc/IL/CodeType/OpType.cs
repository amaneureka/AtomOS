﻿using System;
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

        public override string ToString()
        {
            return string.Format("{0} [0x{1}-0x{2}] {3}", GetType().Name, Position.ToString("X4"), NextPosition.ToString("X4"), Value);
        }
    }
}
