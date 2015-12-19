using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler.x86
{
    public class Call : Instruction        
    {
        public readonly string Address;
        public readonly bool FunctionLabel;

        public Call(string Add, bool aFunctionLabel = false)
            :base ("call")
        {
            this.Address = Add;
            //If FunctionLabel is set then it will look into Label Dictionary for real address
            this.FunctionLabel = aFunctionLabel;
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine("call " + Address);
        }

        public static void FlushText(StreamWriter sw, string mAddress)
        {
            sw.WriteLine("call " + mAddress);
        }
    }
}
