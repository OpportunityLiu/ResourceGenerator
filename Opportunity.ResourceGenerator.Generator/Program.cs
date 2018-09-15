using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Opportunity.ResourceGenerator.Generator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.IsNullOrEmpty())
                {
                    PrintHelp();
                    return 0;
                }
                var root = args.First();
                switch (Path.GetExtension(root))
                {
                case ".sln":
                    HandleSolution(root);
                    return 0;
                case ".csproj":
                case ".vbproj":
                    var config = args.Skip(1).ToArray();
                    HandleProject(root, config);
                    return 0;
                default:
                    Logger.LogWarning(0, "Unsupported parameter. Run without parameters to get help.");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(0, ex.Message);
                return -1;
            }
        }

        private static void HandleSolution(string slnPath)
        {
            slnPath = Path.GetFullPath(slnPath);
            Logger.LogInfo(0, $"Handling solution \"{Path.GetFileNameWithoutExtension(slnPath)}\"");
            try
            {
                var projs = Directory.GetFiles(Path.GetDirectoryName(slnPath), "*.*proj", SearchOption.AllDirectories);
                if (projs.Length == 0)
                {
                    Logger.LogInfo(1, "No csharp project found in solution.");
                    return;
                }
                foreach (var item in projs)
                {
                    HandleProject(item, null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(1, ex.Message);
            }
        }

        private static void HandleProject(string projPath, string[] resgenconfigPaths)
        {
            projPath = Path.GetFullPath(projPath);
            Logger.LogInfo(1, $"Handling project \"{Path.GetFileNameWithoutExtension(projPath)}\"");
            try
            {
                Configuration.SetCurrent(projPath, null);
                if (resgenconfigPaths == null || resgenconfigPaths.Length == 0)
                    resgenconfigPaths = Directory.GetFiles(Configuration.Config.ProjectDirectory, "*.resgenconfig", SearchOption.AllDirectories);
                if (resgenconfigPaths.Length == 0)
                {
                    Logger.LogInfo(2, "No .resgenconfig file found in project");
                    return;
                }
                foreach (var item in resgenconfigPaths)
                {
                    HandleConfig(projPath, item);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(2, ex.Message);
            }
        }

        private static void HandleConfig(string projPath, string resgenconfigPath)
        {
            resgenconfigPath = Path.GetFullPath(resgenconfigPath);
            if (Path.GetExtension(resgenconfigPath) != ".resgenconfig")
            {
                Logger.LogWarning(2, $"Unsupported config file \"{resgenconfigPath.Substring(Configuration.Config.ProjectDirectory.Length)}\"");
                return;
            }
            Logger.LogInfo(2, $"Handling config \"{resgenconfigPath.Substring(Configuration.Config.ProjectDirectory.Length)}\"");
            var generatedFileName = "";
            try
            {
                Configuration.SetCurrent(projPath, resgenconfigPath);
                var className = Path.GetFileNameWithoutExtension(resgenconfigPath);
                generatedFileName = Path.Combine(Path.GetDirectoryName(resgenconfigPath), $"{className}.g.{Configuration.Config.FileType}");
                using (var writer = new StreamWriter(generatedFileName))
                {
                    var s = new CodeGen.Generator(Configuration.Config);
                    s.Write(writer);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(3, ex.Message);
                try
                {
                    File.WriteAllText(generatedFileName, $@"
Something went wrong in generation!

Message: {ex.Message}

StackTrace: {ex.StackTrace}", System.Text.Encoding.UTF8);
                }
                catch { }
            }
        }

        private static void PrintHelp()
        {
            Console.Write($@"
Opportunity.ResourceGenerator.Generator {Helper.ProductVersion}
====================================================
Usage:
[Project file path] <[resgenconfig file path] <[resgenconfig file path...]>>

[Solution file path]

");
        }
    }
}


