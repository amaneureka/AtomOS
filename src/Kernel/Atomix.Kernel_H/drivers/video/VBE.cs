/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          VBE 2.0 Driver
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.Drivers.Video
{
    internal static unsafe class VBE
    {
        internal static int Xres;
        internal static int Yres;
        internal static int BytesPerPixel;
        internal static byte* SecondaryBuffer;
        internal static byte* VirtualFrameBuffer;
        private static VBE_Mode_Info* ModeInfoBlock;

        internal static void Init()
        {
            Debug.Write("VBE Init()\n");
            ModeInfoBlock = (VBE_Mode_Info*)(Multiboot.VBE_Mode_Info + 0xC0000000);
            Xres = ModeInfoBlock->Xres;
            Yres = ModeInfoBlock->Yres;
            BytesPerPixel = (int)(ModeInfoBlock->bpp / 8);
            VirtualFrameBuffer = (byte*)Paging.AllocateMainBuffer(ModeInfoBlock->physbase);
            SecondaryBuffer = (byte*)Paging.AllocateSecondayBuffer();

            /* Print Debug Info */
            Debug.Write("Virtual Frame Buffer: %d\n", (uint)VirtualFrameBuffer);
            Debug.Write("Secondary Frame Buffer: %d\n", (uint)SecondaryBuffer);
            Debug.Write("Resolution: %dx", (uint)Xres);
            Debug.Write("%dx", (uint)Yres);
            Debug.Write("%d\n", (uint)BytesPerPixel);
        }

        internal static void SetPixel(int x, int y, uint c)
        {
            byte *buffer = (byte*)(((x + (y * Xres)) * BytesPerPixel) + SecondaryBuffer);
            buffer[0] = (byte)(c >> 0);
            buffer[1] = (byte)(c >> 8);
            buffer[2] = (byte)(c >> 16);
        }

        internal static uint GetPixel(int x, int y)
        {
            byte* buffer = (byte*)(((x + (y * Xres)) * BytesPerPixel) + SecondaryBuffer);
            return (uint)(buffer[2] << 16 | buffer[1] << 8 | buffer[0]);
        }

        [Assembly(true)]
        internal static void Clear(uint color)
        {
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceRef = "static_Field__System_Byte__Atomix_Kernel_H_Drivers_Video_VBE_SecondaryBuffer", SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x100000" });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Cli ());
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep stosd"));
            AssemblyHelper.AssemblerCode.Add(new Sti ());
        }

        [Assembly(true)]
        internal static void Update()
        {
#warning [VBE] : fixed size memory copy
            //Copy 4MB of data from Secondary Buffer to Virtual Frame Buffer
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceRef = "static_Field__System_Byte__Atomix_Kernel_H_Drivers_Video_VBE_SecondaryBuffer", SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceRef = "static_Field__System_Byte__Atomix_Kernel_H_Drivers_Video_VBE_VirtualFrameBuffer", SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x100000" });
            AssemblyHelper.AssemblerCode.Add(new Cli ());
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep movsd"));
            AssemblyHelper.AssemblerCode.Add(new Sti ());
        }

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
            public Int16 Xres;
            [FieldOffset(20)]
            public Int16 Yres;
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
    }
}
