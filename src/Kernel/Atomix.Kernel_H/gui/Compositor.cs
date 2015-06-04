using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.io.Streams;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.gui
{
    public static class Compositor
    {
        //TODO: It should not be public
        public static Server Server;
        static uint STACK_SERVER;
        static uint STACK_REFRESH;
        const uint MAGIC = 0xDEADCAFE;//I don't know why I put this :P
        
        public static void Setup(Process parent)
        {
            Debug.Write("Compositor Setup\n");
            //why 128? its because 0x1000 / 32 = 128 (0x1000 := 4KB set by the VFS) 
            Server = new Server("sys\\dwm", 32, 128, MAGIC);

            STACK_REFRESH = Heap.kmalloc(0x1000);//4KB
            STACK_SERVER = Heap.kmalloc(0x1000);

            Debug.Write("\tRefresh stack: %d\n", STACK_REFRESH);
            Debug.Write("\tCompositor stack: %d\n", STACK_SERVER);
            
            //Start Refresh Screen Thread
            new Thread(parent, pHandleRequest, STACK_SERVER + 0x1000, 0x1000).Start();

            //Start Handle Request Thread
            new Thread(parent, pRefreshScreen, STACK_REFRESH + 0x1000, 0x1000).Start();
        }

        public static uint pRefreshScreen;
        public static void RefreshScreen()
        {
            while(true)
            {
                //Refresh Screen
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
                    Debug.Write("[compositor]: Invalid Pagic, uid:= %d\n", (uint)uid);

                var header = (RequestHeader)BitConverter.ToUInt32(packet, 8);
                switch(header)
                {
                    case RequestHeader.CREATE_NEW_WINDOW:
                        {
                            Debug.Write("[compositor]: CREATE_NEW_WINDOW, uid:=%d\n", (uint)uid);
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
