/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          add x86 instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Assembler.x86
{
    public class Add : DestinationSourceSize
    {
        public Add()
            : base("add") { }
    }
}
