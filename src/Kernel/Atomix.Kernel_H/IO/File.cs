/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          VFS File
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO
{
    internal abstract class File : FSObject
    {
        internal uint SizeInBytes;

        internal File(string aName)
            : base(aName)
        {

        }

        internal abstract Stream Open(FileMode aMode);
    }
}