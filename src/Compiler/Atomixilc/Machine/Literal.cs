using System;

namespace Atomixilc.Machine
{
    public class Literal : Instruction
    {
        string mCode;

        public Literal(string aCode)
            : base(string.Empty)
        {
            mCode = aCode;
        }

        public Literal(string aCode, params object[] aParams)
            : base(string.Empty)
        {
            mCode = string.Format(aCode, aParams);
        }

        public override string ToString()
        {
            return mCode;
        }
    }
}
