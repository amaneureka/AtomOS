using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atomixilc.IL
{
    internal abstract class MSIL
    {
        internal abstract void Execute(OpCodeType xOp, MethodBase method, Stack<StackItem> vStack);
    }
}
