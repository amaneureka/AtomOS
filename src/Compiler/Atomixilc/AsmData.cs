using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomixilc
{
    public class AsmData
    {
        string mKey;
        byte[] mRawData;
        string[] mData;

        public AsmData(string aKey, byte[] aData)
        {
            mKey = aKey;
            mRawData = aData;
        }

        public AsmData(string aKey, string[] aData)
        {
            mKey = aKey;
            mData = aData;
        }

        public override string ToString()
        {
            if (mRawData != null)
                return string.Format("{0} db {1}", mKey, string.Join(", ", mRawData.Select(a => a.ToString())));
            else if (mData != null)
                return string.Format("{0} dd {1}", mKey, string.Join(", ", mData.Select(a => a.ToString())));
            throw new Exception("Invalid use of AsmData");
        }
    }
}
