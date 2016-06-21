/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Nop MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.IL
{
    [ILOp(ILCode.Nop)]
    public class Nop : MSIL
    {
        /* Do nothing (No operation).*/
        public Nop(Compiler Cmp)
            : base("nop", Cmp) { }
    }
}
