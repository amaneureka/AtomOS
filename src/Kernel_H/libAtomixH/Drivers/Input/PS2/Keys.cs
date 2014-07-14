using System;

namespace libAtomixH.Drivers.Input.PS2
{
    public class Keys
    {
        protected KeyCode aKeyCode;
        protected char aChar;
        public KeyCode Code
        {
            get { return aKeyCode; }
        }

        public char Char
        {
            get { return aChar; }
        }

        public Keys (KeyCode kc, char c)
        {
            aKeyCode = kc;
            aChar = c;
        }
    }
}
