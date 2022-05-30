using APSS.Application.App.Exceptions;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;

namespace APSS.Application.App;

public sealed class DatabaseLogsService : ILogsService
{
    #region Private fields

    private readonly IUnitOfWork _uow;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">the unit of work of the application</param>
    public DatabaseLogsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    #endregion

    #region Public methods

    /// <inheritdoc/>
    public async Task<Log> LogAsync(
        LogSeverity severity,
        string message,
        params string[] tags)
    {
        if (tags.FirstOrDefault(t => t.Contains(',')) is var tag && tag is not null)
            throw new InvalidLogTagException(tag);

        var log = new Log
        {
            Severity = severity,
            Message = message,
            Tags = string.Join(",", tags),
        };

        _uow.Logs.Add(log);
        await _uow.CommitAsync();

        return log;
    }

    /// <inheritdoc/>
    public Task<Log> LogDebugAsync(string message, params string[] tags)
        => LogAsync(LogSeverity.Debug, message, tags);

    /// <inheritdoc/>
    public Task<Log> LogInfoAsync(string message, params string[] tags)
        => LogAsync(LogSeverity.Information, message, tags);

    /// <inheritdoc/>
    public Task<Log> LogWarningAsync(string message, params string[] tags)
        => LogAsync(LogSeverity.Warning, message, tags);

    /// <inheritdoc/>
    public Task<Log> LogErrorAsync(string message, params string[] tags)
        => LogAsync(LogSeverity.Error, message, tags);

    /// <inheritdoc/>
    public Task<Log> LogFatalAsync(string message, params string[] tags)
        => LogAsync(LogSeverity.Fatal, message, tags);

    #endregion
}
