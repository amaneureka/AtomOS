using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace Atomixilc
{
    internal static class Helper
    {
        static Dictionary<FieldInfo, string> cachedFieldLabel = new Dictionary<FieldInfo, string>();
        static Dictionary<MethodBase, string> cachedMethodLabel = new Dictionary<MethodBase, string>();

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

        internal static string FullName(this MethodBase method)
        {
            if (cachedMethodLabel.ContainsKey(method))
                return cachedMethodLabel[method];

            var SB = new StringBuilder();
            SB.Append((method is MethodInfo) ? ((MethodInfo)method).ReturnType.FullName : "System.Void.");
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

        internal static string ConstructorKey(this MethodBase method)
        {
            return string.Format("cctor_{0}", method.FullName());
        }

        internal static string GetLabel(MethodBase xMethod, int NextPosition)
        {
            return string.Format("{0}.IL_{1}", xMethod.FullName(), NextPosition.ToString("X").PadLeft(5, '0'));
        }

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

        internal static int GetFieldOffset(Options Config, Type type, FieldInfo field)
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
                offset += GetTypeSize(fld.FieldType, Config.TargetPlatform);
            }

            throw new Exception(string.Format("Unable to find memory offset of '{0}' in type '{1}'", field.ToString(), type.ToString()));
        }

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

            throw new Exception(string.Format("GetTypeSize of Unhandled type '{0}'", type));
        }
    }
}
