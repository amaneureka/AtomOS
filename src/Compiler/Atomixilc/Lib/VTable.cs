/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Virtual Table Get Entry Implementation
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomixilc.Attributes;

namespace Atomixilc.Lib
{
    internal class VTable
    {
        [Label(Helper.VTable_Label)]
        internal static unsafe int GetEntry(int* FlushTable, int MethodUID, int TypeID)
        {
            while(*FlushTable != 0)
            {
                var xUID = *(FlushTable + 1);
                if (xUID == MethodUID)
                {
                    FlushTable += 2;
                    while(*FlushTable != 0)
                    {
                        var xTypeID = *(FlushTable + 1);
                        if (xTypeID == TypeID)
                            return *FlushTable;
                        FlushTable += 2;
                    }

                    throw new Exception("virtual method not found");
                }

                FlushTable += *FlushTable;
            }

            throw new Exception("virtual method not found");
        }
    }
}
