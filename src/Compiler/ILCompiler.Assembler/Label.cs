using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler
{
    public class Label : Instruction        
    {
        public static string PrimaryLabel;
        public readonly string Name;
        public readonly string FinalisedName;

        public Label(string aName)
            :base ("Label")
        {
            this.Name = aName;
            if (aName.StartsWith("."))
            {
                this.FinalisedName = PrimaryLabel + Name;
            }
            else
            {
                this.FinalisedName = aName;
                PrimaryLabel = FinalisedName;
            }
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine(FinalisedName + ": ");
        }
    }
}
