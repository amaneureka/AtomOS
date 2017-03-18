/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          RFS (Ram File System) Helper
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem.RFS
{
    internal class RamFile : File
    {
        int mLength;
        uint mStartAddress;

        internal RamFile(string aName, uint aStartAddress, int aLength)
            :base(aName)
        {
            mLength = aLength;
            mStartAddress = aStartAddress;
        }

        internal override Stream Open(FileMode aMode)
        {
            if ((aMode & FileMode.Append) != 0 || (aMode & FileMode.Write) != 0)
                return null;
            return new MemoryStream(mStartAddress, mLength);
        }
    }
}
