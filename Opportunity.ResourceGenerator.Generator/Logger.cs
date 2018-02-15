using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opportunity.ResourceGenerator.Generator
{
    static class Logger
    {
        internal enum LogLevel
        {
            Verbose,
            Info,
            Warning,
            Error,
        }

        internal static LogLevel Level { get; set; } = LogLevel.Info;

        internal static void Log(LogLevel level, string info)
        {
            if (level < Level)
                return;
            if (info == null)
                return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(('[' + level.ToString() + ']').PadRight(10));
            Console.ResetColor();
            var lines = info.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            Console.WriteLine(lines[0]);
            for (var i = 1; i < lines.Length; i++)
            {
                Console.Write("          ");
                Console.WriteLine(lines[i]);
            }
        }

        internal static void LogVerbose(string info) => Log(LogLevel.Verbose, info);
        internal static void LogInfo(string info) => Log(LogLevel.Info, info);
        internal static void LogWarning(string info) => Log(LogLevel.Warning, info);
        internal static void LogError(string info) => Log(LogLevel.Error, info);
    }
}
