/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Helper Function
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using Atomix.CompilerExt;
using Atomix.Assembler;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix
{
    public enum ILCode : ushort
    {
        #region Values
        Nop = 0x0000,
        Break = 0x0001,
        Ldarg_0 = 0x0002,
        Ldarg_1 = 0x0003,
        Ldarg_2 = 0x0004,
        Ldarg_3 = 0x0005,
        Ldloc_0 = 0x0006,
        Ldloc_1 = 0x0007,
        Ldloc_2 = 0x0008,
        Ldloc_3 = 0x0009,
        Stloc_0 = 0x000A,
        Stloc_1 = 0x000B,
        Stloc_2 = 0x000C,
        Stloc_3 = 0x000D,
        Ldarg_S = 0x000E,
        Ldarga_S = 0x000F,
        Starg_S = 0x0010,
        Ldloc_S = 0x0011,
        Ldloca_S = 0x0012,
        Stloc_S = 0x0013,
        Ldnull = 0x0014,
        Ldc_I4_M1 = 0x0015,
        Ldc_I4_0 = 0x0016,
        Ldc_I4_1 = 0x0017,
        Ldc_I4_2 = 0x0018,
        Ldc_I4_3 = 0x0019,
        Ldc_I4_4 = 0x001A,
        Ldc_I4_5 = 0x001B,
        Ldc_I4_6 = 0x001C,
        Ldc_I4_7 = 0x001D,
        Ldc_I4_8 = 0x001E,
        Ldc_I4_S = 0x001F,
        Ldc_I4 = 0x0020,
        Ldc_I8 = 0x0021,
        Ldc_R4 = 0x0022,
        Ldc_R8 = 0x0023,
        Dup = 0x0025,
        Pop = 0x0026,
        Jmp = 0x0027,
        Call = 0x0028,
        Calli = 0x0029,
        Ret = 0x002A,
        Br_S = 0x002B,
        Brfalse_S = 0x002C,
        Brtrue_S = 0x002D,
        Beq_S = 0x002E,
        Bge_S = 0x002F,
        Bgt_S = 0x0030,
        Ble_S = 0x0031,
        Blt_S = 0x0032,
        Bne_Un_S = 0x0033,
        Bge_Un_S = 0x0034,
        Bgt_Un_S = 0x0035,
        Ble_Un_S = 0x0036,
        Blt_Un_S = 0x0037,
        Br = 0x0038,
        Brfalse = 0x0039,
        Brtrue = 0x003A,
        Beq = 0x003B,
        Bge = 0x003C,
        Bgt = 0x003D,
        Ble = 0x003E,
        Blt = 0x003F,
        Bne_Un = 0x0040,
        Bge_Un = 0x0041,
        Bgt_Un = 0x0042,
        Ble_Un = 0x0043,
        Blt_Un = 0x0044,
        Switch = 0x0045,
        Ldind_I1 = 0x0046,
        Ldind_U1 = 0x0047,
        Ldind_I2 = 0x0048,
        Ldind_U2 = 0x0049,
        Ldind_I4 = 0x004A,
        Ldind_U4 = 0x004B,
        Ldind_I8 = 0x004C,
        Ldind_I = 0x004D,
        Ldind_R4 = 0x004E,
        Ldind_R8 = 0x004F,
        Ldind_Ref = 0x0050,
        Stind_Ref = 0x0051,
        Stind_I1 = 0x0052,
        Stind_I2 = 0x0053,
        Stind_I4 = 0x0054,
        Stind_I8 = 0x0055,
        Stind_R4 = 0x0056,
        Stind_R8 = 0x0057,
        Add = 0x0058,
        Sub = 0x0059,
        Mul = 0x005A,
        Div = 0x005B,
        Div_Un = 0x005C,
        Rem = 0x005D,
        Rem_Un = 0x005E,
        And = 0x005F,
        Or = 0x0060,
        Xor = 0x0061,
        Shl = 0x0062,
        Shr = 0x0063,
        Shr_Un = 0x0064,
        Neg = 0x0065,
        Not = 0x0066,
        Conv_I1 = 0x0067,
        Conv_I2 = 0x0068,
        Conv_I4 = 0x0069,
        Conv_I8 = 0x006A,
        Conv_R4 = 0x006B,
        Conv_R8 = 0x006C,
        Conv_U4 = 0x006D,
        Conv_U8 = 0x006E,
        Callvirt = 0x006F,
        Cpobj = 0x0070,
        Ldobj = 0x0071,
        Ldstr = 0x0072,
        Newobj = 0x0073,
        Castclass = 0x0074,
        Isinst = 0x0075,
        Conv_R_Un = 0x0076,
        Unbox = 0x0079,
        Throw = 0x007A,
        Ldfld = 0x007B,
        Ldflda = 0x007C,
        Stfld = 0x007D,
        Ldsfld = 0x007E,
        Ldsflda = 0x007F,
        Stsfld = 0x0080,
        Stobj = 0x0081,
        Conv_Ovf_I1_Un = 0x0082,
        Conv_Ovf_I2_Un = 0x0083,
        Conv_Ovf_I4_Un = 0x0084,
        Conv_Ovf_I8_Un = 0x0085,
        Conv_Ovf_U1_Un = 0x0086,
        Conv_Ovf_U2_Un = 0x0087,
        Conv_Ovf_U4_Un = 0x0088,
        Conv_Ovf_U8_Un = 0x0089,
        Conv_Ovf_I_Un = 0x008A,
        Conv_Ovf_U_Un = 0x008B,
        Box = 0x008C,
        Newarr = 0x008D,
        Ldlen = 0x008E,
        Ldelema = 0x008F,
        Ldelem_I1 = 0x0090,
        Ldelem_U1 = 0x0091,
        Ldelem_I2 = 0x0092,
        Ldelem_U2 = 0x0093,
        Ldelem_I4 = 0x0094,
        Ldelem_U4 = 0x0095,
        Ldelem_I8 = 0x0096,
        Ldelem_I = 0x0097,
        Ldelem_R4 = 0x0098,
        Ldelem_R8 = 0x0099,
        Ldelem_Ref = 0x009A,
        Stelem_I = 0x009B,
        Stelem_I1 = 0x009C,
        Stelem_I2 = 0x009D,
        Stelem_I4 = 0x009E,
        Stelem_I8 = 0x009F,
        Stelem_R4 = 0x00A0,
        Stelem_R8 = 0x00A1,
        Stelem_Ref = 0x00A2,
        Ldelem = 0x00A3,
        Stelem = 0x00A4,
        Unbox_Any = 0x00A5,
        Conv_Ovf_I1 = 0x00B3,
        Conv_Ovf_U1 = 0x00B4,
        Conv_Ovf_I2 = 0x00B5,
        Conv_Ovf_U2 = 0x00B6,
        Conv_Ovf_I4 = 0x00B7,
        Conv_Ovf_U4 = 0x00B8,
        Conv_Ovf_I8 = 0x00B9,
        Conv_Ovf_U8 = 0x00BA,
        Refanyval = 0x00C2,
        Ckfinite = 0x00C3,
        Mkrefany = 0x00C6,
        Ldtoken = 0x00D0,
        Conv_U2 = 0x00D1,
        Conv_U1 = 0x00D2,
        Conv_I = 0x00D3,
        Conv_Ovf_I = 0x00D4,
        Conv_Ovf_U = 0x00D5,
        Add_Ovf = 0x00D6,
        Add_Ovf_Un = 0x00D7,
        Mul_Ovf = 0x00D8,
        Mul_Ovf_Un = 0x00D9,
        Sub_Ovf = 0x00DA,
        Sub_Ovf_Un = 0x00DB,
        Endfinally = 0x00DC,
        Leave = 0x00DD,
        Leave_S = 0x00DE,
        Stind_I = 0x00DF,
        Conv_U = 0x00E0,
        Prefix7 = 0x00F8,
        Prefix6 = 0x00F9,
        Prefix5 = 0x00FA,
        Prefix4 = 0x00FB,
        Prefix3 = 0x00FC,
        Prefix2 = 0x00FD,
        Prefix1 = 0x00FE,
        Prefixref = 0x00FF,
        Arglist = 0xFE00,
        Ceq = 0xFE01,
        Cgt = 0xFE02,
        Cgt_Un = 0xFE03,
        Clt = 0xFE04,
        Clt_Un = 0xFE05,
        Ldftn = 0xFE06,
        Ldvirtftn = 0xFE07,
        Ldarg = 0xFE09,
        Ldarga = 0xFE0A,
        Starg = 0xFE0B,
        Ldloc = 0xFE0C,
        Ldloca = 0xFE0D,
        Stloc = 0xFE0E,
        Localloc = 0xFE0F,
        Endfilter = 0xFE11,
        Unaligned = 0xFE12,
        Volatile = 0xFE13,
        Tailcall = 0xFE14,
        Initobj = 0xFE15,
        Constrained = 0xFE16,
        Cpblk = 0xFE17,
        Initblk = 0xFE18,
        Rethrow = 0xFE1A,
        Sizeof = 0xFE1C,
        Refanytype = 0xFE1D,
        Readonly = 0xFE1E
        #endregion
    }

    public static class ILHelper
    {
        /// <summary>
        /// Just declare the current compiler instance for helping the helper =P
        /// </summary>
        public static Compiler Compiler;
        /// <summary>
        /// The array of assembly Illegal chars, if we won't remove it than we have to face a big legal action ^^
        /// </summary>
        private static HashSet<char> IllegalChars = new HashSet<char>
        { ':', '.', '[', ']',
          '(', ')', '<', '>',
          '|', '/', '=', '+',
          '-', '*', '{', '}',
          '&', '%', '$', '#',
          '@', '!', '~', '`', '?', ' ', ','};

        /*
         * Well the concept of saved labels
         * i adopt just to make compiler fast by wating it time in making the labels which it already done in past
         * By saving the labels in memory and if this label is called again than we just point it to memory
         */

        private static Dictionary<MethodBase, string> CachedMethodLabel = new Dictionary<MethodBase, string>();
        private static Dictionary<FieldInfo, string> CachedFieldLabel = new Dictionary<FieldInfo, string>();

        /// <summary>
        /// Save Label for method Base
        /// </summary>
        /// <param name="xMethod"></param>
        /// <param name="lbl"></param>
        private static void SaveLabel(MethodBase xMethod, string lbl)
        {
            if (!CachedMethodLabel.ContainsKey(xMethod))
                CachedMethodLabel.Add(xMethod, lbl);
        }

        /// <summary>
        /// Save Labels for Field Info
        /// </summary>
        /// <param name="xField"></param>
        /// <param name="lbl"></param>
        private static void SaveLabel(FieldInfo xField, string lbl)
        {
            if (!CachedFieldLabel.ContainsKey(xField))
                CachedFieldLabel.Add(xField, lbl);
        }

        /// <summary>
        /// Get Full name of method base, well because the reflection want give us whole name of method
        /// </summary>
        /// <param name="aMethod"></param>
        /// <param name="RemoveIllegalChars"></param>
        /// <returns></returns>
        public static string FullName(this MethodBase aMethod, bool RemoveIllegalChars = true)
        {
            if (RemoveIllegalChars == false)
            {
                /* In Later version, I'm planning to make the method Label by random label generator, it took less time
                 * But the issue is we are not able to debug assembly code by ourself...
                 * So it can only be done when we have stable assembly debugger
                 * Till than we have to be happy with this method
                 */
                StringBuilder SB = new StringBuilder();
                SB.Append((aMethod is MethodInfo) ? ((MethodInfo)aMethod).ReturnType.FullName : "System.Void.");
                SB.Append(".");
                SB.Append(aMethod.ReflectedType.FullName);
                SB.Append(".");
                SB.Append(aMethod.Name);
                SB.Append("<");
                SB.Append(string.Join(", ", (aMethod.GetParameters()).Select(b => string.Format("{0}", b.ParameterType))));
                SB.Append(">");
                return SB.ToString();
            }

            if (CachedMethodLabel.ContainsKey(aMethod))
                return CachedMethodLabel[aMethod];

            // Check if this method has any plug attribute
            string xLabel = null;
            Compiler.Plugs.TryGetValue(aMethod, out xLabel);

            if (xLabel == null)
                xLabel = aMethod.FullName(false);

            // remove illegal characters
            xLabel = xLabel.RemoveIllegalCharacters();
            SaveLabel(aMethod, xLabel);

            return xLabel;
        }

        /// <summary>
        /// Get Full name of field info
        /// </summary>
        /// <param name="aField"></param>
        /// <param name="RemoveIllegalChars"></param>
        /// <returns></returns>
        public static string FullName(this FieldInfo aField, bool RemoveIllegalChars = true)
        {
            if (RemoveIllegalChars)
            {
                if (CachedFieldLabel.ContainsKey(aField))
                    return CachedFieldLabel[aField];

                var xLabel = aField.FullName(false).RemoveIllegalCharacters();
                SaveLabel(aField, xLabel);

                return xLabel;
            }
            else
                return string.Format("static_Field__{2}.{1}.{0}", aField.Name, aField.DeclaringType, aField.FieldType.FullName);
        }

        public static string RemoveIllegalCharacters(this string aStr)
        {
            char[] xResult = new char[aStr.Length];

            for (int i = 0; i < aStr.Length; i++)
            {
                if (!IllegalChars.Contains(aStr[i]))
                    xResult[i] = aStr[i];
                else
                    xResult[i] = '_';
            }

            return new string(xResult);
        }

        /// <summary>
        /// Align the given value to IntPtr Size
        /// </summary>
        /// <param name="aValue">The Value to be Aligned</param>
        /// <returns></returns>
        public static int Align(this int aValue)
        {
            /* Just we make it a multiple of IntPtr :) */
            int val = (ILCompiler.CPUArchitecture == CPUArch.x86 ? 4 : 8);
            int xResult = aValue / val;
            if (aValue % val != 0)
                xResult += 1;
            return xResult * val;
        }

        /// <summary>
        /// The Label of Method with given offset value
        /// </summary>
        /// <param name="xMethod">Method</param>
        /// <param name="NextPosition">Offset Value</param>
        /// <returns></returns>
        public static string GetLabel(MethodBase xMethod, int NextPosition)
        {
            return string.Format("{0}.IL_{1}", xMethod.FullName(), NextPosition.ToString("X").PadLeft(4, '0'));
        }

        /// <summary>
        /// Very important, it just return the hard coded size of value types
        /// </summary>
        /// <param name="aType"></param>
        /// <returns></returns>
        public static int SizeOf(this Type aType)
        {
            if (aType.FullName == "System.Void")
                return 0;
            else if ((!aType.IsValueType && aType.IsClass) || aType.IsInterface)
                return 4;

            if (aType.IsByRef)
                return 4;

            switch (aType.FullName)
            {
                case "System.Char":
                    return 2;
                case "System.Byte":
                case "System.SByte":
                    return 1;
                case "System.UInt16":
                case "System.Int16":
                    return 2;
                case "System.UInt32":
                case "System.Int32":
                    return 4;
                case "System.UInt64":
                case "System.Int64":
                    return 8;
                case "System.UIntPtr":
                case "System.IntPtr":
                    return ILCompiler.CPUArchitecture == CPUArch.x86 ? 4 : 8;
                case "System.Boolean":
                    return 1;
                case "System.Single":
                    return 4;
                case "System.Double":
                    return 8;
                case "System.Decimal":
                    return 16;
            }

            if (aType.FullName != null && aType.FullName.EndsWith("*"))
                return 4;

            if (aType.IsEnum)
                return SizeOf(aType.GetField("value__").FieldType);

            if (aType.IsValueType)
            {
                var xSla = aType.StructLayoutAttribute;
                if (xSla != null)
                {
                    if (xSla.Size > 0)
                    {
                        return (int)xSla.Size;
                    }
                }
            }

            return 4;
        }

        /// <summary>
        /// Get The return type values offset inside EBP
        /// </summary>
        /// <param name="aResultSize"></param>
        /// <param name="aTotalArgumentSize"></param>
        /// <returns></returns>
        public static int GetResultCodeOffset(int aResultSize, int aTotalArgumentSize)
        {
            int xOffset = 8;
            if ((aTotalArgumentSize > 0) && (aTotalArgumentSize >= aResultSize))
            {
                xOffset += aTotalArgumentSize;
                xOffset -= aResultSize;
            }
            return xOffset;
        }

        /// <summary>
        /// Get memory offset of given variable count inside Method Body
        /// </summary>
        /// <param name="xBody"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static int MemoryOffset(MethodBody xBody, ushort Count)
        {
            var lv = xBody.LocalVariables;
            int xOffset = 0;
            for (int i = 0; i < lv.Count; i++)
            {
                if (i == Count)
                    return xOffset;
                xOffset += lv[i].LocalType.SizeOf().Align();
            }
            throw new Exception("Variable Not found");
        }

        /// <summary>
        /// Get size of whole type or struc
        /// </summary>
        /// <param name="aDeclaringType"></param>
        /// <returns></returns>
        public static int StorageSize(Type aDeclaringType)
        {
            // Here is we do thing same as GetFieldOffset
            // but we want last offset or just sum of sizes

            int xOffset = 0; // This is another way of calculation offset we just add the size of each field


            var xFields = (from item in aDeclaringType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                           orderby item.Name, item.DeclaringType.ToString()
                           select item).ToList();

            if (aDeclaringType.BaseType != null)
                xFields.AddRange((from item in aDeclaringType.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                  orderby item.Name, item.DeclaringType.ToString()
                                  select item).ToList());

            // Here reversing is not necessary so why waste time
            for (int i = 0; i < xFields.Count; i++)
            {
                var xField = xFields[i];

                // Maybe the entry is not what we want
                //if (xField.DeclaringType != aDeclaringType)
                //    continue;

                xOffset += xField.FieldType.SizeOf();
            }

            return xOffset;
        }

        /// <summary>
        /// Get the field info with given field ID and also its offset
        /// </summary>
        /// <param name="aDeclaringType"></param>
        /// <param name="aFieldId"></param>
        /// <param name="aFieldInfo"></param>
        /// <returns></returns>
        public static int GetFieldOffset(Type aDeclaringType, string aFieldId, out FieldInfo aFieldInfo)
        {
            /*
             * So what we do is get all fields of structure, than check the which field has
             * same field is as given and than we check the offset attribute ==> This is very necessary that
             * the field has an offset attribute
             */
            int xOffset = 0; // This is another way of calculation offset we just add the size of each field
            var xFields = (from item in aDeclaringType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                           orderby item.Name, item.DeclaringType.ToString()
                           select item).ToList();

            if (aDeclaringType.BaseType != null)
                xFields.AddRange((from item in aDeclaringType.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                  orderby item.Name, item.DeclaringType.ToString()
                                  select item).ToList());

            /* Because the array is from bottom to top */
            xFields.Reverse();

            for (int i = 0; i < xFields.Count; i++)
            {
                var xField = xFields[i];

                //Maybe the entry is not what we want
                //if (xField.DeclaringType != aDeclaringType)
                //    continue;

                // Check if this is what we want
                if (xField.FullName() == aFieldId)
                {
                    var xFieldOffsetAttrib = xField.GetCustomAttributes(typeof(FieldOffsetAttribute), true).FirstOrDefault() as FieldOffsetAttribute;
                    aFieldInfo = xField;
                    if (xFieldOffsetAttrib != null)
                        return (int)xFieldOffsetAttrib.Value;
                    else
                        return xOffset;
                }
                xOffset += xField.FieldType.SizeOf();
            }

            // If not found it should throw an error
            throw new Exception("FieldId Not found: " + aDeclaringType + ", " + aFieldId );
        }

        /// <summary>
        /// Simple function which tells given type is signed or not
        /// </summary>
        /// <param name="Vt"></param>
        /// <returns></returns>
        public static bool IsSigned(this Type Vt)
        {
            if (Vt.FullName == "System.Int16" ||
                Vt.FullName == "System.Int32" ||
                Vt.FullName == "System.Int64" ||
                Vt.FullName == "System.SByte")
                return true;
            return false;
        }
        /*
         * Get Type ID Label, it is used mainly in array, because array has a unique header information
         * where we have to set its metadata etc, and also Type ID
         * By assigning each type a unique number
         */
        private static int TypeIdCounter = 1;
        public static Dictionary<string, int> TypeIDLabel = new Dictionary<string, int>();
        public static string GetTypeIDLabel(Type aType)
        {
            var str = aType.FullName;

            char[] xResult = new char[aType.FullName.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (!IllegalChars.Contains(str[i]))
                    xResult[i] = str[i];
                else
                    xResult[i] = '_';
            }

            string xResult2 = ("TYPE_ID_LABEL__" + new string(xResult));

            // Add it to Data Members
            if (!TypeIDLabel.ContainsKey(xResult2))
            {
                Core.DataMember.Add(new AsmData(xResult2, "dd " + TypeIdCounter));
                TypeIDLabel.Add(xResult2, TypeIdCounter);
                TypeIdCounter++;
            }

            return xResult2;
        }

        public static int GetTypeID(Type aType)
        {
            var str = aType.FullName;

            char[] xResult = new char[aType.FullName.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (!IllegalChars.Contains(str[i]))
                    xResult[i] = str[i];
                else
                    xResult[i] = '_';
            }

            string xResult2 = ("TYPE_ID_LABEL__" + new string(xResult));

            // Add it to Data Members
            if (!TypeIDLabel.ContainsKey(xResult2))
            {
                // we don't add those labels in output assembly which we don't call literal -- optimization
                // Core.DataMember.Add(new AsmData(xResult2, "dd " + xCounter));
                TypeIDLabel.Add(xResult2, TypeIdCounter);
                TypeIdCounter++;
            }

            return TypeIDLabel[xResult2];
        }

        public static int GetArgumentsSize(MethodBase aMethod)
        {
            int ArgSize = (from item in aMethod.GetParameters()
                           select item.ParameterType.SizeOf().Align()).Sum();

            if (!aMethod.IsStatic)
            {
                if (aMethod.DeclaringType.IsValueType)
                    ArgSize += 4;
                else
                    ArgSize += aMethod.DeclaringType.SizeOf().Align();
            }

            return ArgSize;
        }

        public static int GetReturnTypeSize(MethodBase aMethod)
        {
            if (aMethod is MethodInfo)
                return ((MethodInfo)aMethod).ReturnType.SizeOf().Align();

            // constructors -- no return type
            return 0;
        }

        public static bool IsDelegate(Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type.BaseType);
        }

        public static int GetArgumentDisplacement(MethodBase aMethod, int aParamIndex)
        {
            var xMethodInfo = aMethod as MethodInfo;
            int xReturnSize = 0;

            if (xMethodInfo != null)
                xReturnSize = xMethodInfo.ReturnType.SizeOf().Align();

            int xOffset = 8; // EIP and calli header
            var xCorrectedOpValValue = aParamIndex;

            if (!aMethod.IsStatic)
                aParamIndex--;

            var xParams = aMethod.GetParameters();
            for (int i = xParams.Length - 1; i > aParamIndex; i--)
                xOffset += xParams[i].ParameterType.SizeOf().Align();


            return xOffset;
        }
    }
}
