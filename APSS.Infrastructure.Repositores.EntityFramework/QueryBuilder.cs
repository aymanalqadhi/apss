using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APSS.Infrastructure.Repositores.EntityFramework;

public sealed class QueryBuilder<T> : IQueryBuilder<T> where T : AuditableEntity
{
    #region Private fields

    private IQueryable<T> _query;

    #endregion

    #region Ctors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="query">EntityFramework query object</param>
    public QueryBuilder(IQueryable<T> query)
    {
        _query = query;
    }

    #endregion

    #region Public methods

    /// <inheritdoc/>
    public IQueryBuilder<T> Include(Expression<Func<T, object>> expr)
    {
        _query = _query.Include(expr);

        return this;
    }

    /// <inheritdoc/>
    public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expr)
    {
        _query = _query.OrderBy(expr);

        return this;
    }

    /// <inheritdoc/>
    public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> expr)
    {
        _query = _query.OrderByDescending(expr);

        return this;
    }

    /// <inheritdoc/>
    public IQueryBuilder<T> Where(Expression<Func<T, bool>> pred)
    {
        _query = _query.Where(pred);

        return this;
    }

    /// <inheritdoc/>
    public Task<T> FirstAsync(CancellationToken token = default)
        => _query.FirstAsync(token);

    /// <inheritdoc/>
    public Task<T?> FirstOrNullAsync(CancellationToken token = default)
        => _query.FirstOrDefaultAsync(token);

    /// <inheritdoc/>
    public IAsyncEnumerable<T> AsAsyncEnumerable()
        => _query.AsAsyncEnumerable();

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken token = default)
        => _query.CountAsync(token);

    /// <inheritdoc/>
    public Task<int> CountAsync(Expression<Func<T, bool>> pred, CancellationToken token = default)
        => _query.CountAsync(pred, token);

    /// <inheritdoc/>
    public Task<bool> HasItemsAsync(CancellationToken token = default)
        => _query.AnyAsync(token);

    /// <inheritdoc/>
    public Task<bool> AnyAsync(Expression<Func<T, bool>> pred, CancellationToken token = default)
        => _query.AnyAsync(pred, token);

    /// <inheritdoc/>
    public Task<bool> AllAsync(Expression<Func<T, bool>> pred, CancellationToken token = default)
        => _query.AllAsync(pred, token);

    #endregion
}
