using System;

using Atomix.Kernel_H.core;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.Kernel_H.lib.graphic
{
    public unsafe class Surface
    {
        private byte* aBuffer;
        private int Width;
        private int Height;

        private Rectangle* Rectangle_List;
        private int Rectangle_List_index;
        
        public Surface(byte* backbuffer, int width, int height)
        {
            this.aBuffer = backbuffer;
            this.Width = width;
            this.Height = height;

            this.Rectangle_List = (Rectangle*)Heap.kmalloc(0x10 * 1000);//1000 rectangles
            this.Rectangle_List_index = 0;
        }
        
        public void Fill(byte* bitamp, int x, int y, int w, int h)
        {
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
                Copy24bpp(aBuffer, bitamp, l, m, Width, l - x, m - y, w, n - l + 1, o - m + 1);
            }
        }

        [Assembly(0x28)]
        private static void Copy24bpp(byte* des, byte* src, int des_x, int des_y, int des_width, int src_x, int src_y, int src_width, int data_width, int data_height)
        {
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "3" });
            //EBX = width
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 32 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 28 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 36 });            
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //des + (x1 + y1 * width) * BytesPerPixel
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 48 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDI, SourceReg = Registers.EAX });

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 20 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EBP, DestinationIndirect = true, DestinationDisplacement = 16 });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 24 });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            //src + (x1 + y1 * width) * BytesPerPixel
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 40 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ESI, SourceReg = Registers.EAX });

            //factor1 = (des_width - width) * BytesPerPixel
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 28 });
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });

            //factor2 = (src_width - wdith) * BytesPerPixel
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 16 });
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EAX, SourceReg = Registers.EBX });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX });
            /*
             * STACK:
             * [ESP + 0x0] = factor2
             * [ESP + 0x4] = factor1
             */
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 8 });

            Core.AssemblerCode.Add(new Label(".draw_loop"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EAX, SourceReg = Registers.EBP, SourceIndirect = true, SourceDisplacement = 12 });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "4" });
            Core.AssemblerCode.Add(new Div { DestinationReg = Registers.ECX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EAX });
            Core.AssemblerCode.Add(new Literal("rep movsd"));
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceReg = Registers.EDX });
            Core.AssemblerCode.Add(new Literal("rep movsb"));
            Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.EBX, SourceRef = "0x1" });
            Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNZ, DestinationRef = Label.PrimaryLabel + ".draw_loop" });
        }

        public void Rectangle(int x, int y, int w, int h, int start_index = 0)
        {
            if (w <= 0 || h <= 0)
                return;

            int a, b, c, d, p, q, r, s, l, m, n, o;

            a = x;
            b = y;
            c = a + w - 1;
            d = b + h - 1;

            int count = Rectangle_List_index;
            for (int index = start_index; index < count; index++)
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
            int index = Rectangle_List_index;
            Rectangle_List[index].x = x;
            Rectangle_List[index].y = y;
            Rectangle_List[index].width = w;
            Rectangle_List[index].height = h;
            Rectangle_List_index = index + 1;
        }
    }
}
