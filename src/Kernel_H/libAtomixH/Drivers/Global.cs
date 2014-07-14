using System;

using libAtomixH.Drivers.Input;
using libAtomixH.Drivers.Input.PS2;

namespace libAtomixH.Drivers
{
    public static class Global
    {
        public static Keyboard keyboard;

        public static void Initialize ()
        {
            // Initialize the Keyboard
            keyboard = new Keyboard ();
        }
    }
}
