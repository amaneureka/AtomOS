/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Bitmap Graphics class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;
using Atomix.Kernel_H.Arch.x86;

namespace Atomix.Kernel_H.Gui
{
    internal class Bitmap
    {
        uint mWidth;
        uint mHeight;
        uint mBytesPerPixel;

        uint mBuffer;

        internal uint Width
        { get { return mWidth; } }

        internal uint Height
        { get { return mHeight; } }

        internal uint Buffer
        { get { return mBuffer; } }

        internal Bitmap(string aID, uint aWidth, uint aHeight, uint aBpp = 4)
            :this(SHM.Obtain(aID, aWidth * aHeight * aBpp, true), aWidth, aHeight, aBpp)
        { }

        internal Bitmap(uint aWidth, uint aHeight, uint aBpp = 4)
            :this(Heap.kmalloc(aWidth * aHeight * aBpp), aWidth, aHeight, aBpp)
        { }

        internal Bitmap(uint aBuffer, uint aWidth, uint aHeight, uint aBpp = 4)
        {
            mBuffer = aBuffer;
            mWidth = aWidth;
            mHeight = aHeight;
            mBytesPerPixel = aBpp;
        }

        internal void Draw(Bitmap aImage, uint aXpos, uint aYpos)
        {
            uint width = aImage.Width;
            uint height = aImage.Height;

            width = Math.Min(mWidth - aXpos, width);
            height = Math.Min(mHeight - aYpos, height);

            Draw(aImage, aXpos, aYpos, width, height);
        }

        internal void Draw(Bitmap aImage, uint aXpos, uint aYpos, uint aWidth, uint aHeight)
        {
            uint aDes = mBuffer + (aXpos + ((aYpos * mWidth)) * mBytesPerPixel);
            uint aSrc = aImage.Buffer;
            uint aLen = mBytesPerPixel * aWidth;
            uint aLen2 = mBytesPerPixel * mWidth;

            for (uint i = 0; i < aHeight; i++)
            {
                Memory.FastCopy(aDes, aSrc, aLen);
                aDes += aLen2;
                aSrc += aLen;
            }
        }
    }
}
