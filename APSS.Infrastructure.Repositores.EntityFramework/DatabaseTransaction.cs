using APSS.Domain.Repositories;

using Microsoft.EntityFrameworkCore.Storage;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public class DatabaseTransaction : IDatabaseTransaction
{
    private readonly IDbContextTransaction _tx;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="tx">Entity framework transaction object</param>
    public DatabaseTransaction(IDbContextTransaction tx)
        => _tx = tx;

    /// <inheritdoc/>
    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _tx.CommitAsync(cancellationToken);

    /// <inheritdoc/>
    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _tx.RollbackAsync(cancellationToken);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _tx.DisposeAsync();
    }
}