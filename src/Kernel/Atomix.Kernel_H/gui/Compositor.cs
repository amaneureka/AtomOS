using System;

using Atomix.Kernel_H.io;
using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;
using Atomix.Kernel_H.lib.graphic;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.drivers.input;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.gui
{
    public static unsafe class Compositor
    {
        #region DEFINATIONS
        static Pipe SERVER;

        static uint MOUSE_INPUT_STACK;
        static uint COMPOSITOR_STACK;
        static uint RENDER_STACK;

        static byte* MouseBuffer;
        static int Mouse_X, Mouse_Y;
        static int WindowList_Lock;
        static int Surface_Lock;
                
        static IList<Pipe> Clients;
        static IList<Window> WindowList;        
        static IDictionary<Window> WindowMap;

        static Surface Canvas;
        #endregion

        const uint PACKET_SIZE = 32;
        public const uint MAGIC = 0xDEADCAFE;

        public static void Setup(Process parent)
        {
            Debug.Write("Compositor Setup\n");
            SERVER = new Pipe(PACKET_SIZE, 10000);
            Clients = new IList<Pipe>(100);
            WindowList = new IList<Window>(100);
            WindowMap = new IDictionary<Window>();
            Canvas = new Surface(VBE.SecondaryBuffer, VBE.Xres, VBE.Yres);

            WindowList_Lock = Scheduler.GetResourceID();
            Surface_Lock = Scheduler.GetResourceID();
            MouseBuffer = Helper.GetMouseBitamp();

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
            int old_mouse_X = 0, old_mouse_Y = 0;
            var emptyscreen = (byte*)Heap.kmalloc(0x3C000A);
            bool Update;
            while(true)
            {
                int tmp_mouse_X = Mouse_X;
                int tmp_mouse_Y = Mouse_Y;
                Update = false;

                if (tmp_mouse_X != old_mouse_X || tmp_mouse_Y != old_mouse_Y)
                {
                    MarkRectangle(old_mouse_X, old_mouse_Y, 32, 32);
                    MarkRectangle(tmp_mouse_X, tmp_mouse_X, 32, 32);
                    old_mouse_X = tmp_mouse_X;
                    old_mouse_Y = tmp_mouse_Y;
                    Update = true;
                }

                if (Update)
                {
                    Scheduler.SpinLock(WindowList_Lock);
                    for (int i = 0; i < WindowList.Count; i++)
                    {
                        var win = WindowList[i];
                        Canvas.Fill((byte*)win.Buffer, win.PositionX, win.PositionY, win.Width, win.Height);
                    }
                    Scheduler.SpinUnlock(WindowList_Lock);

                    Surface.Copy(VBE.SecondaryBuffer, emptyscreen, 0, 0, VBE.Xres, 0, 0, VBE.Xres, VBE.Xres, VBE.Yres);
                    //Canvas.Fill(MouseBuffer, tmp_mouse_X, tmp_mouse_Y, 32, 32);
                    Surface.Copy(VBE.SecondaryBuffer, MouseBuffer, tmp_mouse_X, tmp_mouse_Y, VBE.Xres, 0, 0, 32, 32, 32);
                    
                    //Flip only if we have updates on screen
                    //TODO: Flip only that much of region
                    VBE.Update();
                }
                Canvas.Reset();
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
                int ClientID = BitConverter.ToInt32(compositor_packet, 5);

                if (BitConverter.ToUInt32(compositor_packet, 0) != MAGIC)
                {
                    Debug.Write("Bad Magic id:= %d\n", (uint)ClientID);
                    continue;
                }

                /*
                 * SIZE     OFFSET      DESCRIPTION
                 * 4        0           MAGIC No
                 * 1        4           Function
                 * 4        5           Client ID
                 * 23       9           Function Arguments
                 */
                switch((RequestHeader)compositor_packet[4])
                {
                    case RequestHeader.CREATE_NEW_WINDOW:
                        {
                            /*
                             * SIZE     OFFSET      DESCRIPTION
                             * 4        9           Width
                             * 4        13          Height
                             */
                            int WindowWidth = BitConverter.ToInt32(compositor_packet, 9);
                            int WindowHeight = BitConverter.ToInt32(compositor_packet, 13);
                            var xNewWindow = new Window(ClientID) { Width = WindowWidth, Height = WindowHeight };

                            string HashString = xNewWindow.HashString;
                            xNewWindow.Buffer = SHM.Obtain(HashString, (int)(WindowHeight * WindowWidth * 3), true);
                            
                            //Yes! we overwrite this buffer because we have no further refernce to this
                            Helper.CreateNewWindowMessage(compositor_packet, WindowWidth, WindowHeight, HashString);

                            WindowMap.Add(HashString, xNewWindow);

                            //Spin Lock WindowList because it is shared between compositor and render thread
                            Scheduler.SpinLock(WindowList_Lock);
                            WindowList.Add(xNewWindow);
                            Scheduler.SpinUnlock(WindowList_Lock);

                            Clients[ClientID].Write(compositor_packet, false);
                        }
                        break;
                    case RequestHeader.WINDOW_REDRAW:
                        {
                            string HashCode = lib.encoding.ASCII.GetString(compositor_packet, 9, 23);
                            var Window = WindowMap[HashCode];
                            if (Window != null)//We should give a response to client but leave for now!
                            {
                                MarkRectangle(Window.PositionX, Window.PositionY, Window.Width, Window.Height);
                            }
                        }
                        break;
                    case RequestHeader.WINDOW_MOVE:
                        {

                        }
                        break;
                    case RequestHeader.INPUT_MOUSE_EVENT:
                        {
                            byte p1 = compositor_packet[5];
                            byte p2 = compositor_packet[6];
                            byte p3 = compositor_packet[7];

                            if ((p1 & 0x10) == 0)
                                Mouse_X += p2 << 1;
                            else
                                Mouse_X -= (p2 ^ 0xFF) << 1;

                            if ((p1 & 0x20) == 0)
                                Mouse_Y -= p3 << 1;
                            else
                                Mouse_Y += (p3 ^ 0xFF) << 1;

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
            compositor_packet.SetUInt(0, MAGIC);
            
            while(true)
            {
                Mouse.MousePipe.Read(packet);
                if (packet[0] != Mouse.MOUSE_MAGIC)
                {
                    Debug.Write("Invalid Mouse Packet MAGIC:=%d\n", (uint)packet[0]);
                    continue;
                }
                compositor_packet[4] = (byte)RequestHeader.INPUT_MOUSE_EVENT;
                compositor_packet[5] = packet[1];
                compositor_packet[6] = packet[2];
                compositor_packet[7] = packet[3];
                SERVER.Write(compositor_packet);
            }
            Thread.Die();
        }

        private static void MarkRectangle(int x, int y, int width, int height)
        {
            Scheduler.SpinLock(Surface_Lock);
            Canvas.Rectangle(x, y, width, height);
            Scheduler.SpinUnlock(Surface_Lock);
        }
    }
}
