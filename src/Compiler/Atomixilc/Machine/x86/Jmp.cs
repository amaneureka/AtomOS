using System;

namespace Atomixilc.Machine.x86
{
    public class Jmp : Instruction
    {
        public ConditionalJump? Condition;
        public string DestinationRef;

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

            if (Condition == ConditionalJump.JMP)
                return string.Format("jmp {0}", DestinationRef);

            return string.Format("{0} near {1}", Condition.ToString().ToLower(), DestinationRef);
        }
    }
}
