using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public enum CPUArch : uint
    {
        none = 0,
        x86 = 1,
        x64 = 2,
        ARM = 3
    };
}
