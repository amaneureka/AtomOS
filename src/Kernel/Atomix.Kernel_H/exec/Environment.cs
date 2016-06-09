/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is strictly prohibited
*                   Proprietary and confidential
* PURPOSE:          To Implement DllImport Attribute over extern method
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.arch.x86;

using Atomix.CompilerExt.Attributes;

namespace Atomix.Kernel_H.exec
{
    public static class Environment
    {
        [Label("environment_import_dll")]
        private static uint ImportDLL(string aDLLName, string aMethodName)
        {
            string SymbolName = aDLLName + aMethodName;
            uint Address = Scheduler.RunningProcess.GetSymbols(SymbolName);

            if (Address != 0)
                return Address;

            //check if DLL has been loaded or not
            if (Scheduler.RunningProcess.GetSymbols(aDLLName) == 1)
                throw new Exception("[ImportDLL]: No such symbol found!");

            ELF.Load(aDLLName);
            Scheduler.RunningProcess.SetSymbol(aDLLName, 1);

            return Scheduler.RunningProcess.GetSymbols(SymbolName);
        }
    }
}
