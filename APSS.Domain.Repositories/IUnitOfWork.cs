namespace APSS.Domain.Repositories;

public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously commits changes to data backend
    /// </summary>
    /// <returns>The affected records count</returns>
    Task<int> CommitAsync();

    /// <summary>
    /// Asynchronously commits changes to data backend
    /// </summary>
    /// <returns>The affected records count</returns>
    Task<int> CommitAsync(IDatabaseTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously begins a transaction
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IDatabaseTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
