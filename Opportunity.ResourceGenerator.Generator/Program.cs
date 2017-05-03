using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Opportunity.ResourceGenerator.Generator
{
    class Program
    {
        static int Main(string[] args)
        {
            var project = args.First();
            Configuration.SetCurrent(project);
            var configPaths = args.Skip(1).ToArray();
            if (configPaths.Length == 0)
                configPaths = Directory.GetFiles(Configuration.Current.ProjectPath, "*.resgenconfig", SearchOption.AllDirectories);
            if (configPaths == null || configPaths.Length == 0)
                return 0;
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
                Console.WriteLine($"{item} ==> {generatedFileName}");
            }
            Console.WriteLine();
            Console.WriteLine($"Finished, {configPaths.Length} file(s) generated.");
            return 0;
        }
    }
}


