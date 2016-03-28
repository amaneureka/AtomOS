using System;

using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.exec
{
    public static class Environment
    {
        [Label("environment_import_dll")]
        private static uint ImportDLL(string aDLLName, string aMethodName)
        {
            var ModuleAddress = SHM.Obtain(aDLLName, -1);
            if (ModuleAddress == 0)
            {
                //Load DLL from disk
                throw new Exception("Can't Load");
            }
            return 0;
        }
    }
}
