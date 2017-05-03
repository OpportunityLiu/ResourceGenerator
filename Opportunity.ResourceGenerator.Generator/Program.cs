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
            //var c = args.First();
            var c = @"C:\Users\liuzh\Source\Repos\ResourceGenerator\Opportunity.ResourceGenerator.TestApp\Opportunity.ResourceGenerator.TestApp.csproj";
            Configuration.SetCurrent(c);
            var configPaths = Directory.GetFiles(Configuration.Current.ProjectPath, "*.resgenconfig", SearchOption.AllDirectories);
            if (configPaths == null || configPaths.Length == 0)
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


