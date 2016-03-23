/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, February 2016                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.plugs                                                                *
*   File          ::  VTable.cs                                                                            *
*                                                                                                          *
*   Description                                                                                            *
*       Implementation of C# .NET Virtual Table Compiler Feature                                           *
*                                                                                                          *
*   History                                                                                                *
*       13-02-2016      Aman Priyadarshi      Added Methods                                                *
*       23-03-2016      Aman Priyadarshi      Added File Header                                            *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.plugs
{
    public static unsafe class VTableImpl
    {
        [Label("VTableImpl")]
        public static uint AddEntry(uint* aTable, uint aTypeID, uint aMethodID)
        {
            uint TypeID, MethodID, Size;
            while((Size = *aTable) != 0)
            {
                TypeID = aTable[1];
                if (TypeID == aTypeID)
                {
                    aTable += 2;
                    while((MethodID = *aTable) != 0)
                    {
                        if (MethodID == aMethodID)
                            return aTable[1];
                    }
                    throw new Exception("[VTable] Method Not Found!");
                }
                aTable += Size;
            }
            throw new Exception("[VTable] Type Not Found!");
        }
    }
}
