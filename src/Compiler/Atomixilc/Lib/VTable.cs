/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
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
