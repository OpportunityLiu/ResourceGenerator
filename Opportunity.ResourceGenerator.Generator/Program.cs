using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Opportunity.ResourceGenerator.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;
            var project = args.First();
            Configuration.SetCurrent(project);
            var configPaths = args.Skip(1).ToArray();
            if (configPaths.Length == 0)
                configPaths = Directory.GetFiles(Configuration.Current.ProjectPath, "*.resgenconfig", SearchOption.AllDirectories);
            if (configPaths.Length == 0)
                return;
            foreach (var item in configPaths)
            {
                var t = File.ReadAllText(item);
                JsonConvert.PopulateObject(t, Configuration.Current);
                var className = Path.GetFileNameWithoutExtension(item);
                var generatedFileName = Path.Combine(Path.GetDirectoryName(item), $"{className}.cs");
                className = Helper.Refine(className);
                Configuration.Current.LocalizedStringsClassName = className;
                using (var writer = new ResourceWriter(generatedFileName))
                {
                    writer.Execute();
                }
            }
        }
    }
}


