using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.ILOpCodes;
using Atomix.CompilerExt;
using Atomix.Assembler.x86;
using Atomix.CompilerExt.Attributes;
using System.Runtime.InteropServices;
using Core = Atomix.Assembler.AssemblyHelper;
using System.Runtime.Serialization.Formatters.Binary;

namespace Atomix
{
    public class Compiler
    {
        /// <summary>
        /// The Members which yet to be build
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
        /// Dummy can be assumed as a doll method which we are not going to build
        /// The uint decide is a plug based building it build the plug containing given integer as string --> Plug
        /// If it is zero than it won't build anything.
        /// </summary>
        public Dictionary<uint, MethodBase> Dummys;
        /// <summary>
        /// It is the dictionary of all Assembly methods (which has Assembly Attribute)
        /// The integer decide its return value size, if it is 0xFF than it will not add footer and header of method
        /// by the compiler itself else it will default header (calli instructions) 
        /// and footer (leave, ret) with given return value
        /// </summary>
        private Dictionary<MethodBase, uint> AssemblyNative;
        /// <summary>
        /// The list of implemented Microsoft IL's by our compiler
        /// </summary>
        public Dictionary<ILCode, MSIL> MSIL;

        private Dictionary<FieldInfo, string> Pointers;

        public Compiler()
        {
            ILCompiler.Logger.Write("@Compiler", "Main Compilation Process", "Loading Non-Static Parameters and starting up Compilation Process...");
            QueuedMember = new Queue<_MemberInfo>();            
            BuildDefinations = new List<_MemberInfo>();
            Plugs = new Dictionary<MethodBase, string>();
            Dummys = new Dictionary<uint, MethodBase>();
            AssemblyNative = new Dictionary<MethodBase, uint>();
            Core.vStack = new VirtualStack();
            Core.DataMember = new List<AsmData>();
            Core.AssemblerCode = new List<Instruction>();
            MSIL = new Dictionary<ILCode, MSIL>();
            Pointers = new Dictionary<FieldInfo, string>();
            Core.StaticLabels = new Dictionary<string, _MemberInfo>();

            ILHelper.Compiler = this;
            ILCompiler.Logger.Write("Parameters Initialized");
        }

        public void Start()
        {
            ILCompiler.Logger.Write("Compiler :: Start()");
            
            //Load the type of Kernel Start, it depends on CPU Arch. we have choosed.
            var xType = LoadKernel(ILCompiler.CPUArchitecture);
            
            ILCompiler.Logger.Write(string.Format("{0} Kernel Type Found :: {1}", ILCompiler.CPUArchitecture.ToString(), xType.ToString()));
            //Just find the start method, which is constant label method inside the kernel type
            var xStartup = xType.GetMethod(Helper.StartupMethod);
            
            if (xStartup == null)
                throw new Exception("No startup found");

            ILCompiler.Logger.Write("Found Startup Method :: " + Helper.StartupMethod);

            //Load our MSIL implementation (basically init it and save into memory)
            LoadMSIL();
            //It just make up the MSIL array, Low and High
            BodyProcesser.Start();
            /*
             * From now onwards we enquque methods, but as you see we do first Enqueue of startup method
             * it is because startup method contains some header code, 
             * if it will not be in assembly header than multiboot signature will be disturbed.
             * Just after this we scan plugs and enquque them
             */
            QueuedMember.Enqueue(xStartup);
            ScanPlugs();

            ILCompiler.Logger.Write("Enququed Start Method");            
            while (QueuedMember.Count > 0)
            {
                //Man we do scarifice to code, first come first process policy is our
                var xMethod = QueuedMember.Dequeue();
                /* Build Only when it is not yet processed mean not in build definations */
                if (!BuildDefinations.Contains(xMethod))
                {
                    //Process MethodBase
                    if (xMethod is MethodBase)
                    {
                        var Method = xMethod as MethodBase;
                        /* Well what is dummy, a good question, 
                         * exactly it is used when we want to fool VS compiler, because we are smarter than bill gates hehe =P (will be not now)
                         * And Replace a method with something else, not like plug...and also if we want not to build something =P                         * 
                         */
                        if (!Dummys.ContainsValue(Method))
                        {
                            //Check if we want to build it inline assembly
                            if (AssemblyNative.ContainsKey(Method))
                                InlineMethod(Method);
                            else
                                ScanMethod(Method);
                        }
                    }
                    //Process Type
                    else if (xMethod is Type)
                    {
                        ScanType((Type)xMethod);
                    }
                    //Process FieldInfo
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
            //Tell logger we are loading msil into memory
            ILCompiler.Logger.Write("Loading MSIL into memory...");
            foreach (Type xType in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var xAttr in xType.CustomAttributes)
                {
                    //Check if the types in current assembly contains ILOp Attribute is yes,
                    //Than it is an ILOp so add it to list
                    if (xAttr.AttributeType == typeof(ILOpAttribute))
                    {
                        MSIL.Add((ILCode)xAttr.ConstructorArguments[0].Value, (MSIL)Activator.CreateInstance(xType, this));
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
                            switch (cpu)
                            {
                                case CPUArch.x86:
                                    {
                                        Core.DataMember.Add(new AsmData("use32", string.Empty));
                                        Core.DataMember.Add(new AsmData("org", xAttr.ConstructorArguments[1].Value as string));
                                    }
                                    break;
                                //Need it to implement for other platforms too
                            }                            
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
                //Just a basic check if it is not dll than remove it from this list
                //it is basic also because only we dev are going to work on this...and we will work in limit
                //we are not going to hack our own compiler man...so don't worry of this implementations
                if (!xFile.EndsWith(".dll"))
                    AssembliesLocation.Remove(xFile);
            }

            var xAssemblies = new List<Assembly>();
            ILCompiler.Logger.Write("@Compiler", "Reference Scanner", "Scanning References Assemblies");
            //This Dictionary just take the list of all the fieldInfo in current type
            //This is used for making pointers list
            Dictionary<string, FieldInfo> xFields = null;
            foreach (var xLoc in AssembliesLocation)
            {
                //Load the assembly which we want and add it to assemblies list
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

            //Now we scan whole source i mean billions of lines kernel code, will be in future so no need to laugh
            ILCompiler.Logger.Write("@Compiler", "Plug Scanner", "Scanning plug Initialized");
            foreach (var xAssembly in xAssemblies)
            {
                //Firstly we scan types for label attribute, well if i tell seriously than it is of no use, but for future implementations 
                foreach (var xType in xAssembly.GetTypes())
                {
                    foreach (var xAttr in xType.CustomAttributes)
                    {
                        if (xAttr.AttributeType == typeof(LabelAttribute))
                        {
                            Core.StaticLabels.Add((string)xAttr.ConstructorArguments[0].Value, xType);                            
                            QueuedMember.Enqueue(xType);
                            ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>LabelAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
                                        xType.Name,
                                        xType.FullName,
                                        xAssembly.FullName));
                        }
                    }
                    /*This is what we called real magic, well this i have done for finding pointer to any c# function
                     * How it works? So it just scan if there is any static 
                     * field inside a type which starts with p and of same name that of a function inside same type
                     * I know bit crazy but really cool, than it just do a thing in field info implementation 
                     * and save the function pointer to that field info, just a hack as the varaible must be a 4 byte long =P
                     * For example the pointer address of a function named "test" will be saved in "ptest" =P
                     */
                    xFields = new Dictionary<string, FieldInfo>();
                    foreach (var xField in xType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        //Add each field to list
                        xFields.Add(xField.Name, xField);
                        foreach (var xAttr in xField.CustomAttributes)
                        {
                            if (xAttr.AttributeType == typeof(LabelAttribute))
                            {
                                Core.StaticLabels.Add((string)xAttr.ConstructorArguments[0].Value, xField);
                                QueuedMember.Enqueue(xField);
                                ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>LabelAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
                                        xField.Name,
                                        xField.Name,
                                        xAssembly.FullName));
                            }
                        }
                    }
                    foreach (var xMethod in xType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                    {
                        //Now we check is same name is present for any void, if yes than we save pointer else not
                        string pointer_lbl = "p" + xMethod.Name;

                        if (xFields.ContainsKey(pointer_lbl))
                        {
                            //Add the vaild entry to list, rest we don't care 
                            Pointers.Add(xFields[pointer_lbl], xMethod.FullName());
                            //Well these pointers are used so we make sure the method is also build so we Enqueue it
                            QueuedMember.Enqueue(xMethod);
                        }
                        //Now we are going for method attributes and Enqueue them all =P
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
                            else if (xAttr.AttributeType == typeof(AssemblyAttribute))
                            {
                                AssemblyNative.Add(xMethod, (uint)xAttr.ConstructorArguments[0].Value);
                                QueuedMember.Enqueue(xMethod);
                                ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>AssemblyAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
                                        xMethod.Name,
                                        xType.FullName,
                                        xAssembly.FullName));
                            }
                            else if (xAttr.AttributeType == typeof(DummyAttribute))
                            {
                                Dummys.Add((uint)xAttr.ConstructorArguments[0].Value, xMethod);
                                ILCompiler.Logger.Write(string.Format(
                                        "<b>Plug Found <u>DummyAttribute</u></b> :: {0}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{1}<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=>{2}",
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
            //If this is in the pointer list, mean we just add the pointer instead of empty bytes
            if (Pointers.ContainsKey(aField))
            {
                //As simple as that
                Core.DataMember.Add(new AsmData(xName, "dd " + Pointers[aField]));
            }
            else
            {                
                int xTheSize = 4;

                Type xFieldTypeDef = aField.FieldType;
                if (!xFieldTypeDef.IsClass || xFieldTypeDef.IsValueType)
                    xTheSize = aField.FieldType.SizeOf();

                //We use marshal and read c# assembly memory to get the value of static field
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
                            //Do nothing, if error...we won't care it
                        }
                    }
                }
                catch
                {
                    //Do nothing, if error...we won't care it
                }
                //Add it to data member
                Core.DataMember.Add(new AsmData(xName, xData));
            }
            //Add it build definations
            BuildDefinations.Add(aField);
        }

        /// <summary>
        /// Build the Inline assembly method, how? quite simple just invoke that method
        /// So, remember we want use parameters of an assembly method via c#, we have to be completely literal        
        /// </summary>
        /// <param name="aMethod"></param>
        private void InlineMethod(MethodBase aMethod)
        {
            ILCompiler.Logger.Write("@Processor", aMethod.FullName(), "Scanning Inline Assembly Method()");
            string xMethodLabel = aMethod.FullName();
            Core.AssemblerCode.Add(new Label(xMethodLabel));

            var xReturn = AssemblyNative[aMethod];

            /* Well we check if we assign any return size than it means we are lazy 
             * and we want compiler to add calli and return code.
             */
            if (xReturn != 0xFF)
            {
                Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EBP });
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });
            }

            /* Do Invoke here */
            aMethod.Invoke(null, new object[aMethod.GetParameters().Length]);
            ILCompiler.Logger.Write("Method Successfully Invoked()");

            if (xReturn != 0xFF)
            {
                Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });
                Core.AssemblerCode.Add(new Leave());
                Core.AssemblerCode.Add(new Ret { Address = (byte)xReturn });
            }

            BuildDefinations.Add(aMethod);
            ILCompiler.Logger.Write("Method Build Done()");
        }

        /// <summary>
        /// Scan the normal method and find inline calls of virtual or real method to add it into queue
        /// After that Process method and add its assembly to main Array
        /// </summary>
        /// <param name="aMethod">The normal mathod =)</param>
        private void ScanMethod(MethodBase aMethod)
        {
            //Just a basic patch to fix Null reference exception
            var xAssemblyName  = aMethod.DeclaringType.Assembly.GetName().Name;
            if ((xAssemblyName == "mscorlib" || xAssemblyName == "System") && Plugs.ContainsValue(aMethod.FullName()))
                return;
            
            //Tell logger that we are processing a method with given name and declaring type
            //Well declaring type is necessary because when we have a plugged method than
            //Its full name is the plugged value so we will not get where the plug is implemented
            //So declaring type help us to determine that
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
            //@Newlabels:   New labels is the branches where we have to assign method
            //              a label for branch/call IL's operations to perform. 
            //              we make its array while making the IL's array so its make our task easier 
            List<int> NewLabels = new List<int>();
            var OpCodes = aMethod.Process(NewLabels);
            ILCompiler.Logger.Write("MSIL Codes Loaded... Count::" + OpCodes.Count);

            string xMethodLabel = aMethod.FullName();
            var xBody = aMethod.GetMethodBody();
            
            /* Method begin */            
            Core.AssemblerCode.Add(new Label(xMethodLabel));
            #warning Optimization
            //Core.AssemblerCode.Add(new Comment("Do_optimization"));
            if (aMethod.IsStatic && aMethod is ConstructorInfo)
            {
                string aData = "cctor_" + xMethodLabel;
                Core.DataMember.Add(new AsmData(aData, new byte[] { 0x00 }));
                Core.AssemblerCode.Add(new Cmp() { DestinationRef = aData, DestinationIndirect = true, SourceRef = "0x0", Size = 8 });
                Core.AssemblerCode.Add(new Jmp() { Condition = ConditionalJumpEnum.JE, DestinationRef = Label.PrimaryLabel + ".Load" });
                /* Footer of method */
                Core.AssemblerCode.Add(new Ret());

                Core.AssemblerCode.Add(new Label(".Load"));
                Core.AssemblerCode.Add(new Mov() { DestinationRef = aData, DestinationIndirect = true, SourceRef = "0x1", Size = 8 });
            }
            /* Calli instructions */
            Core.AssemblerCode.Add(new Push() { DestinationReg = Registers.EBP });
            Core.AssemblerCode.Add(new Mov() { DestinationReg = Registers.EBP, SourceReg = Registers.ESP });

            /* Calculate Method Variables Size */
            int Size = (from item in xBody.LocalVariables
                        select (int)item.LocalType.SizeOf().Align()).Sum();

            if (Size > 0)
            {
                //Make a space for local variables
                Core.AssemblerCode.Add(new Sub { DestinationReg = Registers.ESP, SourceRef = "0x" + Size.Align().ToString("X") });
            }

            ILCompiler.Logger.Write("Listening OpCodes");

            /* Exceute IL Codes */
            foreach (var Op in OpCodes)
            {
                //Check if we need exception to push?
                var xNeedsExceptionPush = (Op.Ehc != null) && (((Op.Ehc.HandlerOffset > 0 && Op.Ehc.HandlerOffset == Op.Position) || ((Op.Ehc.Flags & ExceptionHandlingClauseOptions.Filter) > 0 && Op.Ehc.FilterOffset > 0 && Op.Ehc.FilterOffset == Op.Position)) && (Op.Ehc.Flags == ExceptionHandlingClauseOptions.Clause));
                                
                ILCompiler.Logger.Write(Op.ToString() + "; Stack Count => " + Core.vStack.Count);
                //Check if current position inside the list of label list, if yes than break label and make a new one
                if (NewLabels.Contains(Op.Position))
                {
                    var xLbl = new Label(ILHelper.GetLabel(aMethod, Op.Position));
                    if (!Core.AssemblerCode.Contains(xLbl))
                        Core.AssemblerCode.Add(xLbl);
                }
                //If catch IL here than push current error so, that catch IL pop and do what it want
                if (xNeedsExceptionPush)
                {
                    Core.AssemblerCode.Add(
                        new Push { 
                            DestinationRef = ((FieldInfo)Core.StaticLabels["Exception"]).FullName(), 
                            DestinationIndirect = true });

                    Core.vStack.Push(4, typeof(Exception));
                }
                //Well this is just to comment whole output Assembly
                Core.AssemblerCode.Add(new Comment(Op.ToString() + "; " + Core.vStack.Count));

                //Check if this IL is in out implementation
                if (MSIL.ContainsKey(Op.Code))
                {
                    //If yes than execute it
                    MSIL[Op.Code].Execute(Op, aMethod);
                }
                else
                    //If it is not implementation than comment inside that IL to assembly
                    //So, mind to check any type of comment in code =P
                    Core.AssemblerCode.Add(new Comment(Op.ToString() + "; " + Core.vStack.Count));
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
            //End the method and return method, without exception
            Core.AssemblerCode.Add(new Label(xMethodLabel + ".End"));
            //We assume that if the ecx is 0x0 then the method is done without any exception
            //it can be assumed by test instruction followed by conditional jump while calling a function
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.ECX, SourceRef = "0x0" });

            Core.AssemblerCode.Add(new Label(xMethodLabel + ".Error"));
            //Now below code save the return value to EBP varaible
            //And calculate size
            int ArgSize = (from item in aMethod.GetParameters()
                        select (int)item.ParameterType.SizeOf().Align()).Sum();
            
            if (!aMethod.IsStatic)
            {
                if (aMethod.DeclaringType.IsValueType)
                    ArgSize += 4;
                else
                    ArgSize += aMethod.DeclaringType.SizeOf().Align();
            }

            int xReturnSize = (aMethod is MethodInfo) ? ((MethodInfo)aMethod).ReturnType.SizeOf().Align() : 0;
            if (xReturnSize > 0)
            {
                //For return type Method
                var xOffset = ILHelper.GetResultCodeOffset((uint)xReturnSize, (uint)ArgSize);
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
            #warning Optimization
            //Core.AssemblerCode.Add(new Comment("End_optimization"));
            var xRetSize = ((int)ArgSize) - ((int)xReturnSize);
            if (xRetSize < 0)
            {
                xRetSize = 0;
            }
            //Leave this method mean regain original EBP and ESP offset
            Core.AssemblerCode.Add(new Leave());
            //Return to parent method with given stack offset
            Core.AssemblerCode.Add(new Ret { Address = (byte)xRetSize });

            //Add this method to build definations so we will not build it again
            BuildDefinations.Add(aMethod);

            //And log it
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
            #warning Need to check virtuals
            BuildDefinations.Add(aType);
        }

        public void FlushAsmFile()
        {
            //To Make output assembly looks good
            //But i comment this because the multiboot header comes to down
            //Core.DataMember.Sort();

            //Add a label of Kernel End, it is used by our heap to know from where it starts allocating memory
            Core.AssemblerCode.Add(new Label("Compiler_End"));

            using (var xWriter = new StreamWriter(Path.Combine(ILCompiler.OutputDir, Helper.KernelFile), false))
            {
                //Firstly add datamember
                foreach (var dm in Core.DataMember)
                    dm.FlushText(xWriter);

                //Leave a line because we want make it beautiful =P
                xWriter.WriteLine("");

                foreach (var ac in Core.AssemblerCode)
                {
                    if (ac is Label)
                    {
                        xWriter.WriteLine();
                        ac.FlushText(xWriter);                        
                    }
                    else
                    {
                        xWriter.Write("     ");
                        ac.FlushText(xWriter);
                    }
                }
            }
        }
    }
}
