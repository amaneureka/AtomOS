using System;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Drawing;

namespace Atomix.RamFS
{
    public class FileEntry
    {
        public string FileName { get; private set; }
        public Stream FileData { get; private set; }
        public byte[] RawData { get; private set; }
        
        public FileEntry(string aName, Stream aData)
        {
            this.FileName = aName;
            this.FileData = aData;

            if (FileName.Length > 12)
                FileName = FileName.Substring(0, 12);

            PrepareRawData();
        }

        private void PrepareRawData()
        {
            switch (Path.GetExtension(FileName).ToLower())
            {
                case ".png":
                    using (var bitmap = new Bitmap(FileData))
                    {
                        RawData = new byte[(bitmap.Width * bitmap.Height * 4) + 8];
                        Array.Copy(BitConverter.GetBytes(bitmap.Width), 0, RawData, 0, 4);
                        Array.Copy(BitConverter.GetBytes(bitmap.Height), 0, RawData, 4, 4);

                        int pointer = 8;
                        for (int y = 0; y < bitmap.Height; y++)
                            for (int x = 0; x < bitmap.Width; x++, pointer += 4)
                                Array.Copy(BitConverter.GetBytes(bitmap.GetPixel(x, y).ToArgb()), 0, RawData, pointer, 4);
                    }
                    FileName = FileName.Replace(".png", ".xmp");
                    break;
                default:
                    {
                        RawData = new byte[FileData.Length];
                        FileData.Read(RawData, 0, RawData.Length);
                    }
                    break;
            }
        }

        public void GetEntryData(int StartAddress, byte[] xResult)
        {
            /*
             * 24 bytes := FileName
             * 4 bytes  := FileStartAddress
             * 4 bytes  := FileLength
             */
            var xNameByteArray = Encoding.Unicode.GetBytes(FileName);
            Array.Copy(xNameByteArray, 0, xResult, 0, xNameByteArray.Length);
            Array.Copy(BitConverter.GetBytes(StartAddress), 0, xResult, 24, 4);
            Array.Copy(BitConverter.GetBytes(RawData.Length), 0, xResult, 28, 4);
        }

        public int Dump(Stream xOutput)
        {
            xOutput.Write(RawData, 0, RawData.Length);
            return RawData.Length;
        }
    }
}
