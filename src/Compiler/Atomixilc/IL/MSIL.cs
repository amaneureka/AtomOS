/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
