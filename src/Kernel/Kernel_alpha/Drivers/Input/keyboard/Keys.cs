using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Drivers.Input
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

        public Keys(KeyCode kc, char c)
        {
            aKeyCode = kc;
            aChar = c;
        }
    }
}
