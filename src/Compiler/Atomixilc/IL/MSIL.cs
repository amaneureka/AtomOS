using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atomixilc.IL
{
    internal abstract class MSIL
    {
        internal ILCode msIL;

        internal MSIL(ILCode aILCode)
        {
            msIL = aILCode;
        }

        internal abstract void Execute(Options Config, OpCodeType xOp, MethodBase method, Optimizer Optimizer);
    }
}
