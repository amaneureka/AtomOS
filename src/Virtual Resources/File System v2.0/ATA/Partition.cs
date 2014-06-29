using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileSystem;

namespace FileSystem.ATA
{
    public class Partition : BlockDevice
    {
        protected Atapio aATA;
        public readonly uint BlockCount;

        public Partition(Atapio xATA, Stream xStream, uint aBlockCount)
            : base(xStream)
        {
            this.aATA = xATA;
            this.BlockCount = aBlockCount;
            Program.Devices.Add(this);
            this.BlockSize = 512;
        }

        public override bool Read(ulong BlockNo, uint xBlockCount, byte[] xData)
        {
            if (BlockCount < xBlockCount + BlockNo)
                return false; //Reading more than limit

            try
            {
                Stream xCurrent = IDevice;
                xCurrent.Seek((long)(BlockNo * BlockSize) + IDevice.Position, SeekOrigin.Begin);
                                
                BinaryReader br = new BinaryReader(xCurrent);
                br.Read(xData, 0, (int)(xBlockCount * BlockSize));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override bool Write(ulong BlockNo, uint xBlockCount, byte[] xData)
        {
            if (BlockCount < xBlockCount + BlockNo)
                return false; //Writing more than limit

            try
            {
                Stream xCurrent = IDevice;
                xCurrent.Seek((long)(BlockNo * BlockSize) + IDevice.Position, SeekOrigin.Begin);
                
                BinaryWriter bw = new BinaryWriter(xCurrent);
                bw.Write(xData, 0, (int)(xBlockCount * BlockSize));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "Partition";
        }
    }
}
