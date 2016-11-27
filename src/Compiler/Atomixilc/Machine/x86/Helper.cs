using System;
using System.Collections.Generic;

namespace Atomixilc.Machine.x86
{
    public static class Helper
    {
        public static string SizeToString(byte Size)
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
