using APSS.Domain.Repositories;

using Microsoft.EntityFrameworkCore.Storage;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public class AsyncDatabaseTransaction : IAsyncDatabaseTransaction
{
    #region Private Fields

    private readonly IDbContextTransaction _tx;

    #endregion Private Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="tx">Entity framework transaction object</param>
    public AsyncDatabaseTransaction(IDbContextTransaction tx)
        => _tx = tx;

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _tx.CommitAsync(cancellationToken);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _tx.DisposeAsync();
    }

    /// <inheritdoc/>
    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _tx.RollbackAsync(cancellationToken);

    #endregion Public Methods
}