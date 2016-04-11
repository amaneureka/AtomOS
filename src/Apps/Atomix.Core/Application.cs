using System;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Core
{
    [Kernel(CPUArch.x86, "0x100")]
    public abstract class Application
    {
        public abstract void start(char[] args);

        public abstract void update();

        public void main()
        {
            start(null);
        }

        [Label("Heap")]
        public static void Heap() { }

        [Label("SetException")]
        public static void SetException() { }

        [Label("GetException")]
        public static void GetException() { }
    }
}
