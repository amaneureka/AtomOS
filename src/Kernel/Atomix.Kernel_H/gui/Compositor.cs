using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.io.Streams;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.gui
{
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
    }
}
