/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, February 2016                                       *
*                                                                                                          *
*   Namespace     ::  Atomix                                                                               *
*   File          ::  VTable.cs                                                                            *
*                                                                                                          *
*   Description                                                                                            *
*       Implementation of C# .NET Virtual Table Compiler Feature                                           *
*                                                                                                          *
*   History                                                                                                *
*       13-02-2016      Aman Priyadarshi      Added Methods                                                *
*       23-03-2016      Aman Priyadarshi      Added File Header and Typos                                  *
*       25-03-2016      Aman Priyadarshi      Moved to Compiler Internals                                  *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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
