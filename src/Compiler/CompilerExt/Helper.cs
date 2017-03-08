/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Helper Class
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

namespace Atomix.CompilerExt
{
    public static class Helper
    {
        /// <summary>
        /// Log file name
        /// </summary>
        public const string LoggerFile = ".log.html";
        /// <summary>
        /// Startup method inside kernel main type
        /// </summary>
        public const string StartupMethod = "main";
        /// <summary>
        /// Output assembly name
        /// </summary>
        public const string KernelFile = "Kernel.asm";

        /* non-optional compiler required label */
        public const string lblSetException = "SetException";
        public const string lblGetException = "GetException";
        public const string lblImportDll = "environment_import_dll";
        public const string lblVTable = "VTableImpl";
        public const string lblHeap = "Heap";
    }

    public enum CPUArch : uint
    {
        none = 0,
        x86 = 1,
        x64 = 2,
        ARM = 3
    };
}
