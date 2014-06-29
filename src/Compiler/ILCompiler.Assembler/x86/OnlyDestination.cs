using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Atomix.Assembler.x86
{
    public abstract class OnlyDestination : Instruction
    {
        public Registers? DestinationReg;
        public string DestinationRef;
        public bool DestinationIndirect;
        public int? DestinationDisplacement;        
        public byte Size;

        public OnlyDestination(string aM)
            : base(aM)  { }

        public override void FlushText(StreamWriter sw)
        {
            string des = DestinationReg.HasValue ? DestinationReg.ToString() : DestinationRef;

            if (DestinationDisplacement > 0)
                des = des + " + 0x" + ((uint)DestinationDisplacement).ToString("X");
            else if (DestinationDisplacement < 0)
                des = des + " - 0x" + ((int)(-1 * DestinationDisplacement)).ToString("X");

            if (DestinationIndirect)
                des = "[" + des + "]";

            sw.WriteLine(string.Format("{0} {2} {1}", Code, des, Const.SizeToString(Size)));
        }
    }
}
