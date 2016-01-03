using System;

namespace Atomix.Kernel_H.gui
{
    public enum RequestHeader : byte
    {
        CREATE_NEW_WINDOW = 0xCC,
        INPUT_MOUSE_EVENT = 0xAC,
        WINDOW_REDRAW = 0xDA,
        WINDOW_MOVE = 0x1A
    }
}
