using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Opportunity.ResourceGenerator.Generator
{
    public static class Helper
    {
        public static string GetUnusedName(ICollection<string> usedNames, IEnumerable<string> alternativeNames)
        {
            foreach (var item in alternativeNames)
            {
                if (!usedNames.Contains(item))
                    return item;
            }
            return null;
        }

        public static Assembly Assembly { get; } = typeof(Helper).Assembly;
        public static AssemblyName AssemblyName { get; } = new AssemblyName(Assembly.FullName);

        public static string ProductName => Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public static string ProductVersion => AssemblyName.Version.ToString();

        public static HashSet<string> Keywords = new HashSet<string>() { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

        private static HashSet<string> usedInInterface = new HashSet<string>() { "GetValue" };
        private static HashSet<string> usedInClass = new HashSet<string>() { "ReferenceEquals", "Equals", "GetHashCode", "GetType", "ToString", "MemberwiseClone" };

        public static string CombineResourcePath(string root, string reletive) => string.IsNullOrEmpty(root) ? reletive : $"{root}/{reletive}";

        public static string AsLiteral(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            var sb = new System.Text.StringBuilder(value.Length);
            foreach (var item in value)
            {
                var chType = char.GetUnicodeCategory(item);
                if (item == '"')
                    sb.Append(@"\""");
                else if (item == '\\')
                    sb.Append(@"\\");
                else if (item == '\0')
                    sb.Append(@"\0");
                else if (item == '\a')
                    sb.Append(@"\a");
                else if (item == '\b')
                    sb.Append(@"\b");
                else if (item == '\f')
                    sb.Append(@"\f");
                else if (item == '\r')
                    sb.Append(@"\r");
                else if (item == '\n')
                    sb.Append(@"\n");
                else if (item == '\t')
                    sb.Append(@"\t");
                else if (item == '\v')
                    sb.Append(@"\v");
                else if (chType == System.Globalization.UnicodeCategory.Control)
                {
                    sb.Append(@"\u");
                    sb.Append(((ushort)item).ToString("X4"));
                }
                else
                {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        public static string IPropModifier(string name)
        {
            if (usedInInterface.Contains(name))
                return "new ";
            return "";
        }

        public static string CPropModifier(string name)
        {
            if (usedInClass.Contains(name))
                return "new ";
            return "";
        }

        public static string Refine(string name, bool allowDot = false)
        {
            if (allowDot)
                return string.Join(".", name.Split('.').Select(n => Refine(n)));
            if (string.IsNullOrEmpty(name))
                return "__Empty__";
            if (Keywords.Contains(name))
                return "@" + name;
            if (!isValidStartChar(name[0]))
                name = "_" + name;
            for (var i = 1; i < name.Length; i++)
            {
                if (!isValidPartChar(name[i]))
                    name = name.Replace(name[i], '_');
            }
            return name;
        }

        private static bool isValidStartChar(char ch) => ch == '_' || isLetter(ch);

        private static bool isValidPartChar(char ch)
        {
            var c = (int)char.GetUnicodeCategory(ch);
            return c == 5 || c == 6 || c == 8 || c == 18 || c == 15 || isLetter(ch);
        }

        private static bool isLetter(char ch)
        {
            var c = (int)char.GetUnicodeCategory(ch);
            return c < 5 || c == 9;
        }

        private static Random ran = new Random();

        public static string GetRandomName(string head)
        {
            var buf = new byte[6];
            ran.NextBytes(buf);
            var r = Convert.ToBase64String(buf);
            return $"{head}__{r.Replace("+", "_").Replace("/", "_")}";
        }
    }
}