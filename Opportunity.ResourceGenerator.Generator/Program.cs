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
            try
            {
                if (args == null || args.Length == 0)
                {
                    PrintHelp();
                    return;
                }
                var root = args.First();
                switch (Path.GetExtension(root))
                {
                case ".sln":
                    HandleSolution(root);
                    break;
                case ".csproj":
                    var config = args.Skip(1).ToArray();
                    HandleProject(root, config);
                    break;
                default:
                    Logger.LogInfo("Unsupported parameter. Run without parameters to get help.");
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        static void HandleSolution(string slnPath)
        {
            slnPath = Path.GetFullPath(slnPath);
            Logger.LogInfo($"Start handling solution \"{Path.GetFileNameWithoutExtension(slnPath)}\"");
            var projs = Directory.GetFiles(Path.GetDirectoryName(slnPath), "*.csproj", SearchOption.AllDirectories);
            if (projs.Length == 0)
            {
                Logger.LogInfo("No csharp project found in solution.");
                return;
            }
            foreach (var item in projs)
            {
                HandleProject(item, null);
            }
        }

        static void HandleProject(string csprojPath, string[] resgenconfigPaths)
        {
            csprojPath = Path.GetFullPath(csprojPath);
            Configuration.SetCurrent(csprojPath, null);
            Logger.LogInfo($"Start handling project \"{Path.GetFileNameWithoutExtension(csprojPath)}\"");
            if (resgenconfigPaths == null || resgenconfigPaths.Length == 0)
                resgenconfigPaths = Directory.GetFiles(Configuration.Config.ProjectDirectory, "*.resgenconfig", SearchOption.AllDirectories);
            if (resgenconfigPaths.Length == 0)
            {
                Logger.LogInfo("No .resgenconfig file found in project");
                return;
            }
            foreach (var item in resgenconfigPaths)
            {
                HandleConfig(csprojPath, item);
            }
        }

        static void HandleConfig(string csprojPath, string resgenconfigPath)
        {
            resgenconfigPath = Path.GetFullPath(resgenconfigPath);
            if (Path.GetExtension(resgenconfigPath) != ".resgenconfig")
            {
                Logger.LogWarning($"Unsupported config file \"{resgenconfigPath.Substring(Configuration.Config.ProjectDirectory.Length)}\"");
                return;
            }
            Logger.LogInfo($"Start handling config \"{resgenconfigPath.Substring(Configuration.Config.ProjectDirectory.Length)}\"");
            var className = Path.GetFileNameWithoutExtension(resgenconfigPath);
            var generatedFileName = Path.Combine(Path.GetDirectoryName(resgenconfigPath), $"{className}.g.cs");
            try
            {
                Configuration.SetCurrent(csprojPath, resgenconfigPath);
                using (var writer = new ResourceWriter(generatedFileName))
                {
                    writer.Execute();
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(generatedFileName, $@"Something went wrong in generation!

Message: {ex.Message}

StackTrace: {ex.StackTrace}", System.Text.Encoding.UTF8);
                throw;
            }
        }

        static void PrintHelp()
        {
            Console.Write(@"
Opportunity.ResourceGenerator.Generator
====================================================
Usage:
[Project file path] <[resgenconfig file path] <[resgenconfig file path...]>>

[Solution file path]
");
        }
    }
}


