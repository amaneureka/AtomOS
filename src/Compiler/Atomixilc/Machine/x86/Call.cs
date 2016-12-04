using System;

namespace Atomixilc.Machine.x86
{
    public class Call : Instruction
    {
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
