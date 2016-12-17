/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
