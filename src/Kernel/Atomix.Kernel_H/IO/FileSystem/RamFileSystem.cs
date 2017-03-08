/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Ram File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.IO.FileSystem.RFS;

using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.IO.FileSystem
{
    internal unsafe class RamFileSystem : GenericFileSystem
    {
        uint DataAddress;
        uint DataLength;
        IDictionary<string, RamFile> Files;

        internal RamFileSystem(uint aAddress, uint aLength)
        {
            DataAddress = aAddress;
            DataLength = aLength;
            Files = new IDictionary<string, RamFile>(Internals.GetHashCode, string.Equals);
            mIsValid = LoadFileSystem();
        }

        private bool LoadFileSystem()
        {
            var Entries = (FileEntry*)DataAddress;
            Entries++;
            while(Entries->StartAddress != 0)
            {
                var FileName = new string(Entries->Name);
                Files.Add(FileName, new RamFile(FileName, Entries->StartAddress + DataAddress, Entries->Length));
                Entries++;
            }
            return true;
        }

        internal override Stream GetFile(string[] path, int pointer)
        {
            var FileName = path[pointer];
            if (Files.ContainsKey(FileName))
                return new FileStream(Files[FileName]);
            return null;
        }

        internal override bool CreateFile(string[] path, int pointer)
        {
            return false;
        }

        [StructLayout(LayoutKind.Explicit, Size = 40)]
        struct FileEntry
        {
            [FieldOffset(0)]
            public fixed sbyte Name[32];

            [FieldOffset(32)]
            public uint StartAddress;

            [FieldOffset(36)]
            public int Length;
        }
    }
}
