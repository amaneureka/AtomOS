using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.lib;

namespace Atomix.Kernel_H.gui
{
    public static class Helper
    {
        public static unsafe byte* GetMouseBitamp()
        {
            var aBuffer = (byte*)Heap.kmalloc(32 * 32 * 3);
            
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    //if (32 - j < i && i != j)
                    //    break;
                    int add = ((j*32) + i) * 4;
                    aBuffer[add + 0] = 0xFF;
                    aBuffer[add + 1] = 0xFF;
                    aBuffer[add + 2] = 0xFF;
                }
            }
            return aBuffer;
        }

        public static void CreateNewWindowMessage(byte[] Buffer, int Width, int Height, string WindowHash)
        {
            /*
             * SIZE     OFFSET      DESCRIPTION
             * 4        0           Compositor Magic
             * 1        4           Message Type
             * 4        5           Width
             * 4        9           Height
             * n/a      13          Window Hash
             */
            Buffer.SetUInt(0, Compositor.MAGIC);
            Buffer.SetByte(4, (byte)RequestHeader.CREATE_NEW_WINDOW);
            Buffer.SetInt(5, Width);
            Buffer.SetInt(9, Height);
            Buffer.SetStringASCII(13, WindowHash);
            Buffer.SetByte((uint)(13 + WindowHash.Length), 0x0);//End string with '\0'
        }

    }
}
