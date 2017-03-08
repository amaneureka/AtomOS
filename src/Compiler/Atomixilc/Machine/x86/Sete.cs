/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Sete Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Sete : Instruction
    {
        public Register DestinationReg;

        public Sete()
            : base("sete")
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Mnemonic, DestinationReg);
        }
    }
}
