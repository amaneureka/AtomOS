using System;
using System.Collections.Generic;

using System.IO;
using System.Text;

namespace Atomix.RamFS
{
    public class FileEntry
    {
        public string Name { get; private set; }
        public Stream Data { get; private set; }

        public FileEntry(string aName, Stream aData)
        {
            this.Name = aName;
            this.Data = aData;

            if (Name.Length > 24)
                Name = Name.Substring(0, 24);
        }

        public byte[] GetEntryData(int StartAddress)
        {
            /*
             * 24 bytes := FileName
             * 4 bytes  := FileStartAddress
             * 4 bytes  := FileLength
             */
            var xResult = new byte[32];

            var xNameByteArray = Encoding.Unicode.GetBytes(Name);
            Array.Copy(xNameByteArray, 0, xResult, 0, xNameByteArray.Length);
            Array.Copy(BitConverter.GetBytes(StartAddress), 0, xResult, 24, 4);
            Array.Copy(BitConverter.GetBytes(Data.Length), 0, xResult, 28, 4);
            return xResult;
        }
    }
}
