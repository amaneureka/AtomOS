using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

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
    }
}
