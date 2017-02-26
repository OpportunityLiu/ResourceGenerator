using System;
using System.IO;
using System.Reflection;

partial class Functions
{
    public string ProductName => ResourceGenerator.Helper.ProductName;
    public string ProductVersion => ResourceGenerator.Helper.ProductVersion;

    public host Host { get; private set; } = new host();

    public Functions(TextWriter sw)
    {
        this.sw = sw;
    }

    private TextWriter sw;

    public void WriteLine(string value)
    {
        sw.WriteLine(value);
    }
}

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

namespace EnvDTE
{
    class DTE
    {
        public Solu Solution { get; internal set; } = new Solu();

        public class Solu
        {
            internal pi FindProjectItem(object templateFile)
            {
                return new pi();
            }

            internal class pi
            {
                internal object ContainingProject = new Project();
            }
        }
    }

    class Project
    {
        public dict Properties { get; internal set; } = new dict();

        public class dict
        {
            internal item Item(string v)
            {
                switch(v)
                {
                case "DefaultNamespace":
                    return new item { Value = "ResourceGenerator" };
                case "AssemblyName":
                    return new item { Value = "ResourceGenerator" };
                default:
                    return new item();
                }
            }

            internal class item
            {
                internal object Value;
            }
        }
    }
}

class host : IServiceProvider
{
    internal object TemplateFile = null;

    public object GetService(Type serviceType)
    {
        return Activator.CreateInstance(serviceType);
    }

    internal string ResolveAssemblyReference(string v)
    {
        return Path.Combine(Environment.CurrentDirectory, "../..");
    }
}