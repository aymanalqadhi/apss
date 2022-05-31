using APSS.Domain.Entities;

namespace APSS.Domain.Repositories;

public interface IRepository<T> where T : AuditableEntity
{
    /// <summary>
    /// Creates a query builder for <see cref="T"/>
    /// </summary>
    /// <returns>The query builder object</returns>
    IQueryBuilder<T> Query();

    /// <summary>
    /// Adds an item to the repository
    /// </summary>
    /// <param name="entities">The items to add</param>
    void Add(params T[] entities);

    /// <summary>
    /// Updates an item in the repository
    /// </summary>
    /// <param name="entity">The updated item</param>
    void Update(T entity);

    /// <summary>
    /// Deletes an item from the repositry
    /// </summary>
    /// <param name="entity">The item to delete</param>
    void Remove(T entity);
}