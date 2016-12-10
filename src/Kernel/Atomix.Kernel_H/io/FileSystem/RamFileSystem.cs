/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Ram File System
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Lib.Crypto;
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
            Files = new IDictionary<string, RamFile>(sdbm.GetHashCode, string.Equals);
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

        public override Stream GetFile(string[] path, int pointer)
        {
            var FileName = path[pointer];
            if (Files.ContainsKey(FileName))
                return new FileStream(Files[FileName]);
            return null;
        }

        public override bool CreateFile(string[] path, int pointer)
        {
            return false;
        }

        [StructLayout(LayoutKind.Explicit, Size = 32)]
        struct FileEntry
        {
            [FieldOffset(0)]
            public fixed char Name[12];

            [FieldOffset(24)]
            public uint StartAddress;

            [FieldOffset(28)]
            public int Length;
        }
    }
}
