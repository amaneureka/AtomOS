using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Atomix.Kernel_H;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

namespace Atomix.Kernel_H.drivers.video
{
    public static unsafe class VBE
    {
        private static VBE_Mode_Info* ModeInfoBlock;
        private static uint PhysicalFrameBuffer;
        private static uint VirtualFrameBuffer;

        public static void Init()
        {
            Debug.Write("VBE Init()\n");
            ModeInfoBlock = (VBE_Mode_Info*)(Multiboot.VBE_Mode_Info + 0xC0000000);
            PhysicalFrameBuffer = ModeInfoBlock->physbase;
            Debug.Write("Physical Frame Buffer: %d\n", PhysicalFrameBuffer);
            VirtualFrameBuffer = Paging.AllocateVBE(PhysicalFrameBuffer);
            Debug.Write("Virtual Frame Buffer: %d\n", VirtualFrameBuffer);
            Clear(0xFFFFFFFF);
            Debug.Write("Done()");
        }

        public static void Clear(uint c)
        {
            var LinearFrameBuffer = (UInt32*)VirtualFrameBuffer;
            uint p = 0;
            for (int x = 0; x < ModeInfoBlock->Xres; x++)
            {
                for (int y = 0; y < ModeInfoBlock->Yres; y++)
                {
                    LinearFrameBuffer[p++] = c;
                }
            }
        }

        #region Struct
        [StructLayout(LayoutKind.Explicit, Size = 50)]
        public unsafe struct VBE_Mode_Info
        {
            [FieldOffset(0)]
            public UInt16 attributes;
            [FieldOffset(2)]
            public byte winA;
            [FieldOffset(3)]
            public byte winB;
            [FieldOffset(4)]
            public UInt16 granularity;
            [FieldOffset(6)]
            public UInt16 winsize;
            [FieldOffset(8)]
            public UInt16 segmentA;
            [FieldOffset(10)]
            public UInt16 segmentB;
            [FieldOffset(12)]
            public UInt32 realFctPtr;
            [FieldOffset(16)]
            public UInt16 pitch;
            [FieldOffset(18)]
            public UInt16 Xres;
            [FieldOffset(20)]
            public UInt16 Yres;
            [FieldOffset(22)]
            public byte Wchar;
            [FieldOffset(23)]
            public byte Ychar;
            [FieldOffset(24)]
            public byte planes;
            [FieldOffset(25)]
            public byte bpp;
            [FieldOffset(26)]
            public byte banks;
            [FieldOffset(27)]
            public byte memory_model;
            [FieldOffset(28)]
            public byte bank_size;
            [FieldOffset(29)]
            public byte image_pages;
            [FieldOffset(30)]
            public byte reserved0;
            [FieldOffset(31)]
            public byte red_mask;
            [FieldOffset(32)]
            public byte red_position;
            [FieldOffset(33)]
            public byte green_mask;
            [FieldOffset(34)]
            public byte green_position;
            [FieldOffset(35)]
            public byte blue_mask;
            [FieldOffset(36)]
            public byte blue_position;
            [FieldOffset(37)]
            public byte rsv_mask;
            [FieldOffset(38)]
            public byte rsv_position;
            [FieldOffset(39)]
            public byte directcolor_attributes;
            [FieldOffset(40)]
            public UInt32 physbase;
            [FieldOffset(44)]
            public UInt32 reserved1;
            [FieldOffset(48)]
            public UInt16 reserved2;
        }
        #endregion
    }
}
