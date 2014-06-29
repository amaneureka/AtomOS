using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Atomix.Assembler.x86
{
    public abstract class DestinationSource : Instruction
    {
        public Registers? DestinationReg;
        public string DestinationRef;
        public bool DestinationIndirect;
        public int? DestinationDisplacement;

        public Registers? SourceReg;
        public string SourceRef;
        public bool SourceIndirect;
        public int? SourceDisplacement;

        public DestinationSource(string aM)
            : base(aM)  { }

        public override void FlushText(StreamWriter sw)
        {
            string des = DestinationReg.HasValue ? DestinationReg.ToString() : DestinationRef;
            string src = SourceReg.HasValue ? SourceReg.ToString() : SourceRef;

            if (DestinationDisplacement > 0)
                des = des + " + 0x" + ((uint)DestinationDisplacement).ToString("X");
            else if (DestinationDisplacement < 0)
                des = des + " - 0x" + ((int)(-1 * DestinationDisplacement)).ToString("X");

            if (SourceDisplacement > 0)
                src = src + " + 0x" + ((uint)SourceDisplacement).ToString("X");
            else if (SourceDisplacement < 0)
                src = src + " - 0x" + ((int)(-1 * SourceDisplacement)).ToString("X");

            if (DestinationIndirect)
                des = "[" + des + "]";

            if (SourceIndirect)
                src = "[" + src + "]";

            sw.WriteLine(string.Format("{0} {1}, {2}", Code, des, src));
        }
    }
}
