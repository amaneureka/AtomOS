/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
