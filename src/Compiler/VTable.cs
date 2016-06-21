/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Virtual Table Implementation
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

namespace Atomix
{
    public static class VTable
    {
        public static unsafe uint GetEntry(uint* aTable, uint aTypeID, uint aMethodID)
        {
            uint TypeID, MethodID, Size;
            while ((Size = *aTable) != 0)
            {
                TypeID = aTable[1];
                if (TypeID == aTypeID)
                {
                    aTable += 2;
                    while ((MethodID = *aTable) != 0)
                    {
                        if (MethodID == aMethodID)
                            return aTable[1];
                        aTable += 2;
                    }
                    throw new Exception("[VTable] Method Not Found!");
                }
                aTable += Size;
            }
            throw new Exception("[VTable] Type Not Found!");
        }
    }
}
