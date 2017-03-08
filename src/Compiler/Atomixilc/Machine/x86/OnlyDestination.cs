/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Destination-Size type set Instructions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Text;

namespace Atomixilc.Machine.x86
{
    public class OnlyDestination : Instruction
    {
        public string DestinationRef;
        public Register? DestinationReg;
        public bool DestinationIndirect;
        public int? DestinationDisplacement;

        public byte Size;

        public OnlyDestination(string aMnemonic)
            :base(aMnemonic)
        {
            Size = 32;
        }

        public override string ToString()
        {
            if ((!string.IsNullOrEmpty(DestinationRef) && DestinationReg.HasValue) ||
                (string.IsNullOrEmpty(DestinationRef) && !DestinationReg.HasValue))
                throw new Exception(string.Format("{0} : Invalid Destination Parameters", Mnemonic));

            var SB = new StringBuilder();
            SB.Append(Mnemonic);
            SB.Append(' ');
            SB.Append(Helper.SizeToString(Size));
            SB.Append(' ');

            if (DestinationIndirect)
                SB.Append('[');

            if (DestinationReg.HasValue)
                SB.Append(DestinationReg.Value);
            else
                SB.Append(DestinationRef);

            if (DestinationDisplacement.HasValue && DestinationDisplacement != 0)
            {
                int offset = DestinationDisplacement.Value;
                if (offset < 0)
                    SB.Append(" - ");
                else
                    SB.Append(" + ");
                SB.Append(Math.Abs(offset));
            }

            if (DestinationIndirect)
                SB.Append(']');

            return SB.ToString();
        }
    }
}
