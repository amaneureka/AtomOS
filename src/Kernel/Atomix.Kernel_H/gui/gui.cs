/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          Compositor Helper Functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
