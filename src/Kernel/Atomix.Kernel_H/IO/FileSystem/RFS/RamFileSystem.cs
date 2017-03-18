/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ram File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.IO.FileSystem.RFS;

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal unsafe class RamFileSystem : GenericFileSystem
    {
        IDictionary<string, RamFile> mFiles;

        internal RamFileSystem(string aName, Stream aDevice)
            :base(aName, aDevice)
        {
            mFiles = new IDictionary<string, RamFile>(Internals.GetHashCode, string.Equals);
        }

        internal override bool Detect()
        {
            if (!(Device is MemoryStream))
                return false;

            var stream = (MemoryStream)Device;
            var data = new byte[sizeof(FileEntry)];
            var entry = (FileEntry*)data.GetDataOffset();

            stream.Read(data, sizeof(FileEntry));
            while (entry->StartAddress != 0)
            {
                var name = new string(entry->Name);
                mFiles.Add(name, new RamFile(name, stream.Address + entry->StartAddress, entry->Length));
                Device.Read(data, sizeof(FileEntry));
            }
            return true;
        }

        internal override FSObject FindEntry(string aName)
        {
            return mFiles.GetValue(aName, null);
        }
    }
}
