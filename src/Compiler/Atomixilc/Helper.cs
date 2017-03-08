/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Compiler Helper and Support Functions
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Atomixilc
{
    public static class Helper
    {
        /// <summary>
        /// Label to identify and locate Heap Allocate Function
        /// </summary>
        public const string Heap_Label = "__Heap__";

        /// <summary>
        /// Label to identify and locate VTable Get Entry function
        /// </summary>
        public const string VTable_Label = "__VTable_GetEntry__";

        /// <summary>
        /// Label to identify and locate VTable Entry table
        /// </summary>
        public const string VTable_Flush = "__VTable_Flush__";

        /// <summary>
        /// Stores extra data element added by application through literal assembled code
        /// </summary>
        internal readonly static List<AsmData> DataSegment = new List<AsmData>();

        /// <summary>
        /// Stores extra BSS entries added by application through literal assembled code
        /// </summary>
        internal readonly static Dictionary<string, uint> ZeroSegment = new Dictionary<string, uint>();

        /// <summary>
        /// Stores cached Field Labels so we don't waste time in generating them again
        /// </summary>
        internal readonly static Dictionary<FieldInfo, string> cachedFieldLabel = new Dictionary<FieldInfo, string>();

        /// <summary>
        /// Stores cached MethodBase Labels so we don't waste time in generating them again
        /// </summary>
        internal readonly static Dictionary<MethodBase, string> cachedMethodLabel = new Dictionary<MethodBase, string>();

        /// <summary>
        /// Stores cached String Data Entry Key so we don't waste time in generating them again
        /// </summary>
        internal readonly static Dictionary<string, string> cachedResolvedStringLabel = new Dictionary<string, string>();

        /// <summary>
        /// Assembly name that should be restricted to Compilation Build Process
        /// </summary>
        internal readonly static HashSet<string> RestrictedAssembly = new HashSet<string>()
        { "mscorlib", "System", Assembly.GetExecutingAssembly().GetName().Name };

        /// <summary>
        /// Illegal Assembly Characters
        /// </summary>
        private static HashSet<char> IllegalChars = new HashSet<char>
        {
            ':', '.', '[', ']',
            '(', ')', '<', '>',
            '|', '/', '=', '+',
            '-', '*', '{', '}',
            '&', '%', '$', '#',
            '@', '!', '~', '`',
            '?', ' ', ',', '^'
        };

        /// <summary>
        /// Insert Application Data (non-Zero)
        /// </summary>
        /// <param name="aData">Data Entry</param>
        public static void InsertData(AsmData aData)
        {
            DataSegment.Add(aData);
        }

        /// <summary>
        /// Insert Application Data (Zero)
        /// </summary>
        /// <param name="key">Data name</param>
        /// <param name="size">Size of Data in bytes</param>
        public static void InsertData(string key, uint size)
        {
            ZeroSegment.Add(key, size);
        }

        /// <summary>
        /// Add plug to listening
        /// </summary>
        /// <param name="method">Plug defination function</param>
        /// <param name="target">Plug target method label</param>
        internal static void AddPlug(this MethodBase method, string target)
        {
            if (cachedMethodLabel.ContainsKey(method))
                cachedMethodLabel[method] = target;
            else
                cachedMethodLabel.Add(method, target);
        }

        /// <summary>
        /// Create and cache Label name for method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static string FullName(this MethodBase method)
        {
            if (cachedMethodLabel.ContainsKey(method))
                return cachedMethodLabel[method];

            if (method.GetCustomAttribute<DllImportAttribute>() != null)
            {
                cachedMethodLabel.Add(method, method.Name);
                return method.Name;
            }

            var SB = new StringBuilder();
            SB.Append((method is MethodInfo) ? ((MethodInfo)method).ReturnType.FullName : "System.Void");
            SB.Append('.');
            SB.Append(method.ReflectedType.FullName);
            SB.Append('.');
            SB.Append(method.Name);
            SB.Append('<');
            SB.Append(string.Join(", ", method.GetParameters().Select(b => b.ParameterType)));
            SB.Append('>');

            var illegalLabel = SB.ToString().ToArray();

            for (int i = 0; i < illegalLabel.Length; i++)
            {
                if (IllegalChars.Contains(illegalLabel[i]))
                    illegalLabel[i] = '_';
            }

            var label = new string(illegalLabel);
            cachedMethodLabel.Add(method, label);

            return label;
        }

        /// <summary>
        /// create and cache field name
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static string FullName(this FieldInfo field)
        {
            if (cachedFieldLabel.ContainsKey(field))
                return cachedFieldLabel[field];

            var illegalLabel = string.Format("static_Field__{2}.{1}.{0}", field.Name, field.DeclaringType, field.FieldType.FullName).ToArray();

            for (int i = 0; i < illegalLabel.Length; i++)
            {
                if (IllegalChars.Contains(illegalLabel[i]))
                    illegalLabel[i] = '_';
            }

            var label = new string(illegalLabel);
            cachedFieldLabel.Add(field, label);

            return label;
        }

        /// <summary>
        /// constructor key to store boolean variable
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static string ConstructorKey(this MethodBase method)
        {
            return string.Format("cctor_{0}", method.FullName());
        }

        /// <summary>
        /// method intermediate label
        /// </summary>
        /// <param name="NextPosition"></param>
        /// <returns></returns>
        internal static string GetLabel(int NextPosition)
        {
            return string.Format(".IL_{0}", NextPosition.ToString("X").PadLeft(5, '0'));
        }

        /// <summary>
        /// Is type signed?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsSigned(Type type)
        {
            if ((type == typeof(sbyte))
                || (type == typeof(short))
                || (type == typeof(int))
                || (type == typeof(long))
                || (type == typeof(IntPtr)))
                return true;
            return false;
        }

        /// <summary>
        /// create and cache string entry data name
        /// </summary>
        /// <param name="aData"></param>
        /// <returns></returns>
        internal static string GetResolvedStringLabel(string aData)
        {
            if (cachedResolvedStringLabel.ContainsKey(aData))
                return cachedResolvedStringLabel[aData];

            var str = string.Format("StringContent_{0}", cachedResolvedStringLabel.Count.ToString("X4"));
            cachedResolvedStringLabel[aData] = str;

            return str;
        }

        /// <summary>
        /// Get storage size in bytes for a given type on a given platform
        /// </summary>
        /// <param name="type"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        internal static int GetStorageSize(Type type, Architecture platform)
        {
            int size = 0;
            if (type.IsClass && !type.IsValueType)
                size = 12;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(a => a.Name).ToList();

            if (type.BaseType != null)
                fields.AddRange(type.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(a => a.Name));

            foreach (var fld in fields)
            {
                size += GetTypeSize(fld.FieldType, platform);
            }

            return size;
        }

        /// <summary>
        /// Get offset in memory for a field of type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="field"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        internal static int GetFieldOffset(Type type, FieldInfo field, Architecture platform)
        {
            int offset = 0;
            if (type.IsClass && !type.IsValueType)
                offset = 12;

            var attrib = field.GetCustomAttribute<FieldOffsetAttribute>();
            if (attrib != null)
                return attrib.Value;

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(a => a.Name).ToList();

            if (type.BaseType != null)
                fields.AddRange(type.BaseType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(a => a.Name));

            foreach(var fld in fields)
            {
                if (fld == field)
                    return offset;
                offset += GetTypeSize(fld.FieldType, platform);
            }

            throw new Exception(string.Format("Unable to find memory offset of '{0}' in type '{1}'", field.ToString(), type.ToString()));
        }

        /// <summary>
        /// Get stack offset for a local variable of given index in a method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="index"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        internal static int GetVariableOffset(MethodBody method, int index, Architecture platform)
        {
            var vars = method.LocalVariables;

            if (index >= vars.Count)
                throw new Exception("Variable Index out of bound");

            int offset = 4;
            for (int i = 0; i < index; i++)
                offset += GetTypeSize(vars[i].LocalType, platform, true);

            return offset;
        }

        /// <summary>
        /// Get type declaration size
        /// </summary>
        /// <param name="type"></param>
        /// <param name="platform"></param>
        /// <param name="aligned"></param>
        /// <returns></returns>
        internal static int GetTypeSize(Type type, Architecture platform, bool aligned = false)
        {
            if (aligned)
            {
                int size = GetTypeSize(type, platform, false);
                int alignment = GetTypeSize(typeof(IntPtr), platform);
                return ((size / alignment) + ((size % alignment) != 0 ? 1 : 0)) * alignment;
            }

            if (type == typeof(void))
                return 0;

            if ((type == typeof(byte))
                || (type == typeof(sbyte))
                || (type == typeof(bool)))
                return 1;

            if ((type == typeof(char))
                || (type == typeof(short))
                || (type == typeof(ushort)))
                return 2;

            if ((type == typeof(int))
                || (type == typeof(uint))
                || (type == typeof(float)))
                return 4;

            if ((type == typeof(long))
                || (type == typeof(ulong))
                || (type == typeof(double)))
                return 8;

            if (type == typeof(decimal))
                return 16;

            if ((type == typeof(IntPtr))
                || (type == typeof(UIntPtr))
                || (type.IsByRef)
                || (!type.IsValueType && type.IsClass)
                || (!string.IsNullOrEmpty(type.FullName) && type.FullName.EndsWith("*")))
            {
                switch (platform)
                {
                    case Architecture.x86: return 4;
                    case Architecture.x64: return 8;
                    default: throw new Exception(string.Format("GetTypeSize Unknown Platform '{0}'", platform));
                }
            }

            if (type.IsEnum)
                return GetTypeSize(type.GetField("value__").FieldType, platform);

            if (type.IsValueType)
            {
                var size = type.GetFields().Sum(field => GetTypeSize(field.FieldType, platform));
                var attrib = type.StructLayoutAttribute;
                if (attrib != null && size != attrib.Size)
                {
                    size = Math.Max(size, attrib.Size);
                    Verbose.Warning("GetTypeSize of type '{0}' mismatch. taking size: '{1}'", type, size);
                }
                return size;
            }

            return GetTypeSize(typeof(UIntPtr), platform);
        }
    }
}
