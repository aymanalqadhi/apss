using APSS.Domain.Entities;
using APSS.Domain.Services.Exceptions;

namespace APSS.Domain.Services;

/// <summary>
/// An interface to be implemented by permission inheritance services
/// </summary>
public interface IPermissionsService
{
    #region Public Methods

    /// <summary>
    /// Asynchronously creates a permission inheritance for a user
    /// </summary>
    /// <param name="superuserId">The id of the user to create the inheritance</param>
    /// <param name="fromId">The id of the source user</param>
    /// <param name="toId">The id of the desintantion user</param>
    /// <param name="permissions">The permissions to be inherited</param>
    /// <param name="validUntil">The expiratoin date of the inheritance</param>
    /// <returns>The created permission inheritance object</returns>
    /// <exception cref="InsufficientPermissionsException">
    /// Thrown when the superuser does not have root permissions
    /// </exception>
    /// <exception cref="InvalidDateTimeException">
    /// Thrown when a date in the past is passed
    /// </exception>
    Task<PermissionInheritance> GivePermissionsAsync(
        long superuserId,
        long fromId,
        long toId,
        PermissionType permissions,
        DateTime validUntil);

    /// <summary>
    /// Asynchronously checks whether a user has a user of another user or not
    /// </summary>
    /// <param name="userId">The user who to check permission inheritance for</param>
    /// <param name="ofUserId">The user whoes permissions are inherited</param>
    /// <param name="permissions">The permission to check for</param>
    /// <returns>True if the user has permissions, false otherise</returns>
    Task<bool> HasPermissionsOfAsync(long userId, long ofUserId, PermissionType permissions);

    /// <summary>
    /// Asychrnonously validates permissoins of a user to access another user's resources
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to be accessed</typeparam>
    /// <param name="userId">The user accessing the resources</param>
    /// <param name="ofUserId">The owner of the resource</param>
    /// <param name="resource">The accessed resource</param>
    /// <param name="permissions">The required permissions</param>
    /// <returns>The authorized user id</returns>
    /// <exception cref="CannotAccessResouceOfException">
    /// Thrown if the user has no access to the <see cref="ofUserId"/> resource
    /// </exception>
    Task<long> ValidatePermissionsAsync<TEntity>(
        long userId,
        long ofUserId,
        TEntity resource,
        PermissionType permissions) where TEntity : AuditableEntity;

    /// <summary>
    /// Asychrnonously validates permissoins of a user to access another user's resources
    /// </summary>
    /// <param name="userId">The user accessing the resources</param>
    /// <param name="ofUserId">The owner of the resource</param>
    /// <param name="permissions">The required permissions</param>
    /// <returns>The authorized user id</returns>
    /// <exception cref="InsufficientExecutionStackException">
    /// Thrown if the user has no access to the <see cref="ofUserId"/> resource
    /// </exception>
    Task<long> ValidatePermissionsAsync(
        long userId,
        long ofUserId,
        PermissionType permissions);

    #endregion Public Methods
}