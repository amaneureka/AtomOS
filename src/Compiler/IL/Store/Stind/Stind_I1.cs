/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Stind_I1 MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.Reflection;

namespace Atomix.IL
{
    [ILOp(ILCode.Stind_I1)]
    public class Stind_I1 : MSIL
    {
        public Stind_I1(Compiler Cmp)
            : base("stindi1", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            Stind_I.Stind_All(1);
        }
    }
}
