/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          To Implement DllImport Attribute over extern method
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.Core;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.exec
{
    internal static class Environment
    {
        const uint MAGIC = 0xEF00FE00;

        [Label(Helper.lblImportDll)]
        private static uint ImportDLL(string aDLLName, string aMethodName)
        {
            string SymbolName = aDLLName + aMethodName;
            uint Address = Scheduler.RunningProcess.GetSymbols(SymbolName);

            if (Address == 0)
            {
                // check if DLL has been loaded or not
                if (Scheduler.RunningProcess.GetSymbols(aDLLName) == MAGIC)
                    throw new Exception("[ImportDLL]: No such symbol found!");

                ELF.Load(aDLLName);
                Scheduler.RunningProcess.SetSymbol(aDLLName, MAGIC);

                Address = Scheduler.RunningProcess.GetSymbols(SymbolName);
                if (Address == 0)
                    throw new Exception("[ImportDLL]: No such symbol found!");
            }
            return Address;
        }
    }
}
