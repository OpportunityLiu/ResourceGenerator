using System.Reflection;

namespace ResourceGenerator
{
    public static class Helper
    {
        public static Assembly Assembly { get; } = typeof(Helper).Assembly;
        public static AssemblyName AssemblyName { get; } = new AssemblyName(Assembly.FullName);

        public static string ProductName => Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public static string ProductVersion => AssemblyName.Version.ToString();
    }
}
