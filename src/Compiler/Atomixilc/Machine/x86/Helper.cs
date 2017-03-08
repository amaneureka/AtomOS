/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          x86 Helper functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomixilc.Machine.x86
{
    internal static class Helper
    {
        internal static string SizeToString(byte Size)
        {
            switch(Size)
            {
                case 8: return ("byte");
                case 16: return ("word");
                case 32: return ("dword");
                case 64: return ("qword");
                default: throw new Exception(string.Format("Invalid Size '{0}'", Size));
            }
        }
    }
}
