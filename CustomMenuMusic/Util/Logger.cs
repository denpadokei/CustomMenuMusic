using System.IO;
using System.Runtime.CompilerServices;

namespace CustomMenuMusic
{
    internal static class Logger
    {
        public static IPA.Logging.Logger logger;
        public enum LogLevel { Debug, Warning, Notice, Error, Critical };

        public static void Log(string m, [CallerFilePath] string filePath = null, [CallerLineNumber] int? line = null, [CallerMemberName] string member = null) => Log(m, LogLevel.Debug, null, filePath, line, member);
        public static void Log(string m, LogLevel l, string suggestedAction = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int? line = null, [CallerMemberName] string member = null)
        {
            var level = IPA.Logging.Logger.Level.Debug;
            switch (l) {
                case LogLevel.Debug: level = IPA.Logging.Logger.Level.Debug; break;
                case LogLevel.Notice: level = IPA.Logging.Logger.Level.Notice; break;
                case LogLevel.Warning: level = IPA.Logging.Logger.Level.Warning; break;
                case LogLevel.Error: level = IPA.Logging.Logger.Level.Error; break;
                case LogLevel.Critical: level = IPA.Logging.Logger.Level.Critical; break;
            }
            logger.Log(level, $"{Path.GetFileName(filePath)}({line})[{member}] : {m}");
            if (!string.IsNullOrEmpty(suggestedAction))
                logger.Log(level, $"{Path.GetFileName(filePath)}({line})[{member}] Suggested Action: {suggestedAction}");
        }
    }
}
