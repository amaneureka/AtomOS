/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Generic File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal abstract class GenericFileSystem : Directory
    {
        internal readonly Stream Device;

        internal GenericFileSystem(string aName, Stream aDevice)
            : base(aName)
        {
            Device = aDevice;
        }

        internal abstract bool Detect();
    }
}