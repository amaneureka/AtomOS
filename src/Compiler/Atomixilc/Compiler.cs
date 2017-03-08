/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler entrypoint
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

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
        /// <summary>
        /// Compiler Configurations
        /// </summary>
        Options Config;

        /// <summary>
        /// Input Assembly Entrypoint from which Kernel should be loaded
        /// </summary>
        Type Entrypoint;

        /// <summary>
        /// ILCode to Compiler's MSIL Implementation mapping
        /// </summary>
        Dictionary<ILCode, MSIL> ILCodes;

        /// <summary>
        /// IL byte code to OpCode Type mapping
        /// </summary>
        Dictionary<short, Emit.OpCode> OpCode;

        /// <summary>
        /// MethodBase (override Implementation) to target method's label mapping
        /// </summary>
        Dictionary<MethodBase, string> Plugs;

        /// <summary>
        /// Unique ID to Methodbase (Implementaion) mapping
        /// </summary>
        Dictionary<string, MethodBase> Labels;

        /// <summary>
        /// Compiler Scanner Queue
        /// </summary>
        Queue<object> ScanQ;

        /// <summary>
        /// Compiler Processed Item set
        /// </summary>
        HashSet<object> FinishedQ;

        /// <summary>
        /// Processed String Entries set
        /// </summary>
        HashSet<string> StringTable;

        /// <summary>
        /// Exportable MethodInfo from ELF Image
        /// </summary>
        HashSet<MethodInfo> Globals;

        /// <summary>
        /// External Symbols
        /// </summary>
        HashSet<MethodBase> Externals;

        /// <summary>
        /// Inherit methods
        /// </summary>
        HashSet<MethodInfo> Virtuals;

        /// <summary>
        /// Processed Code blocks
        /// </summary>
        List<FunctionalBlock> CodeSegment;

        /// <summary>
        /// Processed Bss Entries
        /// </summary>
        Dictionary<string, int> ZeroSegment;

        /// <summary>
        /// Processed Data Entries
        /// </summary>
        Dictionary<string, AsmData> DataSegment;

        /// <summary>
        /// Compiler Constructor
        /// </summary>
        /// <param name="aCompilerOptions">Configurations</param>
        internal Compiler(Options aCompilerOptions)
        {
            Config = aCompilerOptions;

            PrepareEnvironment();
        }

        /// <summary>
        /// Initialise variables and load MSIL, OpCode, ByteCode mappings
        /// </summary>
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
            Externals = new HashSet<MethodBase>();
            OpCode = new Dictionary<short, Emit.OpCode>();

            ZeroSegment = new Dictionary<string, int>();
            DataSegment = new Dictionary<string, AsmData>();
            CodeSegment = new List<FunctionalBlock>();
            StringTable = new HashSet<string>();

            /* load OpCode <-> MSIL Implementation mapping */
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

            /* load byte code to OpCode type mapping */
            var ilOpcodes = typeof(Emit.OpCodes).GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
            foreach (var xField in ilOpcodes)
            {
                var xOpCode = (Emit.OpCode)xField.GetValue(null);
                Verbose.Message("[OpCode] {0} [0x{1}]", xOpCode, xOpCode.Value.ToString("X4"));
                OpCode.Add(xOpCode.Value, xOpCode);
            }
        }

        /// <summary>
        /// Clean previous session of compiler and execute new compilation stage
        /// </summary>
        internal void Execute()
        {
            /* clean up mess we've created in last session :P */
            ScanQ.Clear();
            Globals.Clear();
            Virtuals.Clear();
            Externals.Clear();
            FinishedQ.Clear();
            ZeroSegment.Clear();
            DataSegment.Clear();
            CodeSegment.Clear();
            StringTable.Clear();

            /* so much mess around :P */
            Helper.DataSegment.Clear();
            Helper.ZeroSegment.Clear();
            Helper.cachedFieldLabel.Clear();
            Helper.cachedMethodLabel.Clear();
            Helper.cachedResolvedStringLabel.Clear();

            Entrypoint = null;

            /* Anyways, time to scan our new Assembly */
            ScanInputAssembly(out Entrypoint);

            /* why on earth somebody would do this mistake
             * but you never know, what is going in a programmer's brain :P
             * they sometime really don't know what they are doing :P
             */
            if (Entrypoint == null)
                throw new Exception("No input entrypoint found");

            /* Anyways, I am adding all these kind of exceptions throughout the compiler
             * because I know, sometime I act very stupid
             */
            var main = Entrypoint.GetMethod("main");
            if (main == null)
                throw new Exception("No main function found");

            /* I will be speechless if somebody asked me why to Enqueue main function :P */
            ScanQ.Enqueue(main);
            IncludePlugAndLibrary();

            /* The real work :D kidding :P */
            while (ScanQ.Count != 0)
            {
                var ScanObject = ScanQ.Dequeue();

                if (FinishedQ.Contains(ScanObject))
                    continue;

                /* while writing this code, I though there could be a lot of ways
                 * to write this branch, but hey what is the most beautiful way?
                 * now look below, Isn't it look beautiful? full of symmetry
                 * Thanks me later :P
                 */

                /* Some serious stuff :p
                 * Though ScanQ Enqueue Objects but compiler assume it only of these three types
                 */

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

        /// <summary>
        /// Flush Compiler output
        /// </summary>
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

        /// <summary>
        /// Flush method targeting x86 arch only
        /// </summary>
        private void Flushx86()
        {
            using (var SW = new StreamWriter(Config.OutputFile))
            {
                var attrib = Entrypoint.GetCustomAttribute<EntrypointAttribute>();
                if (attrib == null)
                    throw new Exception("Internal compiler error");

                /* Assembly header */
                SW.WriteLine("global entrypoint");
                SW.WriteLine(string.Format("entrypoint equ {0}", attrib.Entrypoint));

                /* Global Symbols */
                foreach (var global in Globals)
                    SW.WriteLine(string.Format("global {0}", global.FullName()));
                SW.WriteLine();

                /* External Symbols */
                foreach (var method in Externals)
                    SW.WriteLine(string.Format("extern {0}", method.FullName()));
                SW.WriteLine();

                /* BSS Section */
                SW.WriteLine("section .bss");
                foreach (var bssEntry in Helper.ZeroSegment)
                    SW.WriteLine(string.Format("{0} resb {1}", bssEntry.Key, bssEntry.Value));

                var bssEntries = ZeroSegment.ToList();
                bssEntries.Sort((x, y) => y.Value.CompareTo(x.Value));
                foreach (var bssEntry in bssEntries)
                    SW.WriteLine(string.Format("{0} resb {1}", bssEntry.Key, bssEntry.Value));
                SW.WriteLine();

                /* Data Section */
                SW.WriteLine("section .data");
                foreach (var dataEntry in Helper.DataSegment)
                    SW.WriteLine(dataEntry);
                foreach (var dataEntry in DataSegment)
                    SW.WriteLine(dataEntry.Value);
                SW.WriteLine();

                /* Code Section */
                SW.WriteLine("section .text");
                foreach (var block in CodeSegment)
                {
                    var xbody = block.Body;
                    foreach (var code in xbody)
                    {
                        /* Some styping and indentation thing, because I love beautiful code :P */
                        if (!(code is Label))
                            SW.Write("    ");
                        else
                            SW.WriteLine();

                        /* Is this a call by label name?
                         * yes, replace label with original label name
                         */
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
                    /* some more styling */
                    SW.WriteLine();
                }

                SW.WriteLine("Compiler_End:");

                SW.Flush();
                SW.Close();
            }
        }

        /// <summary>
        /// Flush string table, create data entries for all discovered strings
        /// </summary>
        private void FlushStringTable()
        {
            var encoding = Encoding.Unicode;
            foreach(var str in StringTable)
            {
                int count = encoding.GetByteCount(str);
                var data = new byte[count + 0x10];

                /* String Entry
                 *      |string_type_id|        : typeof(string).GetHashCode()
                 *      |"0x1"|                 : object identifier ID "0x1" : object, "0x2" : array
                 *      |entry_size|            : total memory length in bytes
                 *      |string_length|         : string length in characters
                 *      |char_0|                : data entry
                 *      |char_1|
                 *        ...
                 */

                Array.Copy(BitConverter.GetBytes(typeof(string).GetHashCode()), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(0x1), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(data.Length), 0, data, 8, 4);
                Array.Copy(BitConverter.GetBytes(str.Length), 0, data, 12, 4);
                Array.Copy(encoding.GetBytes(str), 0, data, 16, count);

                var label = Helper.GetResolvedStringLabel(str);
                DataSegment.Add(label, new AsmData(label, data));
            }
        }

        /// <summary>
        /// FlushVTable to create a lookup table in datasegment
        /// </summary>
        private void FlushVTables()
        {
            /* VTables are implemented based on the idea of lookup tables
             * Structure:
             *      |next_block_offset|             : points to next block offset
             *      |method_uid|                    : MethodBase UID
             *          |method_address||type_id|   : MethodInfo Address and DeclaringType ID
             *          |method_address||type_id|
             *          |method_address||type_id|
             *          |"0"|                       : End of this block
             *      |next_block_offset|
             *      |method_uid|
             *          |method_address||type_id|
             *          |method_address||type_id|
             *          |method_address||type_id|
             *          |"0"|
             *      |"0"|                           : End of VTable
             */

            var count = new Dictionary<int, int>();
            var tables = new List<KeyValuePair<int, MethodInfo> >();
            foreach(var method in Virtuals)
            {
                var baseDef = method.GetBaseDefinition();
                if (!baseDef.IsAbstract)
                    continue;

                int UID = baseDef.GetHashCode();

                if (count.ContainsKey(UID))
                    count[UID]++;
                else
                    count.Add(UID, 1);

                tables.Add(new KeyValuePair<int, MethodInfo>(UID, method));
            }

            tables.Sort((x, y) => x.Key.CompareTo(y.Key));

            var methodgroup = count.ToList();
            methodgroup.Sort((x, y) => x.Key.CompareTo(y.Key));

            int index = 0;
            List<string> data = new List<string>();
            foreach(var methodgroupitem in methodgroup)
            {
                var offset = (methodgroupitem.Value * 2) + 3;
                data.Add(offset.ToString()); // methods count
                data.Add(methodgroupitem.Key.ToString()); // method UID
                for (;  index < tables.Count; index++)
                {
                    var item = tables[index];
                    if (item.Key != methodgroupitem.Key)
                        break;
                    data.Add(item.Value.FullName()); // method address
                    data.Add(item.Value.DeclaringType.GetHashCode().ToString()); // type ID
                }

                data.Add("0");
            }

            data.Add("0");

            DataSegment.Add(Helper.VTable_Flush, new AsmData(Helper.VTable_Flush, data.ToArray()));
        }

        /// <summary>
        /// Include Compiler Provided Libraries
        /// </summary>
        internal void IncludePlugAndLibrary()
        {
            /* Hey, I know what you are thinking
             * this code doesn't look good, right?
             * But you don't know how it feels to debug the whole compiler continously for 3 days
             * that much of debugging completely drain your skills :P
             * don't mess up this code, and move forward
             */
            ScanQ.Enqueue(typeof(Lib.Libc));
            ScanQ.Enqueue(typeof(Lib.VTable));
            ScanQ.Enqueue(typeof(Lib.Memory));
            ScanQ.Enqueue(typeof(Lib.Native));
            ScanQ.Enqueue(typeof(Lib.Plugs.ArrayImpl));
            ScanQ.Enqueue(typeof(Lib.Plugs.StringImpl));
            ScanQ.Enqueue(typeof(Lib.Plugs.ExceptionImpl));
            ScanQ.Enqueue(typeof(Lib.Plugs.BitConverterImpl));

            foreach (var plug in Plugs)
                ScanQ.Enqueue(plug.Key);

            foreach (var label in Labels)
                ScanQ.Enqueue(label.Value);
        }

        /// <summary>
        /// Scan Input assembly for entrypoint, plugs/labels
        /// </summary>
        /// <param name="Entrypoint"></param>
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
                    /* look for plugs */
                    var plugattrib = method.GetCustomAttribute<PlugAttribute>();
                    if (plugattrib != null && plugattrib.Platform == Config.TargetPlatform)
                    {
                        if (Plugs.ContainsValue(plugattrib.TargetLabel))
                            throw new Exception(string.Format("Multiple plugs with same target label '{0}'", plugattrib.TargetLabel));
                        Verbose.Message("[Plug] {0} : {1}", plugattrib.TargetLabel, method.FullName());
                        method.AddPlug(plugattrib.TargetLabel);
                        Plugs.Add(method, plugattrib.TargetLabel);
                    }

                    /* look for labels */
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

        /// <summary>
        /// Scan type for constructors, plugs, labels and virtuals
        /// </summary>
        /// <param name="type"></param>
        internal void ScanType(Type type)
        {
            if (type.BaseType != null)
                ScanQ.Enqueue(type.BaseType);

            /* Scan for constructors */
            var constructors = type.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var ctor in constructors)
            {
                if (ctor.DeclaringType != type)
                    continue;
                ScanQ.Enqueue(ctor);
            }

            /* Scan for plugs/labels */
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                /* You might ask, why I am scanning plugs/labels again (ScanInputAssembly already doing this)
                 * well the reason is, ScanInputAssembly is called for scanning input assembly only
                 * but we are inserting compiler implementations too in build process
                 * they might also have plugs/labels
                 */

                /* scan plugs */
                var plugattrib = method.GetCustomAttribute<PlugAttribute>();
                if (plugattrib != null && !Plugs.ContainsKey(method))
                {
                    ScanQ.Enqueue(method);
                    method.AddPlug(plugattrib.TargetLabel);
                    Plugs.Add(method, plugattrib.TargetLabel);
                    Verbose.Message("[Plug] {0} : {1}", plugattrib.TargetLabel, method.FullName());
                }

                /* scan labels */
                var labelattrib = method.GetCustomAttribute<LabelAttribute>();
                if (labelattrib != null && !Labels.ContainsKey(labelattrib.RefLabel))
                {
                    ScanQ.Enqueue(method);
                    Labels.Add(labelattrib.RefLabel, method);
                    Verbose.Message("[Label] {0} : {1}", labelattrib.RefLabel, method.FullName());
                }
            }

            /* Scan for virtual methods */
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

        /// <summary>
        /// Scan Method and process if recognized format
        /// </summary>
        /// <param name="method">Method function</param>
        internal void ScanMethod(MethodBase method)
        {
            FunctionalBlock block = null;

            /* Hey? Is this label exportable? */
            if (!Helper.RestrictedAssembly.Contains(method.DeclaringType.Assembly.GetName().Name)
                && method.IsPublic
                && method.DeclaringType.IsVisible
                && (method as MethodInfo) != null)
                Globals.Add((MethodInfo)method);

            /* Wow! Is it somekind of special method? :P */

            /* Assembly method */
            if (method.GetCustomAttribute<AssemblyAttribute>() != null)
                ProcessAssemblyMethod(method, ref block);

            /* Dll Import? Dynamic external function */
            else if (method.GetCustomAttribute<DllImportAttribute>() != null)
                ProcessExternMethod(method, ref block);

            /* Is this a delegate? wow! */
            else if (typeof(Delegate).IsAssignableFrom(method.DeclaringType))
                ProcessDelegate(method, ref block);

            /* lol, sorry It was just a normal method :P */
            else
                ProcessMethod(method, ref block);

            /* no function body? great! You're dead for me now */
            if (block != null)
                CodeSegment.Add(block);

            /* I am not going to treat you again :P */
            FinishedQ.Add(method);
        }

        /// <summary>
        /// Process Assembly attributed function
        /// </summary>
        /// <param name="method">method function</param>
        /// <param name="block">code block</param>
        internal void ProcessAssemblyMethod(MethodBase method, ref FunctionalBlock block)
        {
            var attrib = method.GetCustomAttribute<AssemblyAttribute>();
            /* bwahahha, though this method is being called by ScanMethod only so, this attrib should not be null!
             * but I really don't myself when I am coding sleepless
             */
            if (attrib == null)
                throw new Exception("Invalid call to ProcessAssemblyMethod");

            if (attrib.CalliHeader == false && method.GetCustomAttribute<NoExceptionAttribute>() == null)
                Verbose.Error("NoException Attribute not present '{0}' CalliHeader == false", method.FullName());

            /* I am sure captain that we have discovered something, can we have our new functional block? */
            block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);

            /* Instructions capturing ON! */
            Instruction.Block = block;

            /* Function entry label */
            new Label(method.FullName());

            /* Some assembly method don't really need a calliHeader -- Optimization thing */
            if (attrib.CalliHeader)
                EmitHeader(block, method, 0);

            /* This is the simplest method processing, all you have to do is, execute that function
             * Instruction will be created and captured by Instruction.Block hence our fucntions block
             */
            try
            {
                method.Invoke(null, new object[method.GetParameters().Length]);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Exception occured while invoking assembly function '{0}' => {1}", method.FullName(), e.ToString()));
            }

            /* No header so no footer :P */
            if (attrib.CalliHeader)
                EmitFooter(block, method);

            /* this isn't necessary, but again I can't take bet on my sleepless coding :P */
            Instruction.Block = null;
        }

        /// <summary>
        /// Process dynamic loadable methods
        /// </summary>
        /// <param name="method"></param>
        /// <param name="block"></param>
        internal void ProcessExternMethod(MethodBase method, ref FunctionalBlock block)
        {
            var attrib = method.GetCustomAttribute<DllImportAttribute>();
            if (attrib == null)
                throw new Exception("Invalid call to ProcessExternMethod");

            Externals.Add(method);
            Verbose.Message("Extern Method Found '{0}'", method.FullName());
        }

        /// <summary>
        /// Process and Add fieldInfo data/entry
        /// </summary>
        /// <param name="fieldInfo"></param>
        internal void ProcessFieldInfo(FieldInfo fieldInfo)
        {
            var name = fieldInfo.FullName();
            int size = Helper.GetTypeSize(fieldInfo.FieldType, Config.TargetPlatform);

            /* simply add this to BSS segment with given size */
            InsertData(name, size);
            FinishedQ.Add(fieldInfo);
        }

        /// <summary>
        /// Helper function to add data to BSS segment
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        internal void InsertData(string name, int size)
        {
            /* Again I can't take bet on my stupidity */
            if (ZeroSegment.ContainsKey(name))
            {
                if (ZeroSegment[name] != size)
                    /* please log, if I did some stupidity */
                    Verbose.Error("Two different size for same field label '{0}' : '{1}' '{2}'", name, ZeroSegment[name], size);
                return;
            }

            ZeroSegment.Add(name, size);
        }

        /// <summary>
        /// Process Delegates
        /// </summary>
        /// <param name="method"></param>
        /// <param name="block"></param>
        internal void ProcessDelegate(MethodBase method, ref FunctionalBlock block)
        {
            /* This is difficult to explain :p believe me, I wasted a lot of time in designing and optimizing this stuff :P
             * But I am going to explain it :P hahaha ready? scroll down!
             */

            /* two important methods are implemented, "constructor" and "invoke" */
            if (method.Name == ".ctor")
            {
                /* constructor */
                block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);
                Instruction.Block = block;

                /* method signature: void.ctor(System.Object, IntPtr)
                 * because this is non-static method, original signature would be : void.ctor(memory, System.Object, IntPtr)
                 * memory points to just allocated memory for this delegate, so we can use it to store info
                 * 0xC byte size metadata -- added by compiler
                 * [memory + 0xC] := IntPtr
                 * [memory + 0x10] := System.Object
                 */

                new Label(method.FullName());

                EmitHeader(block, method, 0);
                new Mov { DestinationReg = Register.EAX, SourceReg = Register.EBP, SourceDisplacement = 0x10, SourceIndirect = true };
                new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0x8, SourceIndirect = true };
                // [Memory + 0xC] := Intptr
                new Mov { DestinationReg = Register.EAX, DestinationDisplacement = 0xC, DestinationIndirect = true, SourceReg = Register.EDX };
                new Mov { DestinationReg = Register.EDX, SourceReg = Register.EBP, SourceDisplacement = 0xC, SourceIndirect = true };
                // [Memory + 0x10] := Object
                new Mov { DestinationReg = Register.EAX, DestinationDisplacement = 0x10, DestinationIndirect = true, SourceReg = Register.EDX };
                EmitFooter(block, method);

                Instruction.Block = null;
            }
            else if (method.Name == "Invoke")
            {
                block = new FunctionalBlock(method.FullName(), Config.TargetPlatform, CallingConvention.StdCall);
                Instruction.Block = block;

                /* Ah! I really can't explain this :P It was so difficult to write
                 * signature : void.ctor(memory, params ...)
                 * check if System.Object [memory + 0x10] is null or not
                 * if null, simply call Intptr [memory + 0xC] after pushing params on to the stack
                 * if not null, first push System.Object [memory + 0x10] then push push params and then call Intptr [memory + 0xC]
                 */

                // Return-Type Invoke(params)
                new Label(method.FullName());

                int ESPOffset = 4;
                var xparams = method.GetParameters();

                foreach (var par in xparams)
                    ESPOffset += Helper.GetTypeSize(par.ParameterType, Config.TargetPlatform, true);

                new Mov { DestinationReg = Register.EAX, SourceReg = Register.ESP, SourceDisplacement = ESPOffset, SourceIndirect = true };
                new Add { DestinationReg = Register.EAX, SourceRef = "0xC" };
                new Cmp { DestinationReg = Register.EAX, DestinationDisplacement = 0x4, DestinationIndirect = true, SourceRef = "0x0" };
                new Jmp { Condition = ConditionalJump.JNE, DestinationRef = ".push_object_ref" };

                new Pop { DestinationReg = Register.EDX };
                new Mov { DestinationReg = Register.ESP, DestinationDisplacement = ESPOffset - 4, DestinationIndirect = true, SourceReg = Register.EDX };
                new Call { DestinationRef = "[EAX]" };
                new Ret { Offset = 0 };

                new Label(".push_object_ref");
                new Push { DestinationReg = Register.EAX, DestinationDisplacement = 0x4, DestinationIndirect = true };
                for (int i = ESPOffset; i > 4; i -= 4)
                    new Push { DestinationReg = Register.ESP, DestinationDisplacement = ESPOffset, DestinationIndirect = true };

                new Call { DestinationRef = "[EAX]" };

                new Ret { Offset = (byte)ESPOffset };

                Instruction.Block = null;
            }
            else
            {
                /* wohooo! Aman didn't implement you. just go to hell! :P */
                Verbose.Error("Unimplemented delegate function '{0}'", method.Name);
            }
        }

        /// <summary>
        /// Process normal method body
        /// </summary>
        /// <param name="method"></param>
        /// <param name="block"></param>
        internal void ProcessMethod(MethodBase method, ref FunctionalBlock block)
        {
            var Body = method.GetMethodBody();
            if (Body == null)
            {
                /* wow! you don't have a body? Great! */
                Verbose.Warning("Body == null");
                return;
            }

            var MethodName = method.FullName();

            /* You are not allowed sir! if you have been implemented by a plug */
            if (Plugs.ContainsValue(MethodName) && !Plugs.ContainsKey(method))
                return;

            /* Some dark magic is happening below; I a not going to explain everything :p */
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
                /* Is static? Is constructor? hey you can't be called more than once */
                EmitConstructor(block, method);
            }

            EmitHeader(block, method, bodySize);

            var ReferencedPositions = new HashSet<int>();
            var xOpCodes = EmitOpCodes(method, ReferencedPositions).ToDictionary(IL => IL.Position);

            var ILQueue = new Queue<int>();
            ILQueue.Enqueue(0);

            var ILBlocks = new Dictionary<int, FunctionalBlock>();
            var Optimizer = new Optimizer(Config, ILQueue);
            while(ILQueue.Count != 0)
            {
                int index = ILQueue.Dequeue();

                /* Create room for new IL and add it to Blocks List */
                var xOp = xOpCodes[index];
                if (xOp == null) continue;

                xOpCodes[index] = null;

                var tempBlock = new FunctionalBlock(null, Config.TargetPlatform, CallingConvention.StdCall);
                Instruction.Block = tempBlock;
                ILBlocks.Add(xOp.Position, tempBlock);

                /* scan inline OpCodes, maybe we get some treasure */
                if (xOp is OpMethod)
                    /* Wow! I found a method, Lucky me :P */
                    ScanQ.Enqueue(((OpMethod)xOp).Value);
                else if (xOp is OpType)
                    /* Wow! I found a Type, Great! xD */
                    ScanQ.Enqueue(((OpType)xOp).Value);
                else if (xOp is OpField)
                {
                    /* Wow! I found a field, Cool */
                    var xOpField = ((OpField)xOp).Value;
                    ScanQ.Enqueue(xOpField.DeclaringType);
                    if (xOpField.IsStatic)
                        ScanQ.Enqueue(xOpField);
                }
                else if (xOp is OpToken)
                {
                    /* Ah! I found a toek, Thanks :) */
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
                    /* Please! no more treasure :P */
                    var xOpStr = (OpString)xOp;
                    StringTable.Add(xOpStr.Value);
                }

                /* Thank god, It's over xD */

                /* Add label to branch/refernced locations */
                if (ReferencedPositions.Contains(xOp.Position))
                    new Label(Helper.GetLabel(xOp.Position));

                /* Load state of dynamic state */
                Optimizer.LoadStack(xOp.Position);

                if (xOp.NeedHandler)
                {
                    /* sir You asked me load execption object onto the stack? sure sir! */
                    EmitExceptionHandler(block, method);
                    Optimizer.vStack.Push(new StackItem(typeof(Exception)));
                }

                /* darkest magic of all time */
                MSIL ILHandler = null;
                ILCodes.TryGetValue(xOp.ILCode, out ILHandler);
                if (ILHandler == null)
                {
                    /* please I can't implement more IL code */
                    new Comment(string.Format("Unimplemented ILCode '{0}'", xOp.ILCode));
                    Verbose.Error("Unimplemented ILCode '{0}'", xOp.ILCode);
                }
                else
                {
                    new Comment(string.Format("[{0}] : {1} => {2}", xOp.ILCode.ToString(), xOp.ToString(), Optimizer.vStack.Count));
                    /* yayaya! I found one that is already implement. I am lucky, right? I should buy a lottery ticket :P */
                    ILHandler.Execute(Config, xOp, method, Optimizer);
                }
            }

            /* Add minor blocks to main block  */
            var ILKeys = ILBlocks.Keys.ToList();
            ILKeys.Sort();

            foreach (var pos in ILKeys)
                block.Body.AddRange(ILBlocks[pos].Body);

            /* revert to original main block */
            Instruction.Block = block;
            EmitFooter(block, method);

            Instruction.Block = null;
        }

        /// <summary>
        /// Emit calli header
        /// </summary>
        /// <param name="block"></param>
        /// <param name="method"></param>
        /// <param name="stackspace"></param>
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
                                    /* this is pretty much standard thing :P */

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

        /// <summary>
        /// Load up execption pointer onto the stack
        /// </summary>
        /// <param name="block"></param>
        /// <param name="method"></param>
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
                                    /* ECX contains pointer to the exception object */
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

        /// <summary>
        /// Emit static method constructor's header
        /// </summary>
        /// <param name="block"></param>
        /// <param name="method"></param>
        internal void EmitConstructor(FunctionalBlock block, MethodBase method)
        {
            if ((method is ConstructorInfo) == false)
                throw new Exception(string.Format("Illegal call to EmitConstructor by '{0}'", method.FullName()));

            /* Constructors should be called once in a life time, so make sure they are not getting called again */
            switch (block.Platform)
            {
                case Architecture.x86:
                    {
                        var key = method.ConstructorKey();
                        InsertData(key, 1);

                        /* Return if it was called before */

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

        /// <summary>
        /// Emit Calli footer
        /// </summary>
        /// <param name="block"></param>
        /// <param name="method"></param>
        internal void EmitFooter(FunctionalBlock block, MethodBase method)
        {
            /* warning: dark magic below */
            var paramsSize = method.GetParameters().Sum(arg => Helper.GetTypeSize(arg.ParameterType, Config.TargetPlatform, true));

            if (!method.IsStatic)
                paramsSize += Helper.GetTypeSize(method.DeclaringType, Config.TargetPlatform, true);

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
                                    /* if process didn't throw any error, then clear ECX register */
                                    new Xor { DestinationReg = Register.ECX, SourceReg = Register.ECX };

                                    new Label(".Error");

                                    /* pop return value and put it in EAX register */
                                    if (returncount != 0)
                                    {
                                        if (returncount > 8)
                                            throw new Exception("Return type > 8 not supported");

                                        new Pop { DestinationReg = Register.EAX };
                                        if (returncount == 8)
                                            new Pop { DestinationReg = Register.EDX };
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

        /// <summary>
        /// try to emit OpCodeType from method IL byte body
        /// </summary>
        /// <param name="method">method function</param>
        /// <param name="ReferencedPositions">branch positions</param>
        /// <returns></returns>
        internal List<OpCodeType> EmitOpCodes(MethodBase method, HashSet<int> ReferencedPositions)
        {
            /* probably this is the darkest magic of all this, and not that MSIL stuff */

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

                xOpCodeType.IsLastIL = xOpCodeType.NextPosition == byteCode.Length;
                EmittedOpCodes.Add(xOpCodeType);
            }

            return EmittedOpCodes;
        }
    }
}
