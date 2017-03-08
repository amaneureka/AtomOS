/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using Atomix.IL;
using Atomix.ILOpCodes;

using Atomix.ILOptimizer;

using Atomix.Assembler;
using Atomix.Assembler.x86;

using Atomix.CompilerExt;
using Atomix.CompilerExt.Attributes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix
{
    public class Compiler
    {
        /// <summary>
        /// The Members which are yet to be build
        /// </summary>
        public Queue<_MemberInfo> QueuedMember;

        /// <summary>
        /// The Members which are already build
        /// </summary>
        private List<_MemberInfo> BuildDefinations;

        /// <summary>
        /// The Dictionary of Build Target method and its label --> Plug
        /// it can also take integer but the Dummy should present which contains that integer
        /// </summary>
        public Dictionary<MethodBase, string> Plugs;

        /// <summary>
        /// It contains all literal string datamember
        /// </summary>
        private Dictionary<string, string> StringTable;

        /// <summary>
        /// The list of implemented Microsoft IL's by our compiler
        /// </summary>
        public Dictionary<ILCode, MSIL> MSIL;

        private List<MethodInfo> Virtuals;
        private bool DoOptimization;
        /// <summary>
        /// Are we building ELF binary?
        /// </summary>
        private bool BuildingApplication;

        public Compiler(bool aDoOptimization)
        {
            ILCompiler.Logger.Write("@Compiler", "Main Compilation Process", "Loading Non-Static Parameters and starting up Compilation Process...");
            QueuedMember = new Queue<_MemberInfo>();
            BuildDefinations = new List<_MemberInfo>();
            Plugs = new Dictionary<MethodBase, string>();
            StringTable = new Dictionary<string, string>();
            Core.vStack = new VirtualStack();
            Core.NasmHeaders = new List<string>();
            Core.DataMember = new List<AsmData>();
            Core.AssemblerCode = new List<Instruction>();
            MSIL = new Dictionary<ILCode, MSIL>();
            Virtuals = new List<MethodInfo>();
            DoOptimization = aDoOptimization;
            Core.StaticLabels = new Dictionary<string, _MemberInfo>();

            ILHelper.Compiler = this;
            ILCompiler.Logger.Write("Parameters Initialized");
        }

        public void Start()
        {
            ILCompiler.Logger.Write("Compiler :: Start()");

            // Load the type of Kernel Start, it depends on CPU Arch. we have choosed.
            var xType = LoadKernel(ILCompiler.CPUArchitecture);

            ILCompiler.Logger.Write(string.Format("{0} Kernel Type Found :: {1}", ILCompiler.CPUArchitecture.ToString(), xType.ToString()));
            // Just find the start method, which is constant label method inside the kernel type
            var xStartup = xType.GetMethod(Helper.StartupMethod, BindingFlags.Static | BindingFlags.Public);

            if (xStartup == null)
            {
                if (BuildingApplication)
                    ILCompiler.Logger.Write("Application has no entry point :: " + Helper.StartupMethod);
                else
                    throw new Exception("No startup found");
            }
            else
                ILCompiler.Logger.Write("Found Startup Method :: " + Helper.StartupMethod);

            // Load our MSIL implementation (basically init it and save into memory)
            LoadMSIL();
            // It just make up the MSIL array, Low and High
            BodyProcesser.Start();
            /*
             * From now onwards we enquque methods, but as you see we do first Enqueue of startup method
             * it is because startup method contains some header code,
             * if it will not be in assembly header than multiboot signature will be disturbed.
             * Just after this we scan plugs and enquque them
             */
            if (xStartup != null)
                QueuedMember.Enqueue(xStartup);

            ScanPlugs();

            /* Compiler Included Code */
            LoadCompilerLibrary();

            ILCompiler.Logger.Write("Enququed Start Method");
            while (QueuedMember.Count > 0)
            {
                // Man we do scarifice to code, first come first process policy is our
                var xMethod = QueuedMember.Dequeue();
                /* Build Only when it is not yet processed mean not in build definations */
                if (!BuildDefinations.Contains(xMethod))
                {
                    // Process MethodBase
                    if (xMethod is MethodBase)
                    {
                        var Method = xMethod as MethodBase;

                        // Check if we want to build it inline assembly
                        if (Method.GetCustomAttribute(typeof(AssemblyAttribute)) != null)
                            ExecutableMethod(Method);
                        else if (Method.GetCustomAttribute(typeof(DllImportAttribute)) != null)
                            ProcessExternalMethod(Method);
                        else
                            ScanMethod(Method);
                    }
                    // Process Type
                    else if (xMethod is Type)
                    {
                        ScanType((Type)xMethod);
                    }
                    // Process FieldInfo
                    else if (xMethod is FieldInfo)
                    {
                        var xField = ((FieldInfo)xMethod);
                        ProcessField(xField);
                    }
                }
            }
        }

        /// <summary>
        /// Loading Microsoft IL's implementations to memory --> I mean list
        /// </summary>
        private void LoadMSIL()
        {
            // Tell logger we are loading msil into memory
            ILCompiler.Logger.Write("Loading MSIL into memory...");
            foreach (Type xType in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var attrib in xType.GetCustomAttributes(typeof(ILOpAttribute)))
                {
                    var Attribute = attrib as ILOpAttribute;
                    if (Attribute != null)
                    {
                        MSIL.Add(Attribute.ILCode, (MSIL)Activator.CreateInstance(xType, this));
                    }
                }
            }
        }

        /// <summary>
        /// Load Kernel startup type in input dll, by scanning Kernel attribute
        /// </summary>
        /// <param name="cpu">The Current target platform</param>
        /// <returns></returns>
        private Type LoadKernel(CPUArch cpu)
        {
            var xAssembly = Assembly.LoadFile(ILCompiler.InputDll);

            /* Search For Kernel Attribute */
            foreach (var xType in xAssembly.GetExportedTypes())
            {
                foreach (var xAttr in xType.CustomAttributes)
                {
                    if (xAttr.AttributeType == typeof(KernelAttribute))
                    {
                        if ((CPUArch)xAttr.ConstructorArguments[0].Value == cpu)
                        {
                            Core.NasmHeaders.Add("global Kernel_Main");
                            Core.NasmHeaders.Add(string.Format("Kernel_Main equ (_Kernel_Main - {0})", xAttr.ConstructorArguments[1].Value));

                            return xType;
                        }
                    }
                    else if (xAttr.AttributeType == typeof(ApplicationAttribute))
                    {
                        if ((CPUArch)xAttr.ConstructorArguments[0].Value == cpu)
                        {
                            BuildingApplication = true;
                            return xType;
                        }
                    }
                }
            }
            throw new Exception("No Input kernel found");
        }

        /// <summary>
        /// This method crawl all assemblies except the compiler and mscorlib
        /// and check for our attributes and add it to attribute list and Enqueue them all =P
        /// </summary>
        private void ScanPlugs()
        {
            var AssembliesLocation = ILCompiler.InputFiles;
            AssembliesLocation.Add(ILCompiler.InputDll);

            foreach (var xFile in AssembliesLocation)
            {
                // Just a basic check if it is not dll than remove it from this list
                // it is basic also because only we dev are going to work on this...and we will work in limit
                // we are not going to hack our own compiler man...so don't worry of this implementations
                if (!xFile.EndsWith(".dll"))
                    AssembliesLocation.Remove(xFile);
            }

            var xAssemblies = new List<Assembly>();
            ILCompiler.Logger.Write("@Compiler", "Reference Scanner", "Scanning References Assemblies");

            foreach (var xLoc in AssembliesLocation)
            {
                // Load the assembly which we want and add it to assemblies list
                var xAss = Assembly.LoadFile(xLoc);
                if (xAss != null)
                {
                    xAssemblies.Add(xAss);
                    //For now i don't want this
                    //It is also because i got error...hehe =P
                    /*
                    foreach (var xRef in xAss.GetReferencedAssemblies())
                    {
                        var xRefAssembly = Assembly.Load(xRef);
                        if (xRef.Name == "mscorlib"
                            || xRef.Name == "CompilerExt"
                            || xRef.Name == "ILCompiler.Assembler")
                            continue;
                        else
                        {
                            xAssemblies.Add(xRefAssembly);
                            ILCompiler.Logger.Write(string.Format("Found Assembly: {0}; {1}", xAss.FullName, xRef.Name));
                        }
                    }*/
                }
            }

            // Now we scan whole source i mean billions of lines kernel code, will be in future so no need to laugh
            ILCompiler.Logger.Write("@Compiler", "Plug Scanner", "Scanning plug Initialized");
            foreach (var xAssembly in xAssemblies)
            {
                // Firstly we scan types for label attribute, well if i tell seriously than it is of no use, but for future implementations
                foreach (var xType in xAssembly.GetTypes())
                {
                    foreach (var xMethod in xType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        // TODO: review the below code
                        foreach (var xAttr in xMethod.CustomAttributes)
                        {
                            if (xAttr.AttributeType == typeof(PlugAttribute))
                            {
                                if ((CPUArch)xAttr.ConstructorArguments[1].Value == ILCompiler.CPUArchitecture)
                                {
                                    var xLbl = (string)xAttr.ConstructorArguments[0].Value;
                                    if (xLbl != "#" && xLbl != "0" && xLbl != string.Empty)
                                        Plugs.Add(xMethod, xLbl);
                                    QueuedMember.Enqueue(xMethod);
                                    ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>PlugAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
                                        xMethod.Name,
                                        xType.FullName,
                                        xAssembly.FullName));
                                }
                            }
                            else if (xAttr.AttributeType == typeof(LabelAttribute))
                            {
                                var xLbl = (string)xAttr.ConstructorArguments[0].Value;
                                if (xLbl != "#" && xLbl != "0" && xLbl != string.Empty)
                                    Core.StaticLabels.Add(xLbl, xMethod);
                                QueuedMember.Enqueue(xMethod);
                                ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>LabelAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
                                        xMethod.Name,
                                        xType.FullName,
                                        xAssembly.FullName));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Process the field and add to datamember
        /// </summary>
        /// <param name="aField">The field to process</param>
        private void ProcessField(FieldInfo aField)
        {
            var xName = aField.FullName();
            int xTheSize = 4;

            Type xFieldTypeDef = aField.FieldType;
            if (!xFieldTypeDef.IsClass || xFieldTypeDef.IsValueType)
                xTheSize = aField.FieldType.SizeOf();

            // We use marshal and read c# assembly memory to get the value of static field
            byte[] xData = new byte[xTheSize];
            try
            {
                object xValue = aField.GetValue(null);
                if (xValue != null)
                {
                    try
                    {
                        xData = new byte[xTheSize];
                        if (xValue.GetType().IsValueType)
                            for (int x = 0; x < xTheSize; x++)
                                xData[x] = Marshal.ReadByte(xValue, x);
                    }
                    catch
                    {
                        // Do nothing, if error...we won't care it
                    }
                }
            }
            catch
            {
                // Do nothing, if error...we won't care it
            }
            // Add it to data member
            Core.InsertData(new AsmData(xName, xData));

            // Add it build definations
            BuildDefinations.Add(aField);
        }

        private void LoadCompilerLibrary()
        {
            var xMethod = typeof(VTable).GetMethod("GetEntry", BindingFlags.Public | BindingFlags.Static);

            Core.StaticLabels.Add(Helper.lblVTable, xMethod);
            QueuedMember.Enqueue(xMethod);
        }

        private void ProcessExternalMethod(MethodBase aMethod)
        {
            ILCompiler.Logger.Write("@Processor", aMethod.FullName(), "Processing External Method");

            if (!aMethod.IsStatic)
                throw new Exception("Non-static extern fields not supported");

            var aAttributeData = (DllImportAttribute)aMethod.GetCustomAttribute(typeof(DllImportAttribute));
            if (aAttributeData == null)
                throw new Exception("[DllImport]: Invalid Build");

            string xLibName = aAttributeData.Value;
            string xMethodLabel = aMethod.FullName();
            string xMethodName = aAttributeData.EntryPoint == null ? aMethod.Name : aAttributeData.EntryPoint;
            int xCalliConvention = (int)aAttributeData.CallingConvention;
            bool IsAtomixCompiled = (xCalliConvention == (int)aMethod.CallingConvention);

            var xParms = aMethod.GetParameters();

            /*
             * For now assume normal calli method
             * - Push Library Name
             * - Push Function Name
             * - Call Kernel API
             * - Jump to function address
             */
            Core.AssemblerCode.Add(new Label(xMethodLabel));

            // standard calli header
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });

            // load and get address of external function
            Core.AssemblerCode.Add(new Push { DestinationRef = AddStringData(xLibName) });
            Core.AssemblerCode.Add(new Push { DestinationRef = AddStringData(xMethodName) });

            Core.AssemblerCode.Add(new Call(Helper.lblImportDll, true));
            Core.AssemblerCode.Add(new Test { DestinationReg = Registers.ECX, SourceRef = "0x2" });

            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });

            int ArgSize = ILHelper.GetArgumentsSize(aMethod);
            int xReturnSize = ILHelper.GetReturnTypeSize(aMethod);
            int xRetSize = (ArgSize - xReturnSize);

            #region _CALLI_HEADER_
            if (IsAtomixCompiled)
            {

                // push the arguments from left to right
                for (int i = 0; i < xParms.Length; i++)
                {
                    int xDisplacement = ILHelper.GetArgumentDisplacement(aMethod, i);
                    int xArgSize = aMethod.GetParameters()[i].ParameterType.SizeOf().Align();

                    for (int j = 0; j < (xArgSize / 4); j++)
                    {
                        Core.AssemblerCode.Add(
                            new Push
                            {
                                DestinationReg = Registers.EBP,
                                DestinationIndirect = true,
                                DestinationDisplacement = xDisplacement - (j * 4)
                            });
                    }
                }

                if (xRetSize < 0)
                {
                    Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x" + (-xRetSize).ToString("x") });
                    xRetSize = 0;
                }
            }
            else if (xCalliConvention == (int)CallingConvention.StdCall)
            {
                // push arguments to the stack from right to left
                for (int i = xParms.Length - 1; i >= 0; i--)
                {
                    int xDisplacement = ILHelper.GetArgumentDisplacement(aMethod, i);
                    int xArgSize = aMethod.GetParameters()[i].ParameterType.SizeOf().Align();

                    for (int j = 0; j < (xArgSize / 4); j++)
                    {
                        Core.AssemblerCode.Add(
                            new Push
                            {
                                DestinationReg = Registers.EBP,
                                DestinationIndirect = true,
                                DestinationDisplacement = xDisplacement - (j * 4)
                            });
                    }
                }
            }
            #endregion

            // Call the function
            Core.AssemblerCode.Add(new Call("EAX"));

            #region _CALLI_FOOTER_
            if (xReturnSize > 0)
            {
                var xOffset = ILHelper.GetResultCodeOffset(xReturnSize, ArgSize);
                if (IsAtomixCompiled)
                {
                    // For return type Method
                    for (int i = 0; i < xReturnSize / 4; i++)
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Mov
                        {
                            DestinationReg = Registers.EBP,
                            DestinationIndirect = true,
                            DestinationDisplacement = (xOffset + (i * 4)),
                            SourceReg = Registers.EAX
                        });
                    }
                }
                else if (xCalliConvention == (int)CallingConvention.StdCall)
                {
                    if (xReturnSize > 8)
                        throw new Exception("[Extern]: Unsupported Return Size for StdCall");

                    Core.AssemblerCode.Add(new Mov
                    {
                        DestinationReg = Registers.EBP,
                        DestinationIndirect = true,
                        DestinationDisplacement = xOffset,
                        SourceReg = Registers.EAX
                    });

                    if (xReturnSize == 8)
                    {
                        Core.AssemblerCode.Add(new Mov
                        {
                            DestinationReg = Registers.EBP,
                            DestinationIndirect = true,
                            DestinationDisplacement = (xOffset + 4),
                            SourceReg = Registers.EAX
                        });
                    }
                }
            }
            #endregion

            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });
            Core.AssemblerCode.Add(new Leave());
            Core.AssemblerCode.Add(new Ret { Address = (byte)xRetSize });

            BuildDefinations.Add(aMethod);
        }

        static int CurrentLabel = 0;
        public string AddStringData(string aStr)
        {
            if (StringTable.ContainsKey(aStr))
                return StringTable[aStr];

            var xLabel = "StringContent_" + CurrentLabel.ToString().PadLeft(4, '0');
            StringTable.Add(aStr, xLabel);
            CurrentLabel++;

            return xLabel;
        }

        public void FlushStringDataTable()
        {
            int CurrentLabel = 0;
            string Content, str;
            foreach(var xObj in StringTable)
            {
                str = xObj.Key;
                Content = xObj.Value;
                Encoding xEncoding = Encoding.Unicode;
                var xBytecount = xEncoding.GetByteCount(str);
                var xObjectData = new byte[(xBytecount) + 0x10]; // 0xC is object data offset

                Array.Copy(BitConverter.GetBytes(ILHelper.GetTypeID(typeof(string))), 0, xObjectData, 0, 4);
                Array.Copy(BitConverter.GetBytes(0x1), 0, xObjectData, 4, 4);
                Array.Copy(BitConverter.GetBytes(xObjectData.Length), 0, xObjectData, 8, 4);
                Array.Copy(BitConverter.GetBytes(str.Length), 0, xObjectData, 12, 4);
                Array.Copy(xEncoding.GetBytes(str), 0, xObjectData, 16, xBytecount);

                Core.DataMember.Add(new AsmData(Content, xObjectData));
                CurrentLabel++;
            }
        }

        /// <summary>
        /// Build the Inline assembly method, how? quite simple just invoke that method
        /// So, remember we want use parameters of an assembly method via c#, we have to be completely literal
        /// </summary>
        /// <param name="aMethod"></param>
        private void ExecutableMethod(MethodBase aMethod)
        {
            ILCompiler.Logger.Write("@Processor", aMethod.FullName(), "Scanning Inline Assembly Method()");
            string xMethodLabel = aMethod.FullName();

            TryToMakeGlobalSymbol(aMethod);

            Core.AssemblerCode.Add(new Label(xMethodLabel));

            var Attribute = (AssemblyAttribute)aMethod.GetCustomAttribute(typeof(AssemblyAttribute));
            var NeedCalliHeader = Attribute.NeedCalliHeader;

            /* Well we check if we assign any return size than it means we are lazy
             * and we want compiler to add calli and return code.
             */
            if (NeedCalliHeader)
            {
                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });
            }

            /* Do Invoke here */
            aMethod.Invoke(null, new object[aMethod.GetParameters().Length]);
            ILCompiler.Logger.Write("Method Successfully Invoked()");

            if (NeedCalliHeader)
            {
                byte RetSize = (byte)Math.Max(0, ILHelper.GetArgumentsSize(aMethod) - ILHelper.GetReturnTypeSize(aMethod));

                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });
                Core.AssemblerCode.Add(new Leave());
                Core.AssemblerCode.Add(new Ret { Address = (byte)RetSize });
            }

            BuildDefinations.Add(aMethod);
            ILCompiler.Logger.Write("Method Build Done()");
        }

        private static string[] RestrictedAssembliesName =
            {
                "mscorlib",
                "System",
                Assembly.GetExecutingAssembly().GetName().Name
            };

        /// <summary>
        /// Scan the normal method and find inline calls of virtual or real method to add it into queue
        /// After that Process method and add its assembly to main Array
        /// </summary>
        /// <param name="aMethod">The normal mathod =)</param>
        private void ScanMethod(MethodBase aMethod)
        {
            // Just a basic patch to fix Null reference exception
            var xBody = aMethod.GetMethodBody();

            var xAssemblyName  = aMethod.DeclaringType.Assembly.GetName().Name;
            if (RestrictedAssembliesName.Contains(xAssemblyName) && Plugs.ContainsValue(aMethod.FullName()))
                return;

            if (ILHelper.IsDelegate(aMethod.DeclaringType))
            {
                ProcessDelegate(aMethod);
                return;
            }

            if (xBody == null) // Don't try to build null method
                return;

            // Tell logger that we are processing a method with given name and declaring type
            // Well declaring type is necessary because when we have a plugged method than
            // Its full name is the plugged value so we will not get where the plug is implemented
            // So declaring type help us to determine that
            ILCompiler.Logger.Write("@Processor", aMethod.FullName(), "Scanning Method()");
            ILCompiler.Logger.Write("Type: " + aMethod.DeclaringType.FullName);
            ILCompiler.Logger.Write("Assembly: " + xAssemblyName);

            /* Scan Method for inline virtual or real methods call */
            var xParams = aMethod.GetParameters();
            var xParamTypes = new Type[xParams.Length];

            for (int i = 0; i < xParams.Length; i++)
            {
                xParamTypes[i] = xParams[i].ParameterType;
                QueuedMember.Enqueue(xParamTypes[i]);
            }

            bool DynamicMethod = (aMethod.DeclaringType == null);

            if (!DynamicMethod)
                QueuedMember.Enqueue(aMethod.DeclaringType);

            if (aMethod is MethodInfo)
                QueuedMember.Enqueue(((MethodInfo)aMethod).ReturnType);

            if (!DynamicMethod && aMethod.IsVirtual)
            {
                #warning Need to implement currently
            }

            /* Process method and get out its IL's array */
            // @Newlabels:   New labels is the branches where we have to assign method
            //               a label for branch/call IL's operations to perform.
            //               we make its array while making the IL's array so its make our task easier
            List<int> NewLabels = new List<int>();
            var OpCodes = aMethod.Process(NewLabels);

            // Here seems to be going something wrong
            ILCompiler.Logger.Write ("MSIL Codes Loaded... Count::" + OpCodes.Count);

            string xMethodLabel = aMethod.FullName();

            /* Method begin */
            Core.AssemblerCode.Add(new Label(xMethodLabel));

            TryToMakeGlobalSymbol(aMethod);

            Core.AssemblerCode.Add(new Comment(Worker.OPTIMIZATION_START_FLAG));
            if (aMethod.IsStatic && aMethod is ConstructorInfo)
            {
                string aData = "cctor_" + xMethodLabel;
                Core.InsertData(new AsmData(aData, new byte[] { 0x00 }));
                Core.AssemblerCode.Add(new Cmp() { DestinationRef = aData, DestinationIndirect = true, SourceRef = "0x0", Size = 8 });
                Core.AssemblerCode.Add(new Jmp() { Condition = ConditionalJumpEnum.JE, DestinationRef = Label.PrimaryLabel + ".Load" });
                /* Footer of method */
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });
                Core.AssemblerCode.Add(new Ret());

                Core.AssemblerCode.Add(new Label(".Load"));
                Core.AssemblerCode.Add(new Mov() { DestinationRef = aData, DestinationIndirect = true, SourceRef = "0x1", Size = 8 });
            }
            /* Calli instructions */
            Core.AssemblerCode.Add(new Push() { DestinationReg = Registers.EBP });
            Core.AssemblerCode.Add(new Mov() { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });

            /* Calculate Method Variables Size */
            int Size = (from item in xBody.LocalVariables
                        select item.LocalType.SizeOf().Align()).Sum();

            if (Size > 0)
            {
                // Make a space for local variables
                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x" + Size.Align().ToString("X") });
            }

            ILCompiler.Logger.Write("Listening OpCodes");

            /* Exceute IL Codes */
            foreach (var Op in OpCodes)
            {
                // Check if we need exception to push?
                var xNeedsExceptionPush = (Op.Ehc != null) && (((Op.Ehc.HandlerOffset > 0 && Op.Ehc.HandlerOffset == Op.Position) || ((Op.Ehc.Flags & ExceptionHandlingClauseOptions.Filter) > 0 && Op.Ehc.FilterOffset > 0 && Op.Ehc.FilterOffset == Op.Position)) && (Op.Ehc.Flags == ExceptionHandlingClauseOptions.Clause));

                ILCompiler.Logger.Write(Op.ToString() + "; Stack Count => " + Core.vStack.Count);
                // Check if current position inside the list of label list, if yes than break label and make a new one
                if (NewLabels.Contains(Op.Position))
                {
                    var xLbl = new Label(ILHelper.GetLabel(aMethod, Op.Position));
                    if (!Core.AssemblerCode.Contains(xLbl))
                        Core.AssemblerCode.Add(xLbl);
                }

                // If catch IL here than push current error so, that catch IL pop and do what it want
                if (xNeedsExceptionPush)
                {
                    Core.AssemblerCode.Add(new Push { DestinationRef = "0x0" });
                    Core.AssemblerCode.Add(new Call (Helper.lblGetException, true));
                    Core.vStack.Push(4, typeof(Exception));
                }

                // Well this is just to comment whole output Assembly
                if (!DoOptimization)
                    Core.AssemblerCode.Add(new Comment(Op.ToString() + "; " + Core.vStack.Count));

                // Check if this IL is in out implementation
                if (MSIL.ContainsKey(Op.Code))
                {
                    // If yes than execute it
                    MSIL[Op.Code].Execute(Op, aMethod);
                }
                else
                    // If it is not implementation than throw error while compilation
                    throw new Exception(Op.ToString() + "; " + xMethodLabel);
                #region Queue Inline calls
                if (Op is OpMethod)
                {
                    QueuedMember.Enqueue(((OpMethod)Op).Value);
                }
                else if (Op is OpType)
                {
                    QueuedMember.Enqueue(((OpType)Op).Value);
                }
                else if (Op is OpField)
                {
                    QueuedMember.Enqueue(((OpField)Op).Value.DeclaringType);
                    if (((OpField)Op).Value.IsStatic)
                    {
                        QueuedMember.Enqueue(((OpField)Op).Value);
                    }
                }
                else if (Op is OpToken)
                {
                    var x = ((OpToken)Op);
                    if (x.ValueIsType)
                    {
                        QueuedMember.Enqueue(x.ValueType);
                    }

                    if (x.ValueIsField)
                    {
                        QueuedMember.Enqueue(x.ValueField.DeclaringType);
                        if (x.ValueField.IsStatic)
                        {
                            QueuedMember.Enqueue(x.ValueField);
                        }
                    }
                }
                #endregion
            }
            // End the method and return method, without exception
            Core.AssemblerCode.Add(new Label(xMethodLabel + ".End"));
            // We assume that if the ecx is 0x0 then the method is done without any exception
            // it can be assumed by test instruction followed by conditional jump while calling a function
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });

            Core.AssemblerCode.Add(new Label(xMethodLabel + ".Error"));
            // Now below code save the return value to EBP varaible
            // And calculate size
            int ArgSize = ILHelper.GetArgumentsSize(aMethod);

            int ReturnSize = ILHelper.GetReturnTypeSize(aMethod);
            if (ReturnSize > 0)
            {
                // For return type Method
                var xOffset = ILHelper.GetResultCodeOffset(ReturnSize, ArgSize);
                for (int i = 0; i < ReturnSize / 4; i++)
                {
                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                    Core.AssemblerCode.Add(new Mov
                    {
                        DestinationReg = Registers.EBP,
                        DestinationIndirect = true,
                        DestinationDisplacement = (int)(xOffset + ((i + 0) * 4)),
                        SourceReg = Registers.EAX
                    });
                }
            }
            Core.AssemblerCode.Add(new Comment(Worker.OPTIMIAZTION_END_FLAG));

            byte RetSize = (byte)Math.Max(0, ILHelper.GetArgumentsSize(aMethod) - ILHelper.GetReturnTypeSize(aMethod));

            // Leave this method mean regain original EBP and ESP offset
            Core.AssemblerCode.Add(new Leave());
            // Return to parent method with given stack offset
            Core.AssemblerCode.Add(new Ret { Address = RetSize });

            // Add this method to build definations so we will not build it again
            BuildDefinations.Add(aMethod);

            // And log it
            ILCompiler.Logger.Write("Method Build Done()");
        }
        /// <summary>
        /// We just scan type for to Queued methods
        /// </summary>
        /// <param name="aType">The type need to scan</param>
        private void ScanType(Type aType)
        {
            if (aType.BaseType != null)
                QueuedMember.Enqueue(aType.BaseType);

            foreach (var xCctor in aType.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (xCctor.DeclaringType == aType)
                    QueuedMember.Enqueue(xCctor);
            }

            foreach (var xAB in aType.GetMethods())
            {
                /* For abstract methods conditions are :-
                 * 1) Its base defination type is not its declaring type
                 * 2) Its base defination should be Abstract
                 * 3) Its declaring type should not be abstract
                 * 4) Method Body should not be null
                 */
                if (xAB.GetBaseDefinition().DeclaringType != xAB.DeclaringType &&
                    xAB.GetBaseDefinition().IsAbstract &&
                    !xAB.DeclaringType.IsAbstract)
                {
                    QueuedMember.Enqueue(xAB);
                    Virtuals.Add(xAB);
                }
            }
            BuildDefinations.Add(aType);
        }

        private void TryToMakeGlobalSymbol(MethodBase aMethod)
        {
            var xAssemblyName = aMethod.DeclaringType.Assembly.GetName().Name;
            if (aMethod.IsPublic &&
                aMethod.DeclaringType.IsVisible &&
                !RestrictedAssembliesName.Contains(xAssemblyName))
            {
                Core.NasmHeaders.Add("global " + aMethod.FullName());
            }
        }

        private void VTableFlush()
        {
            uint xUID = 0;

            var xDict = new Dictionary<int, List<MethodInfo>>();
            foreach (var xV in Virtuals)
            {
                if (!xV.GetBaseDefinition().IsAbstract) continue;

                var xTypeID = ILHelper.GetTypeID(xV.DeclaringType);
                if (!xDict.ContainsKey(xTypeID))
                    xDict[xTypeID] = new List<MethodInfo>();
                xDict[xTypeID].Add(xV);
            }

            /*
             * Array of Entries
             *      - Size of entry in DWORD
             *      - TypeID
             *      - ARRAY OF {METHOD_ID, LABEL}
             *      - Zero at end
             */

            var xVTableData = new List<string>();
            foreach(var xItem in xDict)
            {
                int xTypeID = xItem.Key;
                int xSize = 3 + (xItem.Value.Count * 2);
                xVTableData.Add(xSize.ToString());
                xVTableData.Add(xTypeID.ToString());

                foreach(var yItem in xItem.Value)
                {
                    string xLabel = yItem.FullName();
                    xUID = (uint)((MethodBase)yItem.GetBaseDefinition()).GetHashCode();
                    xVTableData.Add(xUID.ToString());
                    xVTableData.Add(xLabel);
                }
                xVTableData.Add("0");
            }
            xVTableData.Add("0");
            Core.DataMember.Add(new AsmData("__VTable_Flush__", xVTableData.ToArray()));
        }

        private void ProcessDelegate(MethodBase xMethod)
        {
            var lbl = xMethod.FullName();
            var lbl_exception = lbl + ".Error";
            var lbl_end = lbl + ".End";

            Core.AssemblerCode.Add(new Label(lbl));

            // Calli header
            Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });

            if (lbl.Contains("ctor"))
            {
                // Load Argument
                ((Ldarg)MSIL[ILCode.Ldarg]).Execute2(0, xMethod);
                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EBX });

                ((Ldarg)MSIL[ILCode.Ldarg]).Execute2(2, xMethod); // The pointer
                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EBX, SourceRef = "0xC" });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, SourceReg = Registers.EAX });

                ((Ldarg)MSIL[ILCode.Ldarg]).Execute2(1, xMethod); // The Object
                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EBX, SourceRef = "0x4" });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBX, DestinationIndirect = true, SourceReg = Registers.EAX });

                // calli footer
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });
                Core.AssemblerCode.Add(new Leave());
                Core.AssemblerCode.Add(new Ret { Address = 0xC }); // Memory location + Object + Intptr
            }
            else if (lbl.Contains("Invoke"))
            {
                // Load Reference
                ((Ldarg)MSIL[ILCode.Ldarg]).Execute2(0, xMethod);

                Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0xC" });

                // If it is a non static field than get its parent Type memory location (object in constructor)
                if (!xMethod.IsStatic)
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EAX, DestinationDisplacement = 0x4, DestinationIndirect = true });

                var xParms = xMethod.GetParameters();
                int xSize = ILHelper.GetArgumentsSize(xMethod);

                // Load arguments to throw
                int xArgSize;
                for (ushort i = 1; i <= xParms.Length; i++)
                {
                    // New Code
                    int xDisplacement = ILHelper.GetArgumentDisplacement(xMethod, i);

                    if (xMethod.IsStatic)
                        xArgSize = xMethod.GetParameters()[i].ParameterType.SizeOf().Align();
                    else
                        xArgSize = xMethod.GetParameters()[i - 1].ParameterType.SizeOf().Align();

                    for (int j = 0; j < (xArgSize / 4); j++)
                    {
                        Core.AssemblerCode.Add(
                            new Push
                            {
                                DestinationReg = Registers.EBP,
                                DestinationIndirect = true,
                                DestinationDisplacement = xDisplacement - (j * 4)
                            });
                    }
                }

                // Call the function
                Core.AssemblerCode.Add(new Call("[EAX]"));

                // Check for anytype of exception
                Core.AssemblerCode.Add(new Test { DestinationReg = Registers.ECX, SourceRef = "0x2" });
                Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = lbl_exception });

                // calli footer
                Core.AssemblerCode.Add(new Label(lbl_end));
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });

                Core.AssemblerCode.Add(new Label(lbl_exception));
                int xReturnSize = ILHelper.GetReturnTypeSize(xMethod);

                if (xReturnSize > 0)
                {
                    // For return type Method
                    var xOffset = ILHelper.GetResultCodeOffset(xReturnSize, xSize);
                    for (int i = 0; i < xReturnSize / 4; i++)
                    {
                        Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                        Core.AssemblerCode.Add(new Mov
                        {
                            DestinationReg = Registers.EBP,
                            DestinationIndirect = true,
                            DestinationDisplacement = (int)(xOffset + ((i + 0) * 4)),
                            SourceReg = Registers.EAX
                        });
                    }
                }

                byte xRetSize = (byte)Math.Max(0, xSize - xReturnSize);

                Core.AssemblerCode.Add(new Leave());
                Core.AssemblerCode.Add(new Ret { Address = xRetSize }); // Parameter + Memory
            }

            BuildDefinations.Add(xMethod);
        }

        private static void BssSectionHeader(StreamWriter aSW)
        {
            aSW.WriteLine();
            aSW.WriteLine("section .bss");
        }

        private static void DataSectionHeader(StreamWriter aSW)
        {
            aSW.WriteLine();
            aSW.WriteLine("section .data");
        }

        private static void TextSectionHeader(StreamWriter aSW)
        {
            aSW.WriteLine();
            aSW.WriteLine("section .text");
        }

        public void FlushAsmFile()
        {
            // Flush String Data table
            FlushStringDataTable();

            VTableFlush();

            // Add a label of Kernel End, it is used by our heap to know from where it starts allocating memory
            Core.AssemblerCode.Add(new Label("Compiler_End"));

            using (var xWriter = new StreamWriter(ILCompiler.OutputFile, false))
            {
                foreach (var Header in Core.NasmHeaders)
                    xWriter.WriteLine(Header);

                // if any BSS data is contained or not
                if (Core.DataMemberBssSegmentIndex > 0)
                    BssSectionHeader(xWriter);

                // Firstly add datamember
                bool IsFlagged = true;
                foreach (var AsmData in Core.DataMember)
                {
                    if (AsmData.IsBssData == false && IsFlagged == true)
                    {
                        IsFlagged = false;
                        DataSectionHeader(xWriter);
                    }
                    AsmData.FlushText(xWriter);
                }

                TextSectionHeader(xWriter);

                if (DoOptimization)
                {
                    //Try to execute optimizer
                    try { Core.AssemblerCode = new Worker(Core.AssemblerCode).Start(); }
                    catch (Exception e) { Console.WriteLine("Optimization-Exception:" + e.ToString()); }
                }

                foreach (var instruction in Core.AssemblerCode)
                {
                    if (instruction is Label)
                    {
                        xWriter.WriteLine();
                        instruction.FlushText(xWriter);
                    }
                    else
                    {
                        xWriter.Write("     ");
                        if (instruction is Call)
                        {
                            var InstCall = instruction as Call;
                            if (InstCall.FunctionLabel)
                            {
                                Call.FlushText(xWriter, (Core.StaticLabels[InstCall.Address] as MethodBase).FullName());
                                continue;
                            }
                        }
                        instruction.FlushText(xWriter);
                    }
                }
            }
        }
    }
}
