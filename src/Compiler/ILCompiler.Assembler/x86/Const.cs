using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomix.Assembler.x86
{
    public class Const
    {
        public static string SizeToString(byte aSize)
        {
            switch (aSize)
            {
                case 8:
                    return "byte";
                case 16:
                    return "word";
                case 32:
                    return "dword";
                case 64:
                    return "qword";
                default:
                    return "dword";
            }
        }
    }
}
