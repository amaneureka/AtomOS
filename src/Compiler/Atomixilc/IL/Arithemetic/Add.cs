using System;
using System.Collections.Generic;
using System.Reflection;

using Atomixilc.Attributes;

namespace Atomixilc.IL
{
    //[ILImpl(ILCode.Add)]
    internal class Add_il : MSIL
    {
        internal override void Execute(OpCodeType xOp, MethodBase method, Stack<StackItem> vStack)
        {
            throw new NotImplementedException();
        }
    }
}
