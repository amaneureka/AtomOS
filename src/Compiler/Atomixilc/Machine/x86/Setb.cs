/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Setb Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Setb : Instruction
    {
        public Register DestinationReg;

        public Setb()
            : base("setb")
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Mnemonic, DestinationReg);
        }
    }
}
