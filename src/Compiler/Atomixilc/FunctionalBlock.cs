using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class FunctionalBlock
    {
        string mName;
        Architecture mPlatform;
        List<Instruction> mBody;
        CallingConvention mCallingConvention;

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal Architecture Platform
        {
            get { return mPlatform; }
        }

        internal List<Instruction> Body
        {
            get
            {
                if (mBody == null)
                    mBody = new List<Instruction>();
                return mBody;
            }
        }

        internal CallingConvention CallingConvention
        {
            get { return mCallingConvention; }
        }

        internal FunctionalBlock(string name, Architecture arch, CallingConvention callc)
        {
            mName = name;
            mPlatform = arch;
            mCallingConvention = callc;
        }
    }
}
