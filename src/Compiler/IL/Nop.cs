/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
