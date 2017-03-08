/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Call Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Call : Instruction
    {
        public bool IsLabel;
        public string DestinationRef;

        public Call()
            : base("call")
        {

        }

        public override string ToString()
        {
            if (DestinationRef.StartsWith("."))
                DestinationRef = Label.Primary + DestinationRef;

            return string.Format("call {0}", DestinationRef);
        }
    }
}
