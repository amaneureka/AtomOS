using System;
using System.Collections.Generic;

using Atomix.Kernel_H.devices;
using Atomix.Kernel_H.drivers.video;
using Atomix.Kernel_H.drivers.FileSystem;

namespace Atomix.Kernel_H
{
    public static unsafe class DoBoot
    {
        public static uint pAnimation;        
        public static void Animation()
        {
            DrawBackground();

            //Real dimensions 420x26 in 1920x1080 screenshot
            uint width = ((42000 / 1920) * VBE.Xres) / 100;
            uint height = ((2600 / 1080) * VBE.Yres) / 100;
            uint x = VBE.Xres / 2 - width / 2;
            uint y = ((VBE.Yres * 2) / 3) - height / 2;
            RoundedRectangle(x, y, width, height, 0x7A7A7A);//Progress bar background color

            uint Sx = VBE.Xres/2 - 80;
            uint Sy = y - 185;
            
            uint oldtime = Timer.ElapsedMiliSeconds;
            int curr = 1;
            var xFS = VirtualFileSystem.GetFS("sys\\RamFS");
            PrintSprite(Sx, Sy, (UInt32*)xFS.ReadFile(curr));
            while(true)
            {
                if (oldtime + 10 <= Timer.ElapsedMiliSeconds)
                {
                    oldtime = Timer.ElapsedMiliSeconds;
                    PrintSprite(Sx, Sy, (UInt32*)xFS.ReadFile(curr));
                    curr = (curr + 1) % 97;
                }

                if (updated)
                {
                    RoundedRectangle(x + 3, y + 3, ((width - 6) * progress) / 100, height - 6, 0xC3C3C3);
                    updated = false;
                }
            }
        }

        private static bool updated = false;
        private static uint progress = 0;
        public static void DoProgress()
        {
            if (progress == 100)
                return;

            updated = true;
            progress += 1;
        }

        private static void DrawBackground()
        {
            /*
            //Gradient Color
            uint curr = 66, fac, fact2;
            fac = VBE.Yres / (102 - curr);
            fact2 = fac;
            for (uint j = 0; j < VBE.Yres; j++)
            {
                if (j == fac)
                {
                    curr++;
                    fac += fact2;
                }
                for (uint i = 0; i < VBE.Xres; i++)
                {
                    VBE.SetPixel(i, j, (uint)(0xFF << 24 | curr << 16 | curr << 8 | curr));
                }
            }*/
            for (uint j = 0; j < VBE.Yres; j++)
            {
                for (uint i = 0; i < VBE.Xres; i++)
                {
                    VBE.SetPixel(i, j, 0x3D3D3D);
                }
            }
        }

        private static void PrintSprite(uint x, uint y, UInt32* Image)
        {
            uint width = Image[0];
            uint height = Image[1];

            uint p = 2;
            for (uint j = 0; j < height; j++)
            {
                for (uint i = 0; i < width; i++)
                {
                    VBE.SetPixel(x + i, y + j, Image[p++]);
                }
            }
        }

        private static void RoundedRectangle(uint x, uint y, uint width, uint height, uint color)
        {
            uint factor1 = height / 2, factor2 = width - factor1, factor3 = factor1 * factor1  + (factor1 * 8) / 10;
            for (uint i = 0; i < width; i++)
            {
                for (uint j = 0; j < height; j++)
                {
                    if (i <= factor1)
                    {
                        //(x-r)^2 + (y-r)^2 = r^2
                        uint a = i - factor1;
                        a *= a;

                        uint b = j - factor1;
                        b *= b;

                        if (a + b <= factor3)
                        {
                            VBE.SetPixel(i + x, j + y, color);
                        }
                    }
                    else if (i >= factor2)
                    {
                        //(x-width+r)^2 + (y-r)^2 = r^2
                        uint a = i - width + factor1;
                        a *= a;

                        uint b = j - factor1;
                        b *= b;

                        if (a + b <= factor3)
                        {
                            VBE.SetPixel(i + x, j + y, color);
                        }
                    }
                    else
                    {
                        VBE.SetPixel(i + x, j + y, color);
                    }
                }
            }
        }
    }
}
