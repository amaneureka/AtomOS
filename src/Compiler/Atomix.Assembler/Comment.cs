/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          assembly comment
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System.IO;

namespace Atomix.Assembler
{
    public class Comment : Instruction
    {
        public readonly string Comments;

        public Comment(string aComment)
            :base ("Comment")
        {
            Comments = aComment;
        }

        public override void FlushText(StreamWriter sw)
        {
            sw.WriteLine("; " + Comments);
        }
    }
}
