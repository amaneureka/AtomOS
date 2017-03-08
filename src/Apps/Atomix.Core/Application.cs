/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Core
{
    public static class Application
    {
        [Label(Helper.lblHeap)]
        public static void Heap() { }
    }
}
