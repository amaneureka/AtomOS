using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FileSystem;

namespace FileSystem.ATA
{
    public class Atapio : BlockDevice
    {
        //Calculated value so, not necessary will be true for all vmdk
        private const int vmware_const_zeroth_sector = 0x100;
        public Atapio(Stream IDevice)
            :base(IDevice)
        {
            //Constant for all IDE/SATA hard disk
            BlockSize = 512;
            
            IDevice.Seek((long)(0x20000), SeekOrigin.Begin);
            Program.Devices.Add(this);

            var xMBR = new MBR(this);
        }
                
        public override bool Read(ulong BlockNo, uint BlockCount, byte[] xData)
        {
            try
            {
                Stream xCurrent = IDevice;
                xCurrent.Seek(0x20000 + (long)(BlockNo * BlockCount), SeekOrigin.Begin);

                BinaryReader br = new BinaryReader(xCurrent);
                br.Read(xData, 0, (int)(BlockCount * BlockSize));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override bool Write(ulong BlockNo, uint BlockCount, byte[] xData)
        {
            try
            {
                Stream xCurrent = IDevice;
                xCurrent.Seek(0x20000 + (long)(BlockNo * BlockCount), SeekOrigin.Begin);

                BinaryWriter bw = new BinaryWriter(xCurrent);
                bw.Write(xData, 0, (int)(BlockCount * BlockSize));
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "ATAPIO";
        }
    }
}
