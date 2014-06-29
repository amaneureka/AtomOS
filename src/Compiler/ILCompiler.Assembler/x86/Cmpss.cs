using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;
using Atomix.Assembler;

namespace Atomix.Assembler.x86
{
    public class Cmpss : DestinationSource
    {
        public byte pseudoCode { get; set; }

        public Cmpss()
            : base("cmpss") { }

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

            sw.WriteLine(string.Format("{0} {1}, {2}, 0x{3}", Code, des, src, pseudoCode.ToString("X")));
        }
    }
    public enum ComparePseudoOpcodes : byte
    {
        Equal = 0,
        LessThan = 1,
        LessThanOrEqualTo = 2,
        Unordered = 3,
        NotEqual = 4,
        NotLessThan = 5,
        NotLessThanOrEqualTo = 6,
        Ordered = 7
    }
}
