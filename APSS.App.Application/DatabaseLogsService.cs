using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Domain.ValueTypes;

namespace APSS.Application.App;

public sealed class DatabaseLogsService : ILogsService
{
    #region Private fields

    private readonly IUnitOfWork _uow;

    #endregion Private fields

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">the unit of work of the application</param>
    public DatabaseLogsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    #endregion Constructors

    #region Public methods

    /// <inheritdoc/>
    public IQueryBuilder<Log> GetLogsAsync(params string[] tags)
        => BuildLogsQuery(tags);

    /// <inheritdoc/>
    public IQueryBuilder<Log> GetLogsAsync(DateTimeRange range, params string[] tags)
    {
        var query = BuildLogsQuery(tags);

        return query
            .Where(l => l.CreatedAt >= range.Start && l.CreatedAt <= range.End);
    }

    /// <inheritdoc/>
    public async Task<Log> LogAsync(
        LogSeverity severity,
        string message,
        params string[] tags)
    {
        var log = new Log
        {
            Severity = severity,
            Message = message,
        };

        if (tags.Length > 0)
        {
            await using var tx = await _uow.BeginTransactionAsync();

            log.Tags = await PrepareTags(tags).ToListAsync();

            _uow.Logs.Add(log);
            await _uow.CommitAsync(tx);
        }
        else
        {
            _uow.Logs.Add(log);
            await _uow.CommitAsync();
        }

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

    #endregion Public methods

    #region Private methods

    private IQueryBuilder<Log> BuildLogsQuery(params string[] tags)
    {
        var query = _uow.Logs.Query().OrderByDescending(l => l.Id);

        if (tags.Length == 0)
            return query;

        return query.Where(l => l.Tags.Any(t => tags.Contains(t.Value)));
    }

    private async IAsyncEnumerable<LogTag> PrepareTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            var tagObject = await _uow.LogTags
                .Query()
                .FirstOrNullAsync(t => t.Value == tag);

            if (tagObject is null)
            {
                tagObject = new LogTag { Value = tag };
                _uow.LogTags.Add(tagObject);
            }

            yield return tagObject;
        }
    }

    #endregion Private methods
}