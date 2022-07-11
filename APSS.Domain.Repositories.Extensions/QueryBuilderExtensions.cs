using APSS.Domain.Entities;

namespace APSS.Domain.Repositories.Extensions;

public static class QueryBuilderExtensions
{
    /// <summary>
    /// An extension to <see cref="IQueryBuilder{T}"/> that allows getting items by id
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity of the builder</typeparam>
    /// <param name="self">The query builder</param>
    /// <param name="id">The id to look for</param>
    /// <returns>A modified query builder</returns>
    public static IQueryBuilder<TEntity> Find<TEntity>(this IQueryBuilder<TEntity> self, long id)
        where TEntity : AuditableEntity
    {
        return self.Where(i => i.Id == id);
    }

    /// <summary>
    /// An extension to <see cref="IQueryBuilder{T}"/> that allows asynchronously getting items by id
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity of the builder</typeparam>
    /// <param name="self">The query builder</param>
    /// <param name="id">The id to look for</param>
    /// <returns>A modified query builder</returns>
    public static Task<TEntity> FindAsync<TEntity>(this IQueryBuilder<TEntity> self, long id)
        where TEntity : AuditableEntity
    {
        return self.FirstAsync(i => i.Id == id);
    }

    /// <summary>
    /// An extension to asynchrnously check for an item's existence
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="self"></param>
    /// <param name="item">The item to check for</param>
    /// <returns>True if the item exists in the data store, false otherwise</returns>
    public static Task<bool> ContainsAsync<TEntity>(this IQueryBuilder<TEntity> self, TEntity item)
        where TEntity : AuditableEntity
    {
        return self.AnyAsync(i => i.Id == item.Id);
    }
}