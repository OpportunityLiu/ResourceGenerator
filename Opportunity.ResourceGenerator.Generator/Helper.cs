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

        public static HashSet<string> UsedInInterface { get; } = new HashSet<string>() { "GetValue" };
        public static HashSet<string> UsedInClass { get; } = new HashSet<string>() { "ReferenceEquals", "Equals", "GetHashCode", "GetType", "ToString", "MemberwiseClone" };

        public static string CombineResourcePath(string root, string reletive) => string.IsNullOrEmpty(root) ? reletive : $"{root}/{reletive}";

        public static string Refine(string name, bool allowDot = false)
        {
            if (allowDot)
                return string.Join(".", name.Split('.').Select(n => Refine(n)));
            if (string.IsNullOrEmpty(name))
                return "__Empty__";
            if (!isValidStartChar(name[0]))
                name = "_" + name;
            for (var i = 1; i < name.Length; i++)
            {
                if (!isValidPartChar(name[i]))
                    name = name.Replace(name[i], '_');
            }
            return name;

            bool isValidStartChar(char ch) => ch == '_' || isLetter(ch);
            bool isValidPartChar(char ch)
            {
                var c = (int)char.GetUnicodeCategory(ch);
                return c == 5 || c == 6 || c == 8 || c == 18 || c == 15 || isLetter(ch);
            }
            bool isLetter(char ch)
            {
                var c = (int)char.GetUnicodeCategory(ch);
                return c < 5 || c == 9;
            }
        }

        private static Random ran = new Random();

        public static string GetRandomName(string head)
        {
            var buf = new byte[6];
            ran.NextBytes(buf);
            var r = Convert.ToBase64String(buf);
            return $"{head}__{r.Replace("+", "_").Replace("/", "_")}";
        }

        public static string FormatNamespace(string namespaceName)
        {
            if (Configuration.Config.FileType == "vb" && namespaceName.StartsWith(Configuration.Config.ProjectDefaultNamespace))
                namespaceName = namespaceName.Substring(Configuration.Config.ProjectDefaultNamespace.Length);
            return namespaceName.Trim('.');
        }
    }
}