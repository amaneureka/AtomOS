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

        public string Key
        { get { return mKey; } }

        public AsmData(string aKey)
        {
            mKey = aKey;
        }

        public AsmData(string aKey, uint aData)
        {
            mKey = aKey;
            mData = new string[1];
            mData[0] = aData.ToString();
        }

        public AsmData(string aKey, uint[] aData)
        {
            mKey = aKey;
            mData = aData.Select(a => a.ToString()).ToArray();
        }

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
            else
                return mKey;
        }
    }
}
