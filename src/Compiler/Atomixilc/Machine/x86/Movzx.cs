﻿using System;
using System.Text;

namespace Atomixilc.Machine.x86
{
    public class Movzx : DestinationSourceSize
    {
        public Movzx()
            : base("movzx")
        {

        }

        public override string ToString()
        {
            if ((!string.IsNullOrEmpty(DestinationRef) && DestinationReg.HasValue) ||
                (string.IsNullOrEmpty(DestinationRef) && !DestinationReg.HasValue))
                throw new Exception(string.Format("{0} : Invalid Destination Parameters", Mnemonic));

            if ((!string.IsNullOrEmpty(SourceRef) && SourceReg.HasValue) ||
                (string.IsNullOrEmpty(SourceRef) && !SourceReg.HasValue))
                throw new Exception(string.Format("{0} : Invalid Source Parameters", Mnemonic));

            var SB = new StringBuilder();
            SB.Append(Mnemonic);
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

            SB.Append(", ");

            SB.Append(Helper.SizeToString(Size));
            SB.Append(' ');

            if (SourceIndirect)
                SB.Append('[');

            if (SourceReg.HasValue)
                SB.Append(SourceReg.Value);
            else
                SB.Append(SourceRef);

            if (SourceDisplacement.HasValue && SourceDisplacement != 0)
            {
                int offset = SourceDisplacement.Value;
                if (offset < 0)
                    SB.Append(" - ");
                else
                    SB.Append(" + ");
                SB.Append(Math.Abs(offset));
            }

            if (SourceIndirect)
                SB.Append(']');

            return SB.ToString();
        }
    }
}
