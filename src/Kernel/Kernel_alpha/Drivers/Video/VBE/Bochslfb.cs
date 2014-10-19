using System;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Lib;

namespace Kernel_alpha.Drivers.Video.VBE
{
    public class Bochslfb
    {
        const ushort VBE_DISPI_IOPORT_INDEX         = 0x01CE;
        const ushort VBE_DISPI_IOPORT_DATA          = 0x01CF;
        const ushort VBE_DISPI_INDEX_ID             = 0x0;
        const ushort VBE_DISPI_INDEX_XRES           = 0x1;
        const ushort VBE_DISPI_INDEX_YRES           = 0x2;
        const ushort VBE_DISPI_INDEX_BPP            = 0x3;
        const ushort VBE_DISPI_INDEX_ENABLE         = 0x4;
        const ushort VBE_DISPI_INDEX_BANK           = 0x5;
        const ushort VBE_DISPI_INDEX_VIRT_WIDTH     = 0x6;
        const ushort VBE_DISPI_INDEX_VIRT_HEIGHT    = 0x7;
        const ushort VBE_DISPI_INDEX_X_OFFSET       = 0x8;
        const ushort VBE_DISPI_INDEX_Y_OFFSET       = 0x9;

        const ushort VBE_DISPI_DISABLED             = 0x00;
        const ushort VBE_DISPI_ENABLED              = 0x01;
        const ushort VBE_DISPI_GETCAPS              = 0x02;
        const ushort VBE_DISPI_8BIT_DAC             = 0x20;
        const ushort VBE_DISPI_LFB_ENABLED          = 0x40;
        const ushort VBE_DISPI_NOCLEARMEM           = 0x80;

        IOPort Index, Data;
        MemoryBlock08 Fb;
        uint xRes, yRes, Bpp;
        bool IsValid;

        public Bochslfb()
        {
            //have to detect if bochs
            Index = new IOPort(VBE_DISPI_IOPORT_INDEX);
            Data = new IOPort(VBE_DISPI_IOPORT_DATA);
            Fb = new MemoryBlock08(0xE0000000);//It is static :(
            IsValid = true;
        }

        public void SetMode(ushort x_res, ushort y_res, ushort bpp)
        {
            if (!IsValid)
                return;

            this.xRes = x_res;
            this.yRes = y_res;
            this.Bpp = (uint)(bpp / 8);
            vbe_write(VBE_DISPI_INDEX_ENABLE, VBE_DISPI_DISABLED);
            vbe_write(VBE_DISPI_INDEX_XRES, x_res);
            vbe_write(VBE_DISPI_INDEX_YRES, y_res);
            vbe_write(VBE_DISPI_INDEX_BPP, bpp);
            vbe_write(VBE_DISPI_INDEX_ENABLE, VBE_DISPI_ENABLED | VBE_DISPI_LFB_ENABLED);
        }

        public void SetPixel(uint x, uint y, uint color)
        {
            if (!IsValid)
                return;

            if (x >= xRes || y >= yRes)
                return;

            uint p = (x + (uint)(y * xRes)) * Bpp;
            Fb[p++] = (byte)(color & 0xFF);
            Fb[p++] = (byte)((color >> 8) & 0xFF);
            Fb[p++] = (byte)((color >> 16) & 0xFF);
        }

        public uint GetPixel(uint x, uint y)
        {
            if (!IsValid)
                return 0;

            if (x >= xRes || y >= yRes)
                return 0;

            uint p = (x + (uint)(y * xRes)) * Bpp;

            return (uint)(Fb[p + 2] >> 16 | Fb[p + 1] >> 8 | Fb[p]);
        }

        private void vbe_write(ushort index, ushort value)
        {
            Index.Outw(index);
            Data.Outw(value);
        }
    }
}
