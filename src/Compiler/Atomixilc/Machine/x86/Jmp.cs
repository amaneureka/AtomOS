/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Jmp Instruction
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine.x86
{
    public class Jmp : Instruction
    {
        public ConditionalJump? Condition;
        public string DestinationRef;
        public ushort? Selector;

        string ParentLabel;

        public Jmp()
            :base("jmp")
        {
            ParentLabel = Label.Primary;
        }

        public override string ToString()
        {
            if (!Condition.HasValue)
                Condition = ConditionalJump.JMP;

            if (DestinationRef.StartsWith("."))
                DestinationRef = ParentLabel + DestinationRef;

            if (Selector.HasValue)
                DestinationRef = string.Format("{0}:{1}", Selector, DestinationRef);

            if (Condition == ConditionalJump.JMP)
                return string.Format("jmp {0}", DestinationRef);

            return string.Format("{0} near {1}", Condition.ToString().ToLower(), DestinationRef);
        }
    }
}
