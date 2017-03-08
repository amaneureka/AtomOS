/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Setg Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Setg : Instruction
    {
        public Register DestinationReg;

        public Setg()
            : base("setg")
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Mnemonic, DestinationReg);
        }
    }
}
