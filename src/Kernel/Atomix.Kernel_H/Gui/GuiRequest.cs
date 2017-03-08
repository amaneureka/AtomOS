/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
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
    internal unsafe struct MouseEvent
    {
        [FieldOffset(16)]
        internal int WindowID;
        [FieldOffset(20)]
        internal int Button;
        [FieldOffset(24)]
        internal int Xpos;
        [FieldOffset(28)]
        internal int Ypos;
        [FieldOffset(32)]
        internal MouseFunction Function;
    };

    [StructLayout(LayoutKind.Explicit, Size = 44)]
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
        internal int WindowID;
        [FieldOffset(36)]
        internal fixed sbyte Buffer[8];
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
    };

    [StructLayout(LayoutKind.Explicit, Size = 28)]
    internal struct WindowMove
    {
        [FieldOffset(16)]
        internal int WindowID;
        [FieldOffset(20)]
        internal int RelX;
        [FieldOffset(24)]
        internal int RelY;
    };

    [StructLayout(LayoutKind.Explicit, Size = 20)]
    internal struct DragRequest
    {
        [FieldOffset(16)]
        internal int WindowID;
    };

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    internal struct InfoRequest
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
    };

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct Rect
    {
        [FieldOffset(0)]
        internal int X;
        [FieldOffset(4)]
        internal int Y;
        [FieldOffset(8)]
        internal int Width;
        [FieldOffset(12)]
        internal int Height;
    };

    internal enum RequestType : uint
    {
        None = 0,
        NewWindow = 1,
        Redraw = 2,
        WindowMove = 3,
        MouseEvent = 4,
        KeyboardEvent = 5,
        DragRequest = 6,
        InfoRequest = 7
    };

    internal enum ErrorType : uint
    {
        None = 0,
        BadRequest = 1,
        BadParameters = 2,
        OutOfMemory = 3,
        BadFunction = 4,
    };

    internal enum MouseIcon : int
    {
        None = 0,
        Idle = 1,
        Help = 2,
        Clipboard = 3,
        Busy = 4
    };

    internal enum WindowState : int
    {
        None = 0,
        FullScreen = 1,
        Minimized = 2,
        Close = 3,
    };

    internal enum MouseFunction : int
    {
        None = 0,
        KeyUp = 1,
        KeyDown = 2,
        Move = 4,
        Enter = 8,
        Click = 16
    };
}
