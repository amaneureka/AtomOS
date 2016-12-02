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

        internal static void Swap(ref StackItem A, ref StackItem B)
        {
            StackItem temp = A;
            A = B;
            B = temp;
        }
    }
}
