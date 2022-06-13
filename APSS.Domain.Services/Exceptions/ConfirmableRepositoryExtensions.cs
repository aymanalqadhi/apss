using APSS.Domain.Entities;

namespace APSS.Domain.Repositories.Extensions;

public static class ConfirmableRepositoryExtensions
{
    /// <summary>
    /// An extension method that confirms a confirmable entity
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="self">=A reference to the repository object</param>
    /// <param name="entity">The entity to confirm</param>
    /// <returns>The confirmed object</returns>
    public static T Confirm<T>(this IRepository<T> self, T entity)
        where T : Confirmable
    {
        entity.IsConfirmed = true;

        self.Update(entity);

        return entity;
    }

    /// <summary>
    /// An extension method that confirms a decline entity
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <param name="self">=A reference to the repository object</param>
    /// <param name="entity">The entity to decline</param>
    /// <returns>The declined object</returns>
    public static T Decline<T>(this IRepository<T> self, T entity)
        where T : Confirmable
    {
        entity.IsConfirmed = false;

        self.Update(entity);

        return entity;
    }
}