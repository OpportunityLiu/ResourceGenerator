using System;
using System.IO;

namespace EnvDTE
{
    public class DTE
    {
        public SolutionPlaceHolder Solution { get; internal set; } = new SolutionPlaceHolder();

        public class SolutionPlaceHolder
        {
            public ProjectItemPlaceHolder FindProjectItem(object templateFile)
            {
                return new ProjectItemPlaceHolder();
            }

            public class ProjectItemPlaceHolder
            {
                public object ContainingProject = new Project();
            }
        }
    }

    public class Project
    {
        public DictionaryPlaceHolder Properties { get; internal set; } = new DictionaryPlaceHolder();

        public class DictionaryPlaceHolder
        {
            public ItemPlaceHolder Item(string v)
            {
                switch(v)
                {
                case "DefaultNamespace":
                    return new ItemPlaceHolder { Value = "ResourceGenerator" };
                case "AssemblyName":
                    return new ItemPlaceHolder { Value = "ResourceGenerator" };
                default:
                    return new ItemPlaceHolder();
                }
            }

            public class ItemPlaceHolder
            {
                internal object Value;
            }
        }
    }
    public class HostPlaceHolder : IServiceProvider
    {
        public object TemplateFile = null;

        object IServiceProvider.GetService(Type serviceType)
        {
            return Activator.CreateInstance(serviceType);
        }

        public string ResolveAssemblyReference(string v)
        {
            return Path.Combine(Environment.CurrentDirectory, "../..");
        }
    }
}