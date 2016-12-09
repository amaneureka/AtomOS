using System;

using Atomixilc.Attributes;

namespace Atomixilc.Lib
{
    internal class VTable
    {
        [Label(Helper.VTable_Label)]
        internal static unsafe uint GetEntry(uint* FlushTable, uint TypeID)
        {
            while(*FlushTable != 0)
            {
                if (*FlushTable == TypeID)
                    return *(FlushTable + 1);
                FlushTable += 2;
            }

            return 0;
        }
    }
}
