using UnityEngine;

/// <summary>
/// Level of displayed message
/// </summary>
public enum LogLevel
{
    debug,
    info,
    warn,
    error,
    fatal
}

public static class Logger
{

    private static LogLevel logLevel = LogLevel.debug;

    /// <summary>
    /// Displays a given message if its severity level is at least the log level.
    /// </summary>
    /// <param name="message">String message to display</param>
    /// <param name="level">Severity level of message; checked against overall logLevel</param>
    public static void Log(string message, LogLevel level)
    {
        if (level >= logLevel)
        {
            if (level >= LogLevel.error) Debug.LogError(message);
            else Debug.Log(message);
        }

        // Kill on fatal: only in editor
#if UNITY_EDITOR
        if (level == LogLevel.fatal)
        {
            Application.Quit();
        }
#endif
    }
}
