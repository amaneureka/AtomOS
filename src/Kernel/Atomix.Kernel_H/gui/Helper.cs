﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Compositor Helper Functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.lib;

namespace Atomix.Kernel_H.gui
{
    internal static class Helper
    {
        internal static unsafe byte* GetMouseBitamp()
        {
            var aBuffer = (byte*)Heap.kmalloc(32 * 32 * 4);
            
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    //if (32 - j < i && i != j)
                    //    break;
                    int add = ((j*32) + i) * 4;                    
                    aBuffer[add + 0] = 0xFF;
                    aBuffer[add + 1] = 0x00;
                    aBuffer[add + 2] = 0x00;
                    aBuffer[add + 3] = 0x00;
                }
            }
            return aBuffer;
        }

        internal static unsafe byte* GetEmptyScreen()
        {
            var emptyscreen = (byte*)Heap.kmalloc(0x3C000A);
            int xres = drivers.video.VBE.Xres;
            int yres = drivers.video.VBE.Yres;
            for (int i = 0; i < xres; i++)
            {
                for (int j = 0; j < yres; j++)
                {
                    int offset = (i + j*xres)*4;
                    emptyscreen[offset + 0] = 0xAA;
                    emptyscreen[offset + 1] = 0xBB;
                }
            }
            return emptyscreen;
        }

        internal static void CreateNewWindowMessage(byte[] Buffer, int Width, int Height, string WindowHash)
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
            Buffer.SetByte((uint)(13 + WindowHash.Length), 0x0);// End string with '\0'
        }
    }
}
