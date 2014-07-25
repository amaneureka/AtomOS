using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.x86
{
    public delegate void IRQDelegate();
    public static class xINT
    {
        private static IRQDelegate[] mIRQ_Handlers = new IRQDelegate[256];

        public static void SetIRQHandler(int aIRQNo, IRQDelegate xCall)
        {
            mIRQ_Handlers[aIRQNo + 0x20] = xCall;
        }

        public static void CallIRQ(int aIRQNo)
        {
            var xCaller = mIRQ_Handlers[aIRQNo + 0x20];            
            if (xCaller != null)
            {
                xCaller();
            }
        }
    }
}
