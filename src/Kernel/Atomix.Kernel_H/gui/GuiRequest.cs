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
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    internal struct GuiRequest
    {
        [FieldOffset(0)]
        internal uint HashID;
        [FieldOffset(4)]
        internal int ClientID;
        [FieldOffset(8)]
        internal RequestType Type;
    };

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct NewWindow
    {
        [FieldOffset(12)]
        internal uint X;
        [FieldOffset(16)]
        internal uint Y;
        [FieldOffset(20)]
        internal uint Width;
        [FieldOffset(24)]
        internal uint Height;
        [FieldOffset(28)]
        internal ErrorType Error;
        [FieldOffset(32)]
        internal fixed char Hash[8];
    };

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
        InvalidParameters = 1,
    }
}
