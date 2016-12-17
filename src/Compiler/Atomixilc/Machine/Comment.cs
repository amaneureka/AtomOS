/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Assembly Comment
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomixilc.Machine
{
    public class Comment : Instruction
    {
        string mMessage;

        public Comment(string aMessage)
            : base(string.Empty)
        {
            mMessage = aMessage;
        }

        public override string ToString()
        {
            return string.Format("; {0}", mMessage);
        }
    }
}
