/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Graphics Surface
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.CompilerExt.Attributes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.Lib.Graphic
{
    internal unsafe class Surface
    {
        byte* mBuffer;
        int mWidth;
        int mHeight;

        Rectangle* Rectangle_List;
        int Rectangle_List_index;

        internal Surface(byte* backbuffer, int width, int height)
        {
            mBuffer = backbuffer;
            mWidth = width;
            mHeight = height;
        }

        internal void Fill(byte* bitamp, int x, int y, int w, int h)
        {
            return;

            int a, b, c, d, p, q, r, s, l, m, n, o;

            a = x;
            b = y;
            c = a + w - 1;
            d = b + h - 1;

            int count = Rectangle_List_index;
            for (int index = 0; index < count; index++)
            {
                var rect_a = Rectangle_List[index];

                p = rect_a.x;
                q = rect_a.y;
                r = p + rect_a.width - 1;
                s = q + rect_a.height - 1;

                // They won't overlap
                if (r < x || s < y || c < p || d < q)
                    continue;

                // Overlaping rectangles
                l = Math.Max(p, a);
                m = Math.Max(q, b);
                n = Math.Min(r, c);
                o = Math.Min(s, d);

                // Draw on buffer @{(l, m), (n, o)} from bitamp {(l-x, m-y), (n-x, o-y)}
                CopyToBuffer(mBuffer, bitamp, l, m, mWidth, mHeight, l - x, m - y, w, n - l + 1, o - m + 1);
            }
        }

        internal void Clear(uint aColor)
        {
            Surface.Clear(mBuffer, aColor, (mWidth * mHeight * 4));
        }

        internal void Rectangle(int x, int y, int w, int h)
        {
            return;

            if (w <= 0 || h <= 0)
                return;

            int a, b, c, d, p, q, r, s, l, m, n, o;

            a = x;
            b = y;
            c = a + w - 1;
            d = b + h - 1;

            int count = Rectangle_List_index;
            for (int index = 0; index < count; index++)
            {
                var rect_a = Rectangle_List[index];

                p = rect_a.x;
                q = rect_a.y;
                r = p + rect_a.width - 1;
                s = q + rect_a.height - 1;

                // They won't overlap
                if (r < x || s < y || c < p || d < q)
                    continue;

                // Overlaping rectangles
                l = Math.Min(p, a);
                m = Math.Min(q, b);
                n = Math.Max(r, c);
                o = Math.Max(s, d);

                rect_a.x = l;
                rect_a.y = m;
                rect_a.width = n - l + 1;
                rect_a.height = o - m + 1;
                return;
            }
            List_Add(x, y, w, h);
        }

        private void List_Add(int x, int y, int w, int h)
        {
            return;

            int index = Rectangle_List_index;
            Rectangle_List[index].x = x;
            Rectangle_List[index].y = y;
            Rectangle_List[index].width = w;
            Rectangle_List[index].height = h;
            // Rectangle_List_index = index + 1;
        }

        /*
         * "des"                [EBP + 48]
         * "src"                [EBP + 44]
         * "des_x"              [EBP + 40]
         * "des_y"              [EBP + 36]
         * "des_width"          [EBP + 32]
         * "des_height"         [EBP + 28]
         * "src_x"              [EBP + 24]
         * "src_y"              [EBP + 20]
         * "src_width"          [EBP + 16]
         * "data_width"         [EBP + 12]
         * "data_height"        [EBP + 08]
         */
        [Assembly(true)]
        internal static void CopyToBuffer(byte* des, byte* src, int des_x, int des_y, int des_width, int des_height, int src_x, int src_y, int src_width, int data_width, int data_height)
        {
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "static_Field__System_Int32_Atomix_Kernel_H_Drivers_Video_VBE_BytesPerPixel", SourceIndirect = true });
            //EBX = width
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });

            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 32 });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //des + (x1 + y1 * width) * BytesPerPixel
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 48 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EAX });

            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 20 });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 16 });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 24 });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //src + (x1 + y1 * width) * BytesPerPixel
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 44 });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EAX });

            //factor1 = (des_width - width) * BytesPerPixel
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });
            AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

            //factor2 = (src_width - width) * BytesPerPixel
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 16 });
            AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            AssemblyHelper.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
            /*
             * STACK:
             * [ESP + 0x0] = factor2
             * [ESP + 0x4] = factor1
             */

            //Number of bytes to write
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            AssemblyHelper.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });
            AssemblyHelper.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNG, DestinationRef = Label.PrimaryLabel + ".trunc_width" });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });

            AssemblyHelper.AssemblerCode.Add(new Label (".trunc_width"));
            AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            AssemblyHelper.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EAX });
            AssemblyHelper.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EAX, SourceRef = "0x2" });//Divide by 4
            AssemblyHelper.AssemblerCode.Add(new And { DestinationReg = Registers.EDX, SourceRef = "0x3" });//Modulo by 4

            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 8 });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });
            AssemblyHelper.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 28 });
            AssemblyHelper.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNG, DestinationRef = Label.PrimaryLabel + ".trunc_height" });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 28 });

            AssemblyHelper.AssemblerCode.Add(new Label (".trunc_height"));
            AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });

            AssemblyHelper.AssemblerCode.Add(new Label (".draw_loop"));
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep movsd"));
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EDX });
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep movsb"));
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceReg = Registers.ESP, SourceDisplacement = 0x4, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.ESI, SourceReg = Registers.ESP, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Sub { DestinationReg = Registers.EBX, SourceRef = "0x1" });
            AssemblyHelper.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNZ, DestinationRef = Label.PrimaryLabel + ".draw_loop" });
            AssemblyHelper.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });//2 items on stack
        }
        
        [Assembly(true)]
        internal static void Clear(byte* aAddress, uint aColor, int aSize)
        {
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceDisplacement = 0x10, SourceIndirect = true });

            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            AssemblyHelper.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep stosd"));
            AssemblyHelper.AssemblerCode.Add(new And { DestinationReg = Registers.EBX, SourceRef = "0x3" });
            AssemblyHelper.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            AssemblyHelper.AssemblerCode.Add(new Literal ("rep stosd"));
        }
    }
}
