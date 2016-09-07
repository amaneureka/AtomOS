using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.x86
{
    public delegate void INTDelegate();
    public static class xINT
    {
        private static INTDelegate[] mINT_Handlers = new INTDelegate[0xFF];

        public static void RegisterHandler(INTDelegate xCall, int aINTNo)
        {
            mINT_Handlers[aINTNo] = xCall;
        }

        public static void InvokeHandler(uint aINTNo)
        {
            var xCaller = mINT_Handlers[aINTNo];
            if (xCaller != null)
            {
                xCaller();
            }
        }
    }
}
