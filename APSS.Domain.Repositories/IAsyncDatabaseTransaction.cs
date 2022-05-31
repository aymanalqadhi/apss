namespace APSS.Domain.Repositories;

public interface IAsyncDatabaseTransaction : IAsyncDisposable
{
    /// <summary>
    /// Asynchrnously commits the transaction
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchrnously rolls back the transaction
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}