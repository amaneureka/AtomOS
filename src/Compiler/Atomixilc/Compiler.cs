using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using Emit = System.Reflection.Emit;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Atomixilc.IL;
using Atomixilc.Machine;
using Atomixilc.Attributes;
using Atomixilc.Machine.x86;
using Atomixilc.IL.CodeType;

namespace Atomixilc
{
    internal class Compiler
    {
        Type Entrypoint;
        Options Config;
        Dictionary<ILCode, MSIL> ILCodes;
        Dictionary<short, Emit.OpCode> OpCode;

        Dictionary<MethodBase, string> Plugs;
        Dictionary<string, MethodBase> Labels;

        Queue<object> ScanQ;
        HashSet<object> FinishedQ;
        HashSet<string> StringTable;
        HashSet<MethodInfo> Globals;
        HashSet<MethodInfo> Virtuals;

        List<FunctionalBlock> CodeSegment;
        Dictionary<string, int> ZeroSegment;
        Dictionary<string, AsmData> DataSegment;

        internal Compiler(Options aCompilerOptions)
        {
            Config = aCompilerOptions;

            PrepareEnvironment();
        }

        internal void PrepareEnvironment()
        {
            var ExecutingAssembly = Assembly.GetExecutingAssembly();

            ILCodes = new Dictionary<ILCode, MSIL>();
            Plugs = new Dictionary<MethodBase, string>();
            Labels = new Dictionary<string, MethodBase>();
            ScanQ = new Queue<object>();
            FinishedQ = new HashSet<object>();
            Virtuals = new HashSet<MethodInfo>();
            Globals = new HashSet<MethodInfo>();
            OpCode = new Dictionary<short, Emit.OpCode>();

            ZeroSegment = new Dictionary<string, int>();
            DataSegment = new Dictionary<string, AsmData>();
            CodeSegment = new List<FunctionalBlock>();
            StringTable = new HashSet<string>();

            var types = ExecutingAssembly.GetTypes();
            foreach (var type in types)
            {
                var ILattributes = type.GetCustomAttributes<ILImplAttribute>();
                foreach (var attrib in ILattributes)
                {
                    Verbose.Message("[MSIL] {0}", type.ToString());
                    ILCodes.Add(attrib.OpCode, (MSIL)Activator.CreateInstance(type));
                }
            }

            var ilOpcodes = typeof(Emit.OpCodes).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            foreach (var xField in ilOpcodes)
            {
                var xOpCode = (Emit.OpCode)xField.GetValue(null);
                Verbose.Message("[OpCode] {0} [0x{1}]", xOpCode, xOpCode.Value.ToString("X4"));
                OpCode.Add(xOpCode.Value, xOpCode);
            }
        }

        internal void Execute()
        {
            ScanQ.Clear();
            Globals.Clear();
            Virtuals.Clear();
            FinishedQ.Clear();
            ZeroSegment.Clear();
            DataSegment.Clear();
            CodeSegment.Clear();
            StringTable.Clear();

            Helper.DataSegment.Clear();
            Helper.ZeroSegment.Clear();
            Helper.cachedFieldLabel.Clear();
            Helper.cachedMethodLabel.Clear();
            Helper.cachedResolvedStringLabel.Clear();

            Entrypoint = null;
            ScanInputAssembly(out Entrypoint);

            if (Entrypoint == null)
                throw new Exception("No input entrypoint found");

            var main = Entrypoint.GetMethod("main");
            if (main == null)
                throw new Exception("No main function found");

            ScanQ.Enqueue(main);

            IncludePlugAndLibrary();

            while (ScanQ.Count != 0)
            {
                var ScanObject = ScanQ.Dequeue();

                if (FinishedQ.Contains(ScanObject))
                    continue;

                var method = ScanObject as MethodBase;
                if (method != null)
                {
                    Verbose.Message("Scanning Method : {0}", method.FullName());
                    ScanMethod(method);
                    continue;
                }

                var type = ScanObject as Type;
                if (type != null)
                {
                    Verbose.Message("Scanning Type : {0}", type.FullName);
                    ScanType(type);
                    continue;
                }

                var field = ScanObject as FieldInfo;
                if (field != null)
                {
                    Verbose.Message("Scanning Field : {0}", field.FullName());
                    ProcessFieldInfo(field);
                    continue;
                }

                throw new Exception(string.Format("Invalid Object in Queue of type '{0}'", ScanObject.GetType()));
            }
        }

        internal void Flush()
        {
            FlushVTables();
            FlushStringTable();

            switch (Config.TargetPlatform)
            {
                case Architecture.x86:
                    Flushx86();
                    break;
                default:
                    throw new Exception("Unsupported Flush Platform");
            }
        }

        private void Flushx86()
        {
            using (var SW = new StreamWriter(Config.OutputFile))
            {
                var attrib = Entrypoint.GetCustomAttribute<EntrypointAttribute>();
                if (attrib == null)
                    throw new Exception("Internal compiler error");

                SW.WriteLine("global entrypoint");
                SW.WriteLine(string.Format("entrypoint equ {0}", attrib.Entrypoint));

                foreach (var global in Globals)
                    SW.WriteLine(string.Format("global {0}", global.FullName()));
                SW.WriteLine();

                SW.WriteLine("section .bss");
                foreach (var bssEntry in Helper.ZeroSegment)
                    SW.WriteLine(string.Format("{0} resb {1}", bssEntry.Key, bssEntry.Value));
                foreach (var bssEntry in ZeroSegment)
                    SW.WriteLine(string.Format("{0} resb {1}", bssEntry.Key, bssEntry.Value));
                SW.WriteLine();

                SW.WriteLine("section .data");
                foreach (var dataEntry in Helper.DataSegment)
                    SW.WriteLine(dataEntry);
                foreach (var dataEntry in DataSegment)
                    SW.WriteLine(dataEntry.Value);
                SW.WriteLine();

                SW.WriteLine("section .text");
                foreach (var block in CodeSegment)
                {
                    var xbody = block.Body;
                    foreach (var code in xbody)
                    {
                        if (!(code is Label))
                            SW.Write("    ");
                        else
                            SW.WriteLine();

                        if (code is Call)
                        {
                            var xCall = (Call)code;
                            if (xCall.IsLabel)
                            {
                                xCall.IsLabel = false;
                                xCall.DestinationRef = Labels[xCall.DestinationRef].FullName();
                            }
                        }

                        SW.WriteLine(code);
                    }
                    SW.WriteLine();
                }

                SW.WriteLine("Compiler_End:");

                SW.Flush();
                SW.Close();
            }
        }

        private void FlushStringTable()
        {
            var encoding = Encoding.Unicode;
            foreach(var str in StringTable)
            {
                int count = encoding.GetByteCount(str);
                var data = new byte[count + 0x10];

                Array.Copy(BitConverter.GetBytes(typeof(string).GetHashCode()), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(0x1), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(data.Length), 0, data, 8, 4);
                Array.Copy(BitConverter.GetBytes(str.Length), 0, data, 12, 4);
                Array.Copy(encoding.GetBytes(str), 0, data, 16, count);

                var label = Helper.GetResolvedStringLabel(str);
                DataSegment.Add(label, new AsmData(label, data));
            }
        }

        private void FlushVTables()
        {
            var tables = new List<KeyValuePair<int, MethodInfo> >();
            foreach(var method in Virtuals)
            {
                var baseDef = method.GetBaseDefinition();
                if (!baseDef.IsAbstract)
                    continue;

                tables.Add(new KeyValuePair<int, MethodInfo>(baseDef.GetHashCode(), method));
            }

            tables.Add(new KeyValuePair<int, MethodInfo>(int.MaxValue, null));

            tables.Sort((x, y) => x.Key.CompareTo(y.Key));

            int MethodUID = 0;
            List<string> data = null;
            foreach(var item in tables)
            {
                if (item.Key != MethodUID)
                {
                    MethodUID = item.Key;
                    if (data != null)
                    {
                        var label = Helper.GetVTableFlush(MethodUID);
                        DataSegment.Add(label, new AsmData(label, data.ToArray()));
                    }

                    if (MethodUID == int.MaxValue)
                        break;

                    data = new List<string>();
                }

                data.Add(item.Value.DeclaringType.GetHashCode().ToString());
                data.Add(item.Value.FullName());
            }
        }

        internal void IncludePlugAndLibrary()
        {
            ScanType(typeof(Lib.VTable));

            foreach (var plug in Plugs)
                ScanQ.Enqueue(plug.Key);

            foreach (var label in Labels)
                ScanQ.Enqueue(label.Value);
        }

        internal void ScanInputAssembly(out Type Entrypoint)
        {
            var InputAssembly = Assembly.LoadFile(Config.InputFiles[0]);

            var types = InputAssembly.GetTypes();

            Entrypoint = null;
            Plugs.Clear();
            Labels.Clear();
            foreach (var type in types)
            {
                var entrypointattrib = type.GetCustomAttribute<EntrypointAttribute>();
                if (entrypointattrib != null && entrypointattrib.Platform == Config.TargetPlatform)
                {
                    if (Entrypoint != null)
                        throw new Exception("Multiple Entrypoint with same target platform");
                    Entrypoint = type;
                }

                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var plugattrib = method.GetCustomAttribute<PlugAttribute>();
                    if (plugattrib != null && plugattrib.Platform == Config.TargetPlatform)
                    {
                        if (Plugs.ContainsValue(plugattrib.TargetLabel))
                            throw new Exception(string.Format("Multiple plugs with same target label '{0}'", plugattrib.TargetLabel));
                        Verbose.Message("[Plug] {0} : {1}", plugattrib.TargetLabel, method.FullName());
                        method.AddPlug(plugattrib.TargetLabel);
                        Plugs.Add(method, plugattrib.TargetLabel);
                    }

                    var labelattrib = method.GetCustomAttribute<LabelAttribute>();
                    if (labelattrib != null)
                    {
                        if (Labels.ContainsKey(labelattrib.RefLabel))
                            throw new Exception(string.Format("Multiple labels with same Ref label '{0}'", labelattrib.RefLabel));
                        Verbose.Message("[Label] {0} : {1}", labelattrib.RefLabel, method.FullName());
                        Labels.Add(labelattrib.RefLabel, method);
                    }
                }
            }
        }

        internal void ScanType(Type type)
        {
            if (type.BaseType != null)
                ScanQ.Enqueue(type.BaseType);

            var constructors = type.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var ctor in constructors)
            {
                if (ctor.DeclaringType != type)
                    continue;
                ScanQ.Enqueue(ctor);
            }

            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                var plugattrib = method.GetCustomAttribute<PlugAttribute>();
                if (plugattrib != null && !Plugs.ContainsKey(method))
                {
                    ScanQ.Enqueue(method);
                    Plugs.Add(method, plugattrib.TargetLabel);
                    Verbose.Message("[Plug] {0} : {1}", plugattrib.TargetLabel, method.FullName());
                }

                var labelattrib = method.GetCustomAttribute<LabelAttribute>();
                if (labelattrib != null && !Labels.ContainsKey(labelattrib.RefLabel))
                {
                    ScanQ.Enqueue(method);
                    Labels.Add(labelattrib.RefLabel, method);
                    Verbose.Message("[Label] {0} : {1}", labelattrib.RefLabel, method.FullName());
                }
            }

            methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var basedefination = method.GetBaseDefinition();

                if (Virtuals.Contains(method) ||
                    !basedefination.IsAbstract ||
                    method.DeclaringType.IsAbstract ||
                    basedefination.DeclaringType == method.DeclaringType)
                    continue;

                Virtuals.Add(method);
                ScanQ.Enqueue(method);
            }

            FinishedQ.Add(type);
        }

        internal void ScanMethod(MethodBase method)
        {
            FunctionalBlock block = null;

            if (!Helper.RestrictedAssembly.Contains(method.DeclaringType.Assembly.GetName().Name)
                && method.IsPublic
                && method.DeclaringType.IsVisible
                && (method as MethodInfo) != null)
                Globals.Add((MethodInfo)method);

            if (method.GetCustomAttribute<AssemblyAttribute>() != null)
                ProcessAssemblyMethod(method, ref block);
            else if (method.GetCustomAttribute<DllImportAttribute>() != null)
                ProcessExternMethod(method, ref block);
            else if (typeof(Delegate).IsAssignableFrom(method.DeclaringType))
                ProcessDelegate(method, ref block);
            else
                ProcessMethod(method, ref block);

            if (block != null)
                CodeSegment.Add(block);

            FinishedQ.Add(method);
        }

        internal void ProcessAssemblyMethod(MethodBase method, ref FunctionalBlock block)
        {
            var attrib = method.GetCustomAttribute<AssemblyAttribute>();
            if (attrib == null)
                throw new Exception("Invalid call to ProcessAssemblyMethod");

            block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);

            Instruction.Block = block;

            new Label(method.FullName());

            if (attrib.CalliHeader)
                EmitHeader(block, method, 0);

            try
            {
                method.Invoke(null, new object[method.GetParameters().Length]);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Exception occured while invoking assembly function '{0}' => {1}", method.FullName(), e.ToString()));
            }

            if (attrib.CalliHeader)
                EmitFooter(block, method);

            Instruction.Block = null;
        }

        internal void ProcessExternMethod(MethodBase method, ref FunctionalBlock block)
        {
            var attrib = method.GetCustomAttribute<DllImportAttribute>();
            if (attrib == null)
                throw new Exception("Invalid call to ProcessExternMethod");
            Verbose.Error("Extern Method not support '{0}'", method.FullName());
        }

        internal void ProcessFieldInfo(FieldInfo fieldInfo)
        {
            var name = fieldInfo.FullName();
            int size = Helper.GetTypeSize(fieldInfo.FieldType, Config.TargetPlatform);

            InsertData(name, size);
            FinishedQ.Add(fieldInfo);
        }

        internal void InsertData(string name, int size)
        {
            if (ZeroSegment.ContainsKey(name))
            {
                if (ZeroSegment[name] != size)
                    Verbose.Error("Two different size for same field label '{0}' : '{1}' '{2}'", name, ZeroSegment[name], size);
                return;
            }

            ZeroSegment.Add(name, size);
        }

        internal void ProcessDelegate(MethodBase method, ref FunctionalBlock block)
        {
            if (method.Name == ".ctor")
            {
                block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);
                Instruction.Block = block;

                // void.ctor(System.Object, IntPtr)
                new Label(method.FullName());

                EmitHeader(block, method, 0);
                new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x10, SourceIndirect = true };
                new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
                // [Memory + 0x0] := Intptr
                new Mov { DestinationReg = Register.EAX, DestinationDisplacement = 0xC, DestinationIndirect = true, SourceReg = Register.EDX };
                new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
                // [Memory + 0x4] := Object
                new Mov { DestinationReg = Register.EAX, DestinationDisplacement = 0x10, DestinationIndirect = true, SourceReg = Register.EDX };
                EmitFooter(block, method);

                Instruction.Block = null;
            }
            else if (method.Name == "Invoke")
            {
                block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);
                Instruction.Block = block;

                // Return-Type Invoke(params)
                new Label(method.FullName());

                int ESPOffset = 4;
                var xparams = method.GetParameters();

                foreach (var par in xparams)
                    ESPOffset += Helper.GetTypeSize(par.ParameterType, Config.TargetPlatform, true);

                new Mov { DestinationReg = Register.EDX, SourceReg = Register.ESP, SourceDisplacement = ESPOffset, SourceIndirect = true };
                new Mov { DestinationReg = Register.EAX, SourceReg = Register.EDX, SourceDisplacement = 0xC, SourceIndirect = true };
                new Pop { DestinationReg = Register.EDX };
                new Mov { DestinationReg = Register.ESP, DestinationDisplacement = ESPOffset - 4, DestinationIndirect = true, SourceReg = Register.EDX };
                new Call { DestinationRef = "EAX" };
                new Ret { Offset = 0 };

                Instruction.Block = null;
            }
            else
            {
                Verbose.Error("Unimplemented delegate function '{0}'", method.Name);
            }
        }

        internal void ProcessMethod(MethodBase method, ref FunctionalBlock block)
        {
            var Body = method.GetMethodBody();
            if (Body == null)
            {
                Verbose.Warning("Body == null");
                return;
            }

            var MethodName = method.FullName();
            if (Plugs.ContainsValue(MethodName) && !Plugs.ContainsKey(method))
                return;

            var parameters = method.GetParameters();
            foreach(var param in parameters)
            {
                ScanQ.Enqueue(param.ParameterType);
            }

            var localvars = Body.LocalVariables;

            int bodySize = 0;
            foreach(var localvar in localvars)
            {
                bodySize += Helper.GetTypeSize(localvar.LocalType, Config.TargetPlatform, true);
                ScanQ.Enqueue(localvar.LocalType);
            }

            block = new FunctionalBlock(MethodName, Config.TargetPlatform, CallingConvention.StdCall);

            Instruction.Block = block;

            new Label(method.FullName());

            if (method.IsStatic && method is ConstructorInfo)
            {
                EmitConstructor(block, method);
            }

            EmitHeader(block, method, bodySize);

            var Optimizer = new Optimizer(Config);
            var ReferencedPositions = new HashSet<int>();
            var xOpCodes = EmitOpCodes(method, ReferencedPositions);
            foreach (var xOp in xOpCodes)
            {
                if (xOp is OpMethod)
                    ScanQ.Enqueue(((OpMethod)xOp).Value);
                else if (xOp is OpType)
                    ScanQ.Enqueue(((OpType)xOp).Value);
                else if (xOp is OpField)
                {
                    var xOpField = ((OpField)xOp).Value;
                    ScanQ.Enqueue(xOpField.DeclaringType);
                    if (xOpField.IsStatic)
                        ScanQ.Enqueue(xOpField);
                }
                else if (xOp is OpToken)
                {
                    var xOpToken = (OpToken)xOp;
                    if (xOpToken.IsType)
                        ScanQ.Enqueue(xOpToken.ValueType);
                    else if (xOpToken.IsField)
                    {
                        ScanQ.Enqueue(xOpToken.ValueField.DeclaringType);
                        if (xOpToken.ValueField.IsStatic)
                            ScanQ.Enqueue(xOpToken.ValueField);
                    }
                }
                else if (xOp is OpString)
                {
                    var xOpStr = (OpString)xOp;
                    StringTable.Add(xOpStr.Value);
                }

                if (ReferencedPositions.Contains(xOp.Position))
                    new Label(Helper.GetLabel(xOp.Position));

                if (xOp.NeedHandler)
                {
                    EmitExceptionHandler(block, method);
                    Optimizer.vStack.Push(new StackItem(typeof(Exception)));
                }

                MSIL ILHandler = null;
                ILCodes.TryGetValue(xOp.ILCode, out ILHandler);
                if (ILHandler == null)
                {
                    new Comment(string.Format("Unimplemented ILCode '{0}'", xOp.ILCode));
                    Verbose.Error("Unimplemented ILCode '{0}'", xOp.ILCode);
                }
                else
                    ILHandler.Execute(Config, xOp, method, Optimizer);
            }

            EmitFooter(block, method);

            if (Optimizer.vStack.Count != 0)
                Verbose.Warning("vStack.Count != 0");

            Instruction.Block = null;
        }

        internal void EmitHeader(FunctionalBlock block, MethodBase method, int stackspace)
        {
            switch(block.CallingConvention)
            {
                case CallingConvention.StdCall:
                    {
                        switch(block.Platform)
                        {
                            case Architecture.x86:
                                {
                                    new Push { DestinationReg = Register.EBP };
                                    new Mov { DestinationReg = Register.EBP, SourceReg = Register.ESP };
                                    if (stackspace > 0)
                                        new Sub { DestinationReg = Register.ESP, SourceRef = "0x" + stackspace.ToString("X") };
                                }
                                break;
                            default:
                                throw new Exception(string.Format("Unsupported Platform method '{0}'", method.FullName()));
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported CallingConvention used in method '{0}'", method.FullName()));
            }
        }

        internal void EmitExceptionHandler(FunctionalBlock block, MethodBase method)
        {
            switch (block.CallingConvention)
            {
                case CallingConvention.StdCall:
                    {
                        switch (block.Platform)
                        {
                            case Architecture.x86:
                                {
                                    new Push { DestinationReg = Register.ECX };
                                }
                                break;
                            default:
                                throw new Exception(string.Format("Unsupported Platform method '{0}'", method.FullName()));
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported CallingConvention used in method '{0}'", method.FullName()));
            }
        }

        internal void EmitConstructor(FunctionalBlock block, MethodBase method)
        {
            if ((method is ConstructorInfo) == false)
                throw new Exception(string.Format("Illegal call to EmitConstructor by '{0}'", method.FullName()));

            switch (block.Platform)
            {
                case Architecture.x86:
                    {
                        var key = method.ConstructorKey();
                        InsertData(key, 1);

                        new Test { DestinationRef = key, DestinationIndirect = true, SourceRef = "0x1" };
                        new Jmp { Condition = ConditionalJump.JZ, DestinationRef = ".Load" };
                        new Ret { Offset = 0x0 };
                        new Label(".Load");
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported Platform method '{0}'", method.FullName()));
            }
        }

        internal void EmitFooter(FunctionalBlock block, MethodBase method)
        {
            var paramsSize = method.GetParameters().Sum(arg => Helper.GetTypeSize(arg.ParameterType, Config.TargetPlatform, true));

            if (paramsSize > 255) throw new Exception(string.Format("Too large stack frame for parameters '{0}'", method.FullName()));

            var functionInfo = method as MethodInfo;

            int returncount = 0;
            if (functionInfo != null && functionInfo.ReturnType != typeof(void))
                returncount = Helper.GetTypeSize(functionInfo.ReturnType, Config.TargetPlatform, true);

            switch (block.CallingConvention)
            {
                case CallingConvention.StdCall:
                    {
                        switch (block.Platform)
                        {
                            case Architecture.x86:
                                {
                                    new Label(".End");
                                    new Xor { DestinationReg = Register.ECX, SourceReg = Register.ECX };

                                    new Label(".Error");

                                    if (returncount != 0)
                                    {
                                        if (returncount > 4)
                                            throw new Exception("Return type > 4 not supported");
                                        new Pop { DestinationReg = Register.EAX };
                                    }

                                    new Leave { };
                                    new Ret { Offset = (byte)paramsSize };
                                }
                                break;
                            default:
                                throw new Exception(string.Format("Unsupported Platform method '{0}'", method.FullName()));
                        }
                    }
                    break;
                default:
                    throw new Exception(string.Format("Unsupported CallingConvention used in method '{0}'", method.FullName()));
            }
        }

        internal List<OpCodeType> EmitOpCodes(MethodBase method, HashSet<int> ReferencedPositions)
        {
            var body = method.GetMethodBody();
            if (body == null)
                throw new Exception(string.Format("illegal call to EmitOpCodes '{0}'", method.FullName()));

            var byteCode = body.GetILAsByteArray();

            Type[] genericTypeArgs = null;
            Type[] genericMethodArgs = null;
            var EmittedOpCodes = new List<OpCodeType>();

            if (method.DeclaringType.IsGenericType)
                genericTypeArgs = method.DeclaringType.GetGenericArguments();

            if (method.IsGenericMethod)
                genericMethodArgs = method.GetGenericArguments();

            ILCode xOpCodeVal;
            Emit.OpCode xOpCode;
            OpCodeType xOpCodeType;
            ExceptionHandlingClause xCurrentHandler;

            int index = 0;
            while (index < byteCode.Length)
            {
                xCurrentHandler = null;
                foreach (ExceptionHandlingClause xHandler in body.ExceptionHandlingClauses)
                {
                    if (xHandler.TryOffset >= 0)
                    {
                        if (xHandler.TryOffset <= index && (xHandler.TryLength + xHandler.TryOffset + 1) > index)
                        {
                            if (xCurrentHandler == null)
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                            else if (xHandler.TryOffset > xCurrentHandler.TryOffset && (xHandler.TryLength + xHandler.TryOffset) < (xCurrentHandler.TryLength + xCurrentHandler.TryOffset))
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                        }
                    }
                    if (xHandler.HandlerOffset > 0)
                    {
                        if (xHandler.HandlerOffset <= index && (xHandler.HandlerOffset + xHandler.HandlerLength + 1) > index)
                        {
                            if (xCurrentHandler == null)
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                            else if (xHandler.HandlerOffset > xCurrentHandler.HandlerOffset && (xHandler.HandlerOffset + xHandler.HandlerLength) < (xCurrentHandler.HandlerOffset + xCurrentHandler.HandlerLength))
                            {
                                xCurrentHandler = xHandler;
                                continue;
                            }
                        }
                    }
                    if ((xHandler.Flags & ExceptionHandlingClauseOptions.Filter) > 0)
                    {
                        if (xHandler.FilterOffset > 0)
                        {
                            if (xHandler.FilterOffset <= index)
                            {
                                if (xCurrentHandler == null)
                                {
                                    xCurrentHandler = xHandler;
                                    continue;
                                }
                                else if (xHandler.FilterOffset > xCurrentHandler.FilterOffset)
                                {
                                    xCurrentHandler = xHandler;
                                    continue;
                                }
                            }
                        }
                    }
                }

                if (xCurrentHandler != null)
                    ReferencedPositions.Add(xCurrentHandler.HandlerOffset);

                int position = index;

                if (byteCode[index] == 0xFE)
                {
                    xOpCode = OpCode[(short)(0xFE00 | byteCode[index + 1])];
                    index += 2;
                }
                else
                {
                    xOpCode = OpCode[byteCode[index]];
                    index++;
                }

                xOpCodeVal = (ILCode)xOpCode.Value;

                switch (xOpCode.OperandType)
                {
                    case Emit.OperandType.InlineNone:
                        {
                            switch (xOpCodeVal)
                            {
                                case ILCode.Ldarg_0:
                                    xOpCodeType = new OpVar(ILCode.Ldarg, position, index, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_1:
                                    xOpCodeType = new OpVar(ILCode.Ldarg, position, index, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_2:
                                    xOpCodeType = new OpVar(ILCode.Ldarg, position, index, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldarg_3:
                                    xOpCodeType = new OpVar(ILCode.Ldarg, position, index, 3, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_0:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_1:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_2:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_3:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 3, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_4:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 4, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_5:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 5, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_6:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 6, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_7:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 7, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_8:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, 8, xCurrentHandler);
                                    break;
                                case ILCode.Ldc_I4_M1:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index, -1, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_0:
                                    xOpCodeType = new OpVar(ILCode.Ldloc, position, index, 0, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_1:
                                    xOpCodeType = new OpVar(ILCode.Ldloc, position, index, 1, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_2:
                                    xOpCodeType = new OpVar(ILCode.Ldloc, position, index, 2, xCurrentHandler);
                                    break;
                                case ILCode.Ldloc_3:
                                    xOpCodeType = new OpVar(ILCode.Ldloc, position, index, 3, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_0:
                                    xOpCodeType = new OpVar(ILCode.Stloc, position, index, 0, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_1:
                                    xOpCodeType = new OpVar(ILCode.Stloc, position, index, 1, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_2:
                                    xOpCodeType = new OpVar(ILCode.Stloc, position, index, 2, xCurrentHandler);
                                    break;
                                case ILCode.Stloc_3:
                                    xOpCodeType = new OpVar(ILCode.Stloc, position, index, 3, xCurrentHandler);
                                    break;
                                default:
                                    xOpCodeType = new OpNone(xOpCodeVal, position, index, xCurrentHandler);
                                    break;
                            }
                        }
                        break;
                    case Emit.OperandType.ShortInlineBrTarget:
                        {
                            int xTarget = index + 1 + (sbyte)byteCode[index];

                            index++;
                            ReferencedPositions.Add(xTarget);
                            switch (xOpCodeVal)
                            {
                                case ILCode.Beq_S:
                                    xOpCodeType = new OpBranch(ILCode.Beq, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bge_S:
                                    xOpCodeType = new OpBranch(ILCode.Bge, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bge_Un_S:
                                    xOpCodeType = new OpBranch(ILCode.Bge_Un, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bgt_S:
                                    xOpCodeType = new OpBranch(ILCode.Bgt, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bgt_Un_S:
                                    xOpCodeType = new OpBranch(ILCode.Bgt_Un, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Ble_S:
                                    xOpCodeType = new OpBranch(ILCode.Ble, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Ble_Un_S:
                                    xOpCodeType = new OpBranch(ILCode.Ble_Un, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Blt_S:
                                    xOpCodeType = new OpBranch(ILCode.Blt, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Blt_Un_S:
                                    xOpCodeType = new OpBranch(ILCode.Blt_Un, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Bne_Un_S:
                                    xOpCodeType = new OpBranch(ILCode.Bne_Un, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Br_S:
                                    xOpCodeType = new OpBranch(ILCode.Br, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Brfalse_S:
                                    xOpCodeType = new OpBranch(ILCode.Brfalse, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Brtrue_S:
                                    xOpCodeType = new OpBranch(ILCode.Brtrue, position, index, xTarget, xCurrentHandler);
                                    break;
                                case ILCode.Leave_S:
                                    xOpCodeType = new OpBranch(ILCode.Leave, position, index, xTarget, xCurrentHandler);
                                    break;
                                default:
                                    xOpCodeType = new OpBranch(xOpCodeVal, position, index, xTarget, xCurrentHandler);
                                    break;
                            }
                        }
                        break;
                    case Emit.OperandType.InlineBrTarget:
                        {
                            int xTarget = index + 4 + BitConverter.ToInt32(byteCode, index);
                            index += 4;
                            ReferencedPositions.Add(xTarget);
                            xOpCodeType = new OpBranch(xOpCodeVal, position, index, xTarget, xCurrentHandler);
                        }
                        break;
                    case Emit.OperandType.ShortInlineI:
                        {
                            switch (xOpCodeVal)
                            {
                                case ILCode.Ldc_I4_S:
                                    xOpCodeType = new OpInt(ILCode.Ldc_I4, position, index + 1, ((sbyte)byteCode[index]), xCurrentHandler);
                                    break;
                                default:
                                    xOpCodeType = new OpInt(xOpCodeVal, position, index + 1, ((sbyte)byteCode[index]), xCurrentHandler);
                                    break;
                            }
                            index++;
                        }
                        break;
                    case Emit.OperandType.InlineI:
                        xOpCodeType = new OpInt(xOpCodeVal, position, index + 4, BitConverter.ToInt32(byteCode, index), xCurrentHandler);
                        index += 4;
                        break;
                    case Emit.OperandType.InlineI8:
                        xOpCodeType = new OpInt64(xOpCodeVal, position, index + 8, BitConverter.ToInt64(byteCode, index), xCurrentHandler);
                        index += 8;
                        break;
                    case Emit.OperandType.ShortInlineR:
                        xOpCodeType = new OpSingle(xOpCodeVal, position, index + 4, BitConverter.ToSingle(byteCode, index), xCurrentHandler);
                        index += 4;
                        break;
                    case Emit.OperandType.InlineR:
                        xOpCodeType = new OpDouble(xOpCodeVal, position, index + 8, BitConverter.ToDouble(byteCode, index), xCurrentHandler);
                        index += 8;
                        break;
                    case Emit.OperandType.InlineField:
                        {
                            var xValue = method.Module.ResolveField(BitConverter.ToInt32(byteCode, index), genericTypeArgs, genericMethodArgs);
                            xOpCodeType = new OpField(xOpCodeVal, position, index + 4, xValue, xCurrentHandler);
                            index += 4;
                        }
                        break;
                    case Emit.OperandType.InlineMethod:
                        {
                            var xValue = method.Module.ResolveMethod(BitConverter.ToInt32(byteCode, index), genericTypeArgs, genericMethodArgs);
                            xOpCodeType = new OpMethod(xOpCodeVal, position, index + 4, xValue, xCurrentHandler);
                            index += 4;
                        }
                        break;
                    case Emit.OperandType.InlineSig:
                        xOpCodeType = new OpSig(xOpCodeVal, position, index + 4, BitConverter.ToInt32(byteCode, index), xCurrentHandler);
                        index += 4;
                        break;
                    case Emit.OperandType.InlineString:
                        xOpCodeType = new OpString(xOpCodeVal, position, index + 4, method.Module.ResolveString((int)BitConverter.ToInt32(byteCode, index)), xCurrentHandler);
                        index += 4;
                        break;
                    case Emit.OperandType.InlineSwitch:
                        {
                            int xCount = BitConverter.ToInt32(byteCode, index);
                            index += 4;
                            int xNextOpPos = index + xCount * 4;
                            int[] xBranchLocations = new int[xCount];
                            for (int i = 0; i < xCount; i++)
                            {
                                xBranchLocations[i] = xNextOpPos + BitConverter.ToInt32(byteCode, index + i * 4);
                                ReferencedPositions.Add(xBranchLocations[i]);
                            }
                            xOpCodeType = new OpSwitch(xOpCodeVal, position, xNextOpPos, xBranchLocations, xCurrentHandler);
                            index = xNextOpPos;
                        }
                        break;
                    case Emit.OperandType.InlineTok:
                        xOpCodeType = new OpToken(xOpCodeVal, position, index + 4, BitConverter.ToInt32(byteCode, index), method.Module, genericTypeArgs, genericMethodArgs, xCurrentHandler);
                        index += 4;
                        break;
                    case Emit.OperandType.InlineType:
                        {
                            var xValue = method.Module.ResolveType(BitConverter.ToInt32(byteCode, index), genericTypeArgs, genericMethodArgs);
                            xOpCodeType = new OpType(xOpCodeVal, position, index + 4, xValue, xCurrentHandler);
                            index += 4;
                        }
                        break;
                    case Emit.OperandType.ShortInlineVar:
                        switch (xOpCodeVal)
                        {
                            case ILCode.Ldloc_S:
                                xOpCodeType = new OpVar(ILCode.Ldloc, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            case ILCode.Ldloca_S:
                                xOpCodeType = new OpVar(ILCode.Ldloca, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            case ILCode.Ldarg_S:
                                xOpCodeType = new OpVar(ILCode.Ldarg, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            case ILCode.Ldarga_S:
                                xOpCodeType = new OpVar(ILCode.Ldarga, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            case ILCode.Starg_S:
                                xOpCodeType = new OpVar(ILCode.Starg, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            case ILCode.Stloc_S:
                                xOpCodeType = new OpVar(ILCode.Stloc, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                            default:
                                xOpCodeType = new OpVar(xOpCodeVal, position, index + 1, byteCode[index], xCurrentHandler);
                                break;
                        }
                        index++;
                        break;
                    case Emit.OperandType.InlineVar:
                        xOpCodeType = new OpVar(xOpCodeVal, position, index + 2, BitConverter.ToUInt16(byteCode, index), xCurrentHandler);
                        index += 2;
                        break;
                    default:
                        throw new Exception("Internal Compiler error" + xOpCode.OperandType);
                }
                EmittedOpCodes.Add(xOpCodeType);
            }

            return EmittedOpCodes;
        }
    }
}
