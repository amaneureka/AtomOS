/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Setne Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Setne : Instruction
    {
        public Register DestinationReg;

        public Setne()
            : base("setne")
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Mnemonic, DestinationReg);
        }
    }
}
