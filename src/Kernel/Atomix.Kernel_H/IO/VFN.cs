/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Virtual File Node
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;

namespace Atomix.Kernel_H.IO
{
    internal class VFN : Directory
    {
        IDictionary<string, FSObject> mEntries;

        internal VFN(string aName)
            : base(aName)
        {
            mEntries = new IDictionary<string, FSObject>(Internals.GetHashCode, string.Equals);
        }

        internal override FSObject FindEntry(string aName)
        {
            return mEntries.GetValue(aName, null);
        }

        internal bool Mount(FSObject aObject)
        {
            if (mEntries.ContainsKey(aObject.Name))
                return false;
            mEntries.Add(aObject.Name, aObject);
            return true;
        }
    }
}