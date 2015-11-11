using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldstr)]
    public class Ldstr : MSIL
    {
        public Ldstr(Compiler Cmp)
            : base("ldstr", Cmp) { }

        private static int CurrentLabel = 0;
        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            var xStr = ((OpString)instr).Value;
            var Content = "StringContent_" + CurrentLabel.ToString().PadLeft(4, '0');
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {                        
                        Encoding xEncoding = Encoding.Unicode;
                        var xBytecount = xEncoding.GetByteCount(xStr);
                        var xObjectData = new byte[4 + (xBytecount) + 0xC]; //0xC is object data offset

                        Array.Copy(BitConverter.GetBytes((int)-1), 0, xObjectData, 0, 4);
                        Array.Copy(BitConverter.GetBytes(0x80000001), 0, xObjectData, 4, 4);
                        Array.Copy(BitConverter.GetBytes((int)1), 0, xObjectData, 8, 4);
                        Array.Copy(BitConverter.GetBytes(xStr.Length), 0, xObjectData, 12, 4);
                        Array.Copy(xEncoding.GetBytes(xStr), 0, xObjectData, 16, xBytecount);

                        Core.DataMember.Add(new AsmData(Content, xObjectData));

                        Core.AssemblerCode.Add(new Push { DestinationRef = Content });
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
            CurrentLabel++;
            Core.vStack.Push(4, typeof(string));
        }
    }
}
