using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;
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
        private static void Render()
        {
            int tmp_mouse_X = 0, tmp_mouse_Y = 0;
            uint color = 0xFF0000;
            while(true)
            {
                /*
                 * We should hookup this session so that glitch won't happen while we are rendering the current frame                 * 
                 */
                Scheduler.HookUp();
                int curr_mouse_X = Mouse_X;
                int curr_mouse_Y = Mouse_Y;
                Scheduler.UnHook();

                for (uint i = 0; i < 300; i++)
                    for (uint j = 0; j < 300; j++)
                        VBE.SetPixel(i, j, color);

                color ^= 0xFFFF00;
                if (curr_mouse_X != tmp_mouse_X || curr_mouse_Y != tmp_mouse_Y)
                {
                    /*for (uint i = 0; i < 10; i++)
                        for (uint j = 0; j < 10; j++)
                            VBE.SetPixel(i+50, j+50, color);*/
                    
                    VBE.SetPixel((uint)(tmp_mouse_X), (uint)(tmp_mouse_Y), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X + 1), (uint)(tmp_mouse_Y), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X), (uint)(tmp_mouse_Y + 1), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X + 1), (uint)(tmp_mouse_Y + 1), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X + 2), (uint)(tmp_mouse_Y), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X), (uint)(tmp_mouse_Y + 2), 0x0);
                    VBE.SetPixel((uint)(tmp_mouse_X + 2), (uint)(tmp_mouse_Y + 2), 0x0);

                    VBE.SetPixel((uint)(curr_mouse_X), (uint)(curr_mouse_Y), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X + 1), (uint)(curr_mouse_Y), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X), (uint)(curr_mouse_Y + 1), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X + 1), (uint)(curr_mouse_Y + 1), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X + 2), (uint)(curr_mouse_Y), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X), (uint)(curr_mouse_Y + 2), 0xFFFFFFFF);
                    VBE.SetPixel((uint)(curr_mouse_X + 2), (uint)(curr_mouse_Y + 2), 0xFFFFFFFF);
                    
                    tmp_mouse_X = curr_mouse_X;
                    tmp_mouse_Y = curr_mouse_Y;
                }
                Scheduler.HookUp();
                VBE.Update();
                FRAMES++;
                Scheduler.UnHook();

                Thread.Sleep(1);
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

    /*
    public static class Compositor
    {
        //TODO: It should not be public
        static Server Server;
        static uint STACK_SERVER;
        static uint STACK_REFRESH;
        static uint STACK_INPUT_MOUSE;
        static uint MAGIC = 0xDEADCAFE;//I don't know why I put this :P
        
        public static void Setup(Process parent)
        {
            Debug.Write("Compositor Setup\n");
            //why 128? its because 0x1000 / 32 = 128 (0x1000 := 4KB set by the VFS) 
            Server = new Server("sys\\dwm", 32, 128, MAGIC);

            STACK_REFRESH = Heap.kmalloc(0x1000);//4KB
            STACK_SERVER = Heap.kmalloc(0x1000);
            STACK_INPUT_MOUSE = Heap.kmalloc(0x1000);

            Debug.Write("\tRefresh stack: %d\n", STACK_REFRESH);
            Debug.Write("\tCompositor stack: %d\n", STACK_SERVER);
            
            //setup mouse Buffer
            MouseBackBuffer = new UInt32[16 * 16];
            Sprite = new UInt32[16 * 16];
            for (int i = 0; i < Sprite.Length; i++ )
                Sprite[i] = (uint)((0xFF << 16) | (2*i << 8) | i);
            Print.GetBuffer(MouseBackBuffer, Mouse_X, Mouse_Y, 16, 16);
            
            //Start Refresh Screen Thread
            new Thread(parent, pHandleRequest, STACK_SERVER + 0x1000, 0x1000).Start();

            //Start Handle Request Thread
            new Thread(parent, pRefreshScreen, STACK_REFRESH + 0x1000, 0x1000).Start();

            //Start Input Handler
            new Thread(parent, pHandleMouseInputs, STACK_INPUT_MOUSE + 0x1000, 0x1000).Start();
        }

        #region MouseThingy
        static ushort Mouse_X = 0, Mouse_Y = 0;
        static ushort Mouse_Old_X = 0, Mouse_Old_Y = 0;
        static bool MouseUpdate = true;
        static UInt32[] MouseBackBuffer, Sprite;
        #endregion

        public static uint pRefreshScreen;
        public static void RefreshScreen()
        {
            while(true)
            {
                if (MouseUpdate)
                {
                    ushort currX = Mouse_X;
                    ushort currY = Mouse_Y;
                    MouseUpdate = false;
                    Print.Sprite(MouseBackBuffer, Mouse_Old_X, Mouse_Old_Y, 16, 16);
                    Print.Sprite(MouseBackBuffer, Sprite, currX, currY, 16, 16);                    

                    VBE.Update();
                    Mouse_Old_X = currX;
                    Mouse_Old_Y = currY;
                }
            }
        }

        public static uint pHandleMouseInputs;
        public static void HandleMouseInputs()
        {
            var packet = new byte[4];
            var pipe = VirtualFileSystem.Open("sys\\mouse", FileAttribute.READ_ONLY);
            if (pipe == null)
            {
                Debug.Write("[compositor]: Unable to connect to mouse pipe\n");
                while (true) ;
            }

            var Compositor_Packet = new byte[32];
            var Signature = (uint)RequestHeader.INPUT_MOUSE_EVENT;
            Compositor_Packet[4] = (byte)(MAGIC);
            Compositor_Packet[5] = (byte)(MAGIC >> 8);
            Compositor_Packet[6] = (byte)(MAGIC >> 16);
            Compositor_Packet[7] = (byte)(MAGIC >> 24);

            Compositor_Packet[8] = (byte)Signature;
            while(true)
            {
                while (!pipe.Read(packet, 0)) ;
                if (packet[0] != Mouse.MOUSE_MAGIC)
                {
                    Debug.Write("Invalid Mouse Packet\n");
                    continue;
                }
                Compositor_Packet[9] = packet[1];
                Compositor_Packet[10] = packet[2];
                Compositor_Packet[11] = packet[3];
                Server.Send(Compositor_Packet);
            }
        }
        
        public static uint pHandleRequest;
        private static void HandleRequest()
        {
            var packet = new byte[32];
            while(true)
            {
                if (!Server.Receive(packet))
                    Debug.Write("[compositor]: Message Recieve Failed\n");

                int uid = BitConverter.ToInt32(packet, 0);
                uint magic = BitConverter.ToUInt32(packet, 4);

                if (magic != MAGIC)
                    Debug.Write("[compositor]: Invalid magic, uid:= %d\n", (uint)uid);

                switch ((RequestHeader)packet[8])
                {
                    case RequestHeader.CREATE_NEW_WINDOW:
                        {
                            Debug.Write("[compositor]: CREATE_NEW_WINDOW, uid:=%d\n", (uint)uid);
                        }
                        break;
                    case RequestHeader.INPUT_MOUSE_EVENT:
                        {
                            //Debug.Write("[compositor]: INPUT_MOUSE_EVENT, uid:=%d\n", (uint)uid);
                            //packet[9][10][11] -- mouse data [0][1][2]
                            byte a = packet[9];
                            byte b = packet[10];
                            byte c = packet[11];
                            if ((a & 0x10) != 0)
                                Mouse_X -= (byte)(b ^ 0xFF);
                            else
                                Mouse_X += b;

                            if ((a & 0x20) != 0)
                                Mouse_Y += (byte)(c ^ 0xFF);
                            else
                                Mouse_Y -= c;

                            if (Mouse_X > VBE.Xres)
                                Mouse_X = 0;
                            if (Mouse_Y > VBE.Yres)
                                Mouse_Y = 0;
                            MouseUpdate = true;
                        }
                        break;
                    default:
                        Debug.Write("[compositor]: Bad Request Header, uid:=%d\n", (uint)uid);
                        break;
                }
            }
        }
    }*/
}
