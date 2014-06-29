using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Kernel_alpha.Drivers.PS2
{
    public class Keyboard
    {
        #region Struct
        [StructLayout(LayoutKind.Explicit, Size = 3)]
        public struct State
        {
            [FieldOffset(0)]
            public bool CapsLock;
            [FieldOffset(1)]
            public bool ScrollLock;
            [FieldOffset(2)]
            public bool NumLock;
        }
        #endregion

        public Keyboard()
        {

        }
    }
}
