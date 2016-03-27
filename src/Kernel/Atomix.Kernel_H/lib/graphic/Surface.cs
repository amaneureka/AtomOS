using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.CompilerExt.Attributes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.lib.graphic
{
    public unsafe class Surface
    {
        private byte* mBuffer;
        private int mWidth;
        private int mHeight;

        private Rectangle* Rectangle_List;
        private int Rectangle_List_index;
        
        public Surface(byte* backbuffer, int width, int height)
        {
            this.mBuffer = backbuffer;
            this.mWidth = width;
            this.mHeight = height;
        }
        
        public void Fill(byte* bitamp, int x, int y, int w, int h)
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

                //They won't overlap
                if (r < x || s < y || c < p || d < q)
                    continue;

                //Overlaping rectangles
                l = Math.Max(p, a);
                m = Math.Max(q, b);
                n = Math.Min(r, c);
                o = Math.Min(s, d);

                //Draw on buffer @{(l, m), (n, o)} from bitamp {(l-x, m-y), (n-x, o-y)}
                CopyToBuffer(mBuffer, bitamp, l, m, mWidth, mHeight, l - x, m - y, w, n - l + 1, o - m + 1);
            }
        }

        public void Clear(uint aColor)
        {
            Surface.Clear(mBuffer, aColor, (mWidth * mHeight * 4));
        }
        
        public void Rectangle(int x, int y, int w, int h)
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

                //They won't overlap
                if (r < x || s < y || c < p || d < q)
                    continue;

                //Overlaping rectangles
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
            //Rectangle_List_index = index + 1;
        }

        #region Static Functions
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
        [Assembly(0x2C)]
        public static void CopyToBuffer(byte* des, byte* src, int des_x, int des_y, int des_width, int des_height, int src_x, int src_y, int src_width, int data_width, int data_height)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "static_Field__System_Int32_Atomix_Kernel_H_drivers_video_VBE_BytesPerPixel", SourceIndirect = true });
            //EBX = width
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 32 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });            
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //des + (x1 + y1 * width) * BytesPerPixel
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 48 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EAX });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 20 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 16 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 24 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //src + (x1 + y1 * width) * BytesPerPixel
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 44 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EAX });

            //factor1 = (des_width - width) * BytesPerPixel
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

            //factor2 = (src_width - width) * BytesPerPixel
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 16 });
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
            /*
             * STACK:
             * [ESP + 0x0] = factor2
             * [ESP + 0x4] = factor1
             */
                        
            //Number of bytes to write
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNG, DestinationRef = Label.PrimaryLabel + ".trunc_width" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });

            Core.AssemblerCode.Add(new Label(".trunc_width"));
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.EAX, SourceRef = "0x2" });//Divide by 4
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.EDX, SourceRef = "0x3" });//Modulo by 4

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 8 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });
            Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 28 });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNG, DestinationRef = Label.PrimaryLabel + ".trunc_height" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 28 });

            Core.AssemblerCode.Add(new Label(".trunc_height"));
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });
            
            Core.AssemblerCode.Add(new Label(".draw_loop"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("rep movsd"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EDX });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDI, SourceReg = Registers.ESP, SourceDisplacement = 0x4, SourceIndirect = true });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESI, SourceReg = Registers.ESP, SourceIndirect = true });
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EBX, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNZ, DestinationRef = Label.PrimaryLabel + ".draw_loop" });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.ESP, SourceRef = "0x8" });//2 items on stack
        }
        
        [Assembly(0x8)]
        public static void Clear(byte* aAddress, uint aColor, int aSize)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceDisplacement = 0x8, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceDisplacement = 0xC, SourceIndirect = true });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EBP, SourceDisplacement = 0x10, SourceIndirect = true });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new ShiftRight { DestinationReg = Registers.ECX, SourceRef = "0x2" });
            Core.AssemblerCode.Add(new Literal("rep stosd"));
            Core.AssemblerCode.Add(new And { DestinationReg = Registers.EBX, SourceRef = "0x3" });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Literal("rep stosd"));
        }
        #endregion
    }
}
