using System;
using System.Collections.Generic;
using Kernel_alpha.x86.Intrinsic;
using Kernel_alpha.Drivers.Video.VGA;
using Kernel_alpha.Lib;

namespace Kernel_alpha.Drivers.Video
{
    public unsafe class VGAScreen
    {
        #region IO Ports
        private readonly IOPort AttributeController_Index;        
        private readonly IOPort AttributeController_Read;
        private readonly IOPort AttributeController_Write;

        private readonly IOPort MiscellaneousOutput_Write;

        private readonly IOPort Sequencer_Index;
        private readonly IOPort Sequencer_Data;

        private readonly IOPort DACIndex_Read;
        private readonly IOPort DACIndex_Write;
        private readonly IOPort DAC_Data;

        private readonly IOPort GraphicsController_Index;
        private readonly IOPort GraphicsController_Data;
        private readonly IOPort CRTController_Index;
        private readonly IOPort CRTController_Data;
        private readonly IOPort Instat_Read;
        #endregion

        private const byte NumSeqRegs = 5;
        private const byte NumCRTCRegs = 25;
        private const byte NumGCRegs = 9;
        private const byte NumACRegs = 21;

        private MemoryBlock08 VideoMemory;

        private ushort width;
        private ushort height;

        public ushort Width
        { get { return width; } }

        public ushort Height
        { get { return height; } }

        public VGAScreen()
        {
            AttributeController_Index = new IOPort(0x3C0);
            AttributeController_Read = new IOPort(0x3C1);
            AttributeController_Write = new IOPort(0x3C0);
            MiscellaneousOutput_Write = new IOPort(0x3C2);
            Sequencer_Index = new IOPort(0x3C4);
            Sequencer_Data = new IOPort(0x3C5);
            DACIndex_Read = new IOPort(0x3C7);
            DACIndex_Write = new IOPort(0x3C8);
            DAC_Data = new IOPort(0x3C9);
            GraphicsController_Index = new IOPort(0x3CE);
            GraphicsController_Data = new IOPort(0x3CF);
            CRTController_Index = new IOPort(0x3D4);
            CRTController_Data = new IOPort(0x3D5);
            Instat_Read = new IOPort(0x3DA);

            VideoMemory = new MemoryBlock08(0xA0000);
        }

        public void SetMode0()
        {
            WriteRegister(modes.g_320x200x256);
            width = 320;
            height = 200;

            byte[] xHex = new byte[] { 0x00, 0x33, 0x66, 0x99, 0xCC, 0xFF };
            int c = 0;
            for (byte i = 0; i < 6; i++)
                for (byte j = 0; j < 6; j++)
                    for (byte k = 0; k < 6; k++)
                    {
                        SetPaletteEntry(c, xHex[i], xHex[j], xHex[k]);
                        c++;
                    }
        }
        
        private void WriteRegister(byte[] registers)
        {
            int xIdx = 0;
            byte i;

            /* write MISCELLANEOUS reg */
            MiscellaneousOutput_Write.Byte = registers[xIdx];
            xIdx++;

            /* write SEQUENCER regs */
            for (i = 0; i < NumSeqRegs; i++)
            {
                Sequencer_Index.Byte = i;
                Sequencer_Data.Byte = registers[xIdx];
                xIdx++;
            }

            /* unlock CRTC registers */
            CRTController_Index.Byte = 0x03;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte | 0x80);
            CRTController_Index.Byte = 0x11;
            CRTController_Data.Byte = (byte)(CRTController_Data.Byte & 0x7F);

            /* make sure they remain unlocked */
            registers[0x03] |= 0x80;
            registers[0x11] &= 0x7f;

            /* write CRTC regs */
            for (i = 0; i < NumCRTCRegs; i++)
            {
                CRTController_Index.Byte = i;
                CRTController_Data.Byte = registers[xIdx];
                xIdx++;
            }

            /* write GRAPHICS CONTROLLER regs */
            for (i = 0; i < NumGCRegs; i++)
            {
                GraphicsController_Index.Byte = i;
                GraphicsController_Data.Byte = registers[xIdx];
                xIdx++;
            }

            /* write ATTRIBUTE CONTROLLER regs */
            for (i = 0; i < NumACRegs; i++)
            {
                var xDoSomething = Instat_Read.Byte;
                AttributeController_Index.Byte = i;
                AttributeController_Write.Byte = registers[xIdx];
                xIdx++;
            }

            /* lock 16-color palette and unblank display */
            var xNothing = Instat_Read.Byte;
            AttributeController_Index.Byte = 0x20;
        }

        public void SetPaletteEntry(int index, byte r, byte g, byte b)
        {
            DACIndex_Write.Byte = (byte)index;
            DAC_Data.Byte = (byte)(r >> 2);
            DAC_Data.Byte = (byte)(g >> 2);
            DAC_Data.Byte = (byte)(b >> 2);
        }

        public void SetPixel_8(uint x, uint y, byte color)
        {
            var xOffset = (uint)((y * Width) + x);
            VideoMemory[xOffset] = color;
        }
    }
}
