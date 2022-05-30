using APSS.Domain.Entities;
using APSS.Domain.ValueTypes;

namespace APSS.Domain.Services;

public interface ILogsService
{
    /// <summary>
    /// Asynchronously gets logs
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    IAsyncEnumerable<Log> GetLogsAsync(params string[] tags);

    /// <summary>
    /// Asynchronously gets logs
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    IAsyncEnumerable<Log> GetLogsAsync(DateTimeRange range, params string[] tags);

    /// <summary>
    /// Asynchronously creates a log entry
    /// </summary>
    /// <param name="severity">The severity of the log</param>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogAsync(LogSeverity severity, string message, params string[] tags);

    /// <summary>
    /// Asynchronously creates a debug log entry
    /// </summary>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogDebugAsync(string message, params string[] tags);

    /// <summary>
    /// Asynchronously creates an information log entry
    /// </summary>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogInfoAsync(string message, params string[] tags);

    /// <summary>
    /// Asynchronously creates a warning log entry
    /// </summary>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogWarningAsync(string message, params string[] tags);

    /// <summary>
    /// Asynchronously creates an error log entry
    /// </summary>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogErrorAsync(string message, params string[] tags);

    /// <summary>
    /// Asynchronously creates a fatal log entry
    /// </summary>
    /// <param name="message">The message of the log</param>
    /// <param name="tags">Optional tags to be attached to the log</param>
    /// <returns>The created log object</returns>
    Task<Log> LogFatalAsync(string message, params string[] tags);
}
