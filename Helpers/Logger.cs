using System;
using System.IO;

public static class Logger
{
    public static readonly Logging Log;

    static Logger()
    {
        Logging.IsEnabled = true;
        Log = new Logging("./scripts/WOI/Logging.log");
    }
}

public class Logging
{
    public static bool IsEnabled { get; set; } = true;

    [Flags]
    private enum LogLevel
    {
        TRACE = 0,
        INFO = 1,
        DEBUG = 2,
        WARNING = 3,
        ERROR = 4,
        FATAL = 5
    }

    private readonly object fileLock = new();
    private readonly string datetimeFormat;
    private readonly string logFilename;

    public Logging(string name)
    {
        datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
        logFilename = name;

        try
        {
            // Ensure folder exists
            string dir = Path.GetDirectoryName(logFilename);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Create file with header if it doesn't exist
            if (!File.Exists(logFilename))
            {
                using StreamWriter writer = File.CreateText(logFilename);
                writer.WriteLine($"{DateTime.Now.ToString(datetimeFormat)} Log file '{logFilename}' created.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Logger Init] Failed: {ex.Message}");
        }
    }

    public void Trace(string text) => WriteFormattedLog(LogLevel.TRACE, text);
    public void Info(string text) => WriteFormattedLog(LogLevel.INFO, text);
    public void Debug(string text) => WriteFormattedLog(LogLevel.DEBUG, text);
    public void Warning(string text) => WriteFormattedLog(LogLevel.WARNING, text);
    public void Error(string text) => WriteFormattedLog(LogLevel.ERROR, text);
    public void Fatal(string text) => WriteFormattedLog(LogLevel.FATAL, text);

    private void WriteFormattedLog(LogLevel level, string text)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(text)) return;

        string prefix = level switch
        {
            LogLevel.TRACE => "[TRACE]   ",
            LogLevel.INFO => "[INFO]    ",
            LogLevel.DEBUG => "[DEBUG]   ",
            LogLevel.WARNING => "[WARNING] ",
            LogLevel.ERROR => "[ERROR]   ",
            LogLevel.FATAL => "[FATAL]   ",
            _ => "[LOG]     "
        };

        string fullMessage = $"{DateTime.Now.ToString(datetimeFormat)} {prefix}{text}";
        WriteLine(fullMessage, append: true);
    }

    private void WriteLine(string text, bool append = false)
    {
        lock (fileLock)
        {
            using StreamWriter writer = new StreamWriter(logFilename, append, System.Text.Encoding.UTF8);
            writer.WriteLine(text);
        }
    }
}
