/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Structure defination for different compositor request
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

namespace Atomix.Kernel_H.Gui
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct GuiRequest
    {
        [FieldOffset(0)]
        internal uint HashID;
        [FieldOffset(4)]
        internal int ClientID;
        [FieldOffset(8)]
        internal RequestType Type;
        [FieldOffset(12)]
        internal ErrorType Error;
    };

    [StructLayout(LayoutKind.Explicit, Size = 28)]
    internal unsafe struct MouseData
    {
        [FieldOffset(16)]
        internal int Button;
        [FieldOffset(20)]
        internal int Xpos;
        [FieldOffset(24)]
        internal int Ypos;
    };

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct NewWindow
    {
        [FieldOffset(16)]
        internal int X;
        [FieldOffset(20)]
        internal int Y;
        [FieldOffset(24)]
        internal int Width;
        [FieldOffset(28)]
        internal int Height;
        [FieldOffset(32)]
        internal fixed char Buffer[8];
    };

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    internal struct Redraw
    {
        [FieldOffset(16)]
        internal int WindowID;
        [FieldOffset(20)]
        internal int X;
        [FieldOffset(24)]
        internal int Y;
        [FieldOffset(28)]
        internal int Width;
        [FieldOffset(32)]
        internal int Height;
    }

    internal enum RequestType : uint
    {
        None = 0,
        NewWindow = 1,
        Redraw = 2,
        WindowMove = 3,
        MouseEvent = 4,
        KeyboardEvent = 5
    }

    internal enum ErrorType : uint
    {
        None = 0,
        BadRequest = 1,
        BadParameters = 2,
        OutOfMemory = 3,
        BadFunction = 4,
    }
}
