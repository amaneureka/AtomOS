using System;
using Kernel_alpha.x86;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Drivers.Video.Vmware;
using Kernel_alpha.Lib;

namespace Kernel_alpha.Drivers.Video
{
    public unsafe class VMWareSVGAII
    {
        private PCIDevice Device;

        private IOPort IndexPort;
        private IOPort ValuePort;

        private MemoryBlock32 FB_Memory;
        private MemoryBlock32 FIFO_Memory;

        private UInt32 VersionID;
        private UInt32 Capabilities;
        private UInt32 Width;
        private UInt32 Height;
        private UInt32 BPP;
        private UInt32 Pitch;

        public VMWareSVGAII()
        {
            Device = PCI.GetDeviceVendorID(misc.PCI_VENDOR_ID_VMWARE, misc.PCI_DEVICE_ID_VMWARE_SVGA2);

            if (Device == null)
                throw new Exception("Device Not Found");

            Device.EnableMemory(true);

            //IO Ports
            var IOBase = Device.BaseAddressBar[0].BaseAddress;
            IndexPort = new IOPort((ushort)(IOBase + (ushort)IOPortOffset.Index));
            ValuePort = new IOPort((ushort)(IOBase + (ushort)IOPortOffset.Value));
            
            //Memory Block
            FB_Memory = new MemoryBlock32(Device.BaseAddressBar[1].BaseAddress);
            FIFO_Memory = new MemoryBlock32(Device.BaseAddressBar[2].BaseAddress);
            
           //Version Check
            VersionID = (UInt32)Versions.SVGA_ID_2;
            do
            {
                WriteRegister(Registers.SVGA_REG_ID, VersionID);
                if (ReadRegister(Registers.SVGA_REG_ID) == VersionID)
                    break;
                else
                    VersionID--;
            }
            while (VersionID >= (UInt32)Versions.SVGA_ID_0);
            
            if (VersionID < (UInt32)Versions.SVGA_ID_0)
                throw new Exception("Error negotiating SVGA device version.");
            
            //Memory Block Length
            FB_Memory.Length = ReadRegister(Registers.SVGA_REG_FB_SIZE);
            FIFO_Memory.Length = ReadRegister(Registers.SVGA_REG_MEM_SIZE);
            
            //Memory Block Length Check 
            if (FB_Memory.Length < 0x100000)
                throw new Exception("FB size very small, probably incorrect.");

            if (FIFO_Memory.Length < 0x20000)
                throw new Exception("FIFO size very small, probably incorrect.");

            if (VersionID >= (UInt32)Versions.SVGA_ID_1)
                Capabilities = ReadRegister(Registers.SVGA_REG_CAPABILITIES);
        }

        public void SetMode(uint aWidth, uint aHeight, uint aBPP)
        {
            Width = aWidth;
            Height = aHeight;
            BPP = aBPP;

            WriteRegister(Registers.SVGA_REG_WIDTH, Width);
            WriteRegister(Registers.SVGA_REG_HEIGHT, Height);
            WriteRegister(Registers.SVGA_REG_BITS_PER_PIXEL, BPP);
            WriteRegister(Registers.SVGA_REG_ENABLE, 1);//True

            Pitch = ReadRegister(Registers.SVGA_REG_BYTES_PER_LINE);

            InitializeFIFO();
        }

        private unsafe void InitializeFIFO()
        {
            //FIFO Registers
            FIFO_Memory[(uint)FIFO.SVGA_FIFO_MIN] = (uint)Registers.SVGA_FIFO_NUM_REGS * sizeof(uint);
            FIFO_Memory[(uint)FIFO.SVGA_FIFO_MAX] = FIFO_Memory.Length;
            FIFO_Memory[(uint)FIFO.SVGA_FIFO_NEXT_CMD] = FIFO_Memory[(int)FIFO.SVGA_FIFO_MIN];
            FIFO_Memory[(uint)FIFO.SVGA_FIFO_STOP] = FIFO_Memory[(int)FIFO.SVGA_FIFO_MIN];
            /*if (HasFIFOCap((int)FIFO.SVGA_FIFO_CAPABILITIES) && IsFIFORegValid((int)FIFO.SVGA_FIFO_GUEST_3D_HWVERSION))
            {
                FIFO_Memory[(int)FIFO.SVGA_FIFO_GUEST_3D_HWVERSION] = SVGA3D_HWVERSION_CURRENT;
            }*/

            //Enable FIFO
            WriteRegister(Registers.SVGA_REG_CONFIG_DONE, 1);//True
        }

        public void Update(uint x, uint y, uint width, uint height)
        {
            WriteToFifo((uint)FIFO.Update);
            WriteToFifo(x);
            WriteToFifo(y);
            WriteToFifo(width);
            WriteToFifo(height);
            WaitForFifo();
        }

        public void Clear(uint color)
        {
            for (ushort y = 0; y < Height; y++)
            {
                for (ushort x = 0; x < Width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
            Update(0, 0, Width, Height);
        }

        private void WriteToFifo(uint value)
        {
            if (((GetFIFO(FIFO.SVGA_FIFO_NEXT_CMD) == GetFIFO(FIFO.SVGA_FIFO_MAX) - 4) && GetFIFO(FIFO.SVGA_FIFO_STOP) == GetFIFO(FIFO.SVGA_FIFO_MIN)) ||
                (GetFIFO(FIFO.SVGA_FIFO_NEXT_CMD) + 4 == GetFIFO(FIFO.SVGA_FIFO_STOP)))
                WaitForFifo();

            SetFIFO((FIFO)GetFIFO(FIFO.SVGA_FIFO_NEXT_CMD), value);
            SetFIFO(FIFO.SVGA_FIFO_NEXT_CMD, GetFIFO(FIFO.SVGA_FIFO_NEXT_CMD) + 4);

            if (GetFIFO(FIFO.SVGA_FIFO_NEXT_CMD) == GetFIFO(FIFO.SVGA_FIFO_MAX))
                SetFIFO(FIFO.SVGA_FIFO_NEXT_CMD, GetFIFO(FIFO.SVGA_FIFO_MIN));
        }

        public void SetPixel(ushort x, ushort y, uint color)
        {
            FB_Memory[(UInt32)(((uint)(y * Width) + x) * 4)] = color;
        }

        private uint GetFIFO(FIFO cmd)
        {
            if (Device != null)
                return FIFO_Memory[(uint)cmd];
            return 0;
        }

        private uint SetFIFO(FIFO cmd, uint value)
        {
            if (Device != null)
                return FIFO_Memory[(uint)cmd] = value;
            return 0;
        }

        private void WaitForFifo()
        {
            WriteRegister(Registers.SVGA_REG_SYNC, 1);
            while (ReadRegister(Registers.SVGA_REG_BUSY) != 0) { }
        }

        private bool HasFIFOCap(int cap)
        {
            return (FIFO_Memory[(int)FIFO.SVGA_FIFO_CAPABILITIES] & cap) != 0;
        }

        private bool IsFIFORegValid(int reg)
        {
            return FIFO_Memory[(int)FIFO.SVGA_FIFO_MIN] > (reg << 2);
        }

        #region IO Ports
        private UInt32 ReadRegister(UInt32 Index)
        {
            IndexPort.DWord = Index;
            return ValuePort.DWord;
        }

        private UInt32 ReadRegister(Registers Index)
        {
            IndexPort.DWord = (UInt32)Index;
            return ValuePort.DWord;
        }

        private void WriteRegister(UInt32 Index, UInt32 Value)
        {
            IndexPort.DWord = Index;
            ValuePort.DWord = Value;
        }
        
        private void WriteRegister(Registers Index, UInt32 Value)
        {
            IndexPort.DWord = (UInt32)Index;
            ValuePort.DWord = Value;
        }
        #endregion
    }
}
