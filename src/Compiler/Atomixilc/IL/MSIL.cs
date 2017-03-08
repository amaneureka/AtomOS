/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          MSIL Abstract Type
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
