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

        internal static void Log(LogLevel level, int infoLevel, string info)
        {
            if (level < Level)
                return;
            if (info == null)
                return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(('[' + level.ToString() + ']').PadRight(10));
            Console.Write(new string(' ', infoLevel * 2));
            Console.ResetColor();
            var lines = info.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            Console.WriteLine(lines[0]);
            for (var i = 1; i < lines.Length; i++)
            {
                Console.Write("          ");
                Console.WriteLine(lines[i]);
            }
        }

        internal static void LogVerbose(int infoLevel, string info) => Log(LogLevel.Verbose, infoLevel, info);
        internal static void LogInfo(int infoLevel, string info) => Log(LogLevel.Info, infoLevel, info);
        internal static void LogWarning(int infoLevel, string info) => Log(LogLevel.Warning, infoLevel, info);
        internal static void LogError(int infoLevel, string info) => Log(LogLevel.Error, infoLevel, info);
    }
}
