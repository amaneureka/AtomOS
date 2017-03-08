/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
