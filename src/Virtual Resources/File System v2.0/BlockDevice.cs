using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FileSystem
{
    public abstract class BlockDevice
    {
        public readonly Stream IDevice;
        protected uint BlockSize;
        

        public BlockDevice(Stream IDevice)
        {
            if (!isValid(IDevice))
                return;

            this.IDevice = IDevice;            
        }

        private bool isValid(Stream xDevice)
        {
            return (xDevice.CanRead && xDevice.CanWrite);
        }

        public virtual bool Read(ulong BlockNo, uint BlockCount, byte[] xData)
        {
            return false;
        }

        public virtual bool Write(ulong BlockNo, uint BlockCount, byte[] xData)
        {
            return false;
        }
    }
}
