using System;

using Atomixilc.Attributes;

namespace Atomixilc.Lib
{
    internal class VTable
    {
        [Label("__VTable_GetEntry__")]
        internal static unsafe uint GetEntry(uint* FlushTable, uint TypeID)
        {
            return 0;
        }
    }
}
