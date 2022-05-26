using APSS.Domain.Entities;
using System.Linq.Expressions;

namespace APSS.Domain.Repositories;

public interface IQueryBuilder<T> where T : AuditableEntity
{
    /// <summary>
    /// Adds an include to the query
    /// </summary>
    /// <param name="expr">The expression used to specify the include</param>
    /// <returns>The modified query builder</returns>
    IQueryBuilder<T> Include(Expression<Func<T, object>> expr);

    /// <summary>
    /// Orders the current query ascendingly
    /// </summary>
    /// <param name="expr">The expression used for ordering</param>
    /// <returns>The modified query builder</returns>
    IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expr);

    /// <summary>
    /// Orders the current query descendingly
    /// </summary>
    /// <param name="expr">The expression used for ordering</param>
    /// <returns>The modified query builder</returns>
    IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> expr);

    /// <summary>
    /// Adds a filter to the query
    /// </summary>
    /// <param name="pred">The filting expression</param>
    /// <returns>The modified query builder</returns>
    IQueryBuilder<T> Where(Expression<Func<T, bool>> pred);

    /// <summary>
    /// Asynchronously gets the first item in the query. Throws an exception if
    /// no items were found
    /// </summary>
    /// <param name="token">The task cancellation token</param>
    /// <returns>the first item in the query</returns>
    Task<T> FirstAsync(CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets the first item in the query. A null is returned if
    /// no items were found
    /// </summary>
    /// <returns>the first item in the query if found, null otherwise</returns>
    Task<T?> FirstOrNullAsync(CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets all items in the query
    /// </summary>
    /// <returns>The IAsyncEnumerable item of the items in query</returns>
    IAsyncEnumerable<T> AsAsyncEnumerable();

    /// <summary>
    /// Asynchronously gets the count of the items in the query
    /// </summary>
    /// <returns>The count of the items</returns>
    Task<int> CountAsync(CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets the count of the matching items in the query
    /// </summary>
    /// <param name="pred">The filting expression</param>
    /// <param name="token">The task cancellation token</param>
    /// <returns>The count of the matching items</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> pred, CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets whether the query has no items
    /// </summary>
    /// <param name="token">The task cancellation token</param>
    /// <returns>True if no items are in the qurey, false othersie</returns>
    Task<bool> HasItemsAsync(CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets whether any of the items in the query match the
    /// passed expression
    /// </summary>
    /// <param name="pred">the expression used for filering</param>
    /// <param name="token">The task cancellation token</param>
    /// <returns>True if any of items in the query match, false otherise</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> pred, CancellationToken token = default);

    /// <summary>
    /// Asynchronously gets whether all items in the query match the
    /// passed expression
    /// </summary>
    /// <param name="pred">the expression used for filering</param>
    /// <param name="token">The task cancellation token</param>
    /// <returns>True if all items in the query match, false otherise</returns>
    Task<bool> AllAsync(Expression<Func<T, bool>> pred, CancellationToken token = default);
}