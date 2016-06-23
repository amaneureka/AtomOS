/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
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
