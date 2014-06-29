using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Atomix.CompilerExt;

namespace Atomix.Assembler
{
    public class Comment : Instruction        
    {
        public readonly string Comments;

        public Comment(string aComment)
            :base ("Comment")
        {
            this.Comments = aComment;
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine("; " + Comments);
        }
    }
}
