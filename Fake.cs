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

    public void Write(string value)
    {
        sw.Write(value);
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
    public class DTE
    {
        public Solu Solution { get; internal set; } = new Solu();

        public class Solu
        {
            public pi FindProjectItem(object templateFile)
            {
                return new pi();
            }

            public class pi
            {
                public object ContainingProject = new Project();
            }
        }
    }

    public class Project
    {
        public dict Properties { get; internal set; } = new dict();

        public class dict
        {
            public item Item(string v)
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

            public class item
            {
                internal object Value;
            }
        }
    }
}

public class host : IServiceProvider
{
    public object TemplateFile = null;

    public object GetService(Type serviceType)
    {
        return Activator.CreateInstance(serviceType);
    }

    public string ResolveAssemblyReference(string v)
    {
        return Path.Combine(Environment.CurrentDirectory, "../..");
    }
}
namespace Windows.ApplicationModel.Resources
{
    public class ResourceLoader
    {
        private string name;

        public ResourceLoader(string name) => this.name = name;

        public string GetValue(string key)
        {
            return $"{name}/{key}";
        }

        public static ResourceLoader GetForViewIndependentUse(string name)
        {
            return new ResourceLoader(name);
        }
    }
}