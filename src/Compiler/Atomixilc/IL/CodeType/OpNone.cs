﻿using System;
using System.Reflection;

namespace Atomixilc.IL.CodeType
{
    internal class OpNone : OpCodeType
    {
        internal OpNone(ILCode aCode, int aPosition, int aNextPosition, ExceptionHandlingClause aEhc)
            : base(aCode, aPosition, aNextPosition, aEhc)
        {

        }

        public override string ToString()
        {
            return string.Format("{0} [0x{1}-0x{2}]", GetType().Name, Position.ToString("X4"), NextPosition.ToString("X4"));
        }
    }
}
