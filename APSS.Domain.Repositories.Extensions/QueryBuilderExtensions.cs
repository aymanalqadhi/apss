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
        return self.Find(id).FirstAsync();
    }
}