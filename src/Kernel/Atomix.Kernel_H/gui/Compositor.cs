using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.lib.graphic;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.gui
{
    public static class Compositor
    {
        #region DEFINATIONS
        static Pipe SERVER;

        static uint MOUSE_INPUT_STACK;
        static uint COMPOSITOR_STACK;
        static uint RENDER_STACK;

        static int Mouse_X, Mouse_Y;
        #endregion

        const uint PACKET_SIZE = 32;
        const uint MAGIC = 0xDEADCAFE;
        const int MouseFactor = 2;

        public static void Setup(Process parent)
        {
            Debug.Write("Compositor Setup\n");
            SERVER = new Pipe(PACKET_SIZE, 1000);

            //Threads stack memory allocation
            MOUSE_INPUT_STACK = Heap.kmalloc(0x1000);
            COMPOSITOR_STACK = Heap.kmalloc(0x1000);
            RENDER_STACK = Heap.kmalloc(0x1000);

            new Thread(parent, pHandleMouse, MOUSE_INPUT_STACK + 0x1000, 0x1000).Start();
            new Thread(parent, pHandleRequest, COMPOSITOR_STACK + 0x1000, 0x1000).Start();
            new Thread(parent, pRender, RENDER_STACK + 0x1000, 0x1000).Start();
        }

        public static uint FRAMES;

        private static uint pRender;
        private static unsafe void Render()
        {
            var surface = new Surface(VBE.SecondaryBuffer, VBE.Xres, VBE.Yres);

            int tmp_mouse_X = 0, tmp_mouse_Y = 0, old_mouse_X = 0, old_mouse_Y = 0;

            bool update;

            uint color = 0xFF0000;
            while(true)
            {
                tmp_mouse_X = Mouse_X;
                tmp_mouse_Y = Mouse_Y;
                update = false;

                if (tmp_mouse_X != old_mouse_X || tmp_mouse_Y != old_mouse_Y)
                {
                    //surface.Rectangle(old_mouse_X, old_mouse_Y, 32, 32);
                    //surface.Rectangle(tmp_mouse_X, tmp_mouse_Y, 32, 32);
                    old_mouse_X = tmp_mouse_X;
                    old_mouse_Y = tmp_mouse_Y;
                    update = true;
                }

                color ^= 0xFFFF00;
                for (int i = 0; i < 1366; i++)
                    for (int j = 0; j < 768; j++)
                        VBE.SetPixel(i, j, color);                
                VBE.Update();
                FRAMES++;
            }
            Thread.Die();
        }

        private static uint pHandleRequest;
        private static void HandleRequest()
        {
            var compositor_packet = new byte[PACKET_SIZE];
                        
            while(true)
            {
                SERVER.Read(compositor_packet);
                uint ClientID = BitConverter.ToUInt32(compositor_packet, 5);

                if (BitConverter.ToUInt32(compositor_packet, 0) != MAGIC)
                {
                    Debug.Write("Bad Magic id:= %d\n", ClientID);
                    continue;
                }

                switch((RequestHeader)compositor_packet[4])
                {
                    case RequestHeader.CREATE_NEW_WINDOW:
                        {
                            //Client_ID, Frame Buffer
                        }
                        break;
                    case RequestHeader.INPUT_MOUSE_EVENT:
                        {
                            byte p1 = compositor_packet[5];
                            byte p2 = compositor_packet[6];
                            byte p3 = compositor_packet[7];

                            if ((p1 & 0x10) == 0)
                                Mouse_X += p2 * MouseFactor;
                            else
                                Mouse_X -= (p2 ^ 0xFF) * MouseFactor;

                            if ((p1 & 0x20) == 0)
                                Mouse_Y -= p3 * MouseFactor;
                            else
                                Mouse_Y += (p3 ^ 0xFF) * MouseFactor;

                            if (Mouse_X < 0 || Mouse_X > VBE.Xres)
                                Mouse_X = 0;

                            if (Mouse_Y < 0 || Mouse_Y > VBE.Yres)
                                Mouse_Y = 0;
                        }
                        break;
                    default:
                        Debug.Write("Invalid Request package: %d\n", compositor_packet[4]);
                        break;
                }
            }
            Thread.Die();
        }

        private static uint pHandleMouse;
        private static void HandleMouse()
        {
            var packet = new byte[4];
            var compositor_packet = new byte[PACKET_SIZE];

            compositor_packet[0] = (byte)((MAGIC >> 0) & 0xFF);
            compositor_packet[1] = (byte)((MAGIC >> 8) & 0xFF);
            compositor_packet[2] = (byte)((MAGIC >> 16) & 0xFF);
            compositor_packet[3] = (byte)((MAGIC >> 24) & 0xFF);
            
            while(true)
            {
                Mouse.MousePipe.Read(packet);
                if (packet[0] != Mouse.MOUSE_MAGIC)
                {
                    Debug.Write("Invalid Mouse Packet MAGIC:=%d\n", (uint)packet[0]);
                    continue;
                }
                compositor_packet[4] = (byte)RequestHeader.INPUT_MOUSE_EVENT;
                for (int i = 1; i < 4; i++)
                    compositor_packet[i + 4] = packet[i];
                SERVER.Write(compositor_packet);
            }
            Thread.Die();
        }
    }
}
