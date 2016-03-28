using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.core
{
    public static class Environment
    {
        [Label("environment_import_dll")]
        private static uint ImportDLL(string aDLLName, string aMethodName)
        {
            Debug.Write("Hello `%d`\n", pHelloWorld);
            return pHelloWorld;
        }

        private static uint pHelloWorld;
        private static uint HelloWorld(uint a, uint b, uint c)
        {
            Debug.Write("Aman %d\n", a + b - c);
            return (a + b + c);
        }
    }
}
