/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Graphics Surface
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;

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
        internal static void CopyToBuffer(uint des, uint src, int des_x, int des_y, int des_width, int des_height, int src_x, int src_y, int src_width, int data_width, int data_height)
        {
            new Mov { DestinationReg = Register.ECX, SourceRef = "static_Field__System_UInt32_Atomix_Kernel_H_Drivers_Video_VBE_BytesPerPixel", SourceIndirect = true };
            //EBX = width
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 12 };

            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 36 };
            new Mul { DestinationReg = Register.EBP, DestinationIndirect = true, DestinationDisplacement = 32 };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 40 };
            new Mul { DestinationReg = Register.ECX };
            //des + (x1 + y1 * width) * BytesPerPixel
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 48 };
            new Mov { DestinationReg = Register.EDI, SourceReg = Register.EAX };

            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 20 };
            new Mul { DestinationReg = Register.EBP, DestinationIndirect = true, DestinationDisplacement = 16 };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 24 };
            new Mul { DestinationReg = Register.ECX };
            //src + (x1 + y1 * width) * BytesPerPixel
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 44 };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EAX };

            //factor1 = (des_width - width) * BytesPerPixel
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 32 };
            new Sub { DestinationReg = Register.EAX, SourceReg = Register.EBX };
            new Mul { DestinationReg = Register.ECX };
            new Push { DestinationReg = Register.EAX };

            //factor2 = (src_width - width) * BytesPerPixel
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 16 };
            new Sub { DestinationReg = Register.EAX, SourceReg = Register.EBX };
            new Mul { DestinationReg = Register.ECX };
            new Push { DestinationReg = Register.EAX };
            /*
             * STACK:
             * [ESP + 0x0] = factor2
             * [ESP + 0x4] = factor1
             */

            //Number of bytes to write
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 12 };
            new Add { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 40 };
            new Cmp { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 32 };
            new Jmp { Condition = ConditionalJump.JNG, DestinationRef = ".trunc_width" };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 32 };

            new Label (".trunc_width");
            new Sub { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 40 };
            new Mul { DestinationReg = Register.ECX };
            new Mov { DestinationReg = Register.EDX, SourceReg = Register.EAX };
            new Shr { DestinationReg = Register.EAX, SourceRef = "0x2" };//Divide by 4
            new And { DestinationReg = Register.EDX, SourceRef = "0x3" };//Modulo by 4

            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 8 };
            new Add { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 36 };
            new Cmp { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 28 };
            new Jmp { Condition = ConditionalJump.JNG, DestinationRef = ".trunc_height" };
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 28 };

            new Label (".trunc_height");
            new Sub { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceIndirect = true, SourceDisplacement = 36 };

            new Label (".draw_loop");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EAX };
            new Literal ("rep movsd");
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EDX };
            new Literal ("rep movsb");
            new Add { DestinationReg = Register.EDI, SourceReg = Register.ESP, SourceDisplacement = 0x4, SourceIndirect = true };
            new Add { DestinationReg = Register.ESI, SourceReg = Register.ESP, SourceIndirect = true };
            new Sub { DestinationReg = Register.EBX, SourceRef = "0x1" };
            new Jmp { Condition = ConditionalJump.JNZ, DestinationRef = ".draw_loop" };
            new Add { DestinationReg = Register.ESP, SourceRef = "0x8" };//2 items on stack
        }
        
        [Assembly(true)]
        internal static void Clear(byte* aAddress, uint aColor, int aSize)
        {
            new Mov { DestinationReg = Register.EBX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
            new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
            new Mov { DestinationReg = Register.ESI, SourceReg = Register.EBP, SourceDisplacement = 0x10, SourceIndirect = true };

            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new Shr { DestinationReg = Register.ECX, SourceRef = "0x2" };
            new Literal ("rep stosd");
            new And { DestinationReg = Register.EBX, SourceRef = "0x3" };
            new Mov { DestinationReg = Register.ECX, SourceReg = Register.EBX };
            new Literal ("rep stosb");
        }
    }
}
