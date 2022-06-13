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
    /// Asynchronously checks if the user has read permission (any level of sub-users)
    /// </summary>
    /// <param name="userId">The user who to check for permission existance</param>
    /// <param name="subuserId">The user who to check against</param>
    /// <returns>True if the user has read access, false otherwise</returns>
    Task<bool> HasReadAccessAsync(long userId, long subuserId);

    /// <summary>
    /// Asynchronously checks if the user has rooot permissions
    /// </summary>
    /// <param name="userId">The id of the user to check</param>
    /// <returns>True if the user has root permissions, false otherwise</returns>
    Task<bool> HasRootAccessAsync(long userId);

    /// <summary>
    /// Asynchronously checks if the user has write permission (direct level of subusers)
    /// </summary>
    /// <param name="userId">The user who to check for permission existance</param>
    /// <param name="subuserId">The user who to check against</param>
    /// <returns>True if the user has write access, false otherwise</returns>
    Task<bool> HasWriteAccessAsync(long userId, long subuserId);

    /// <summary>
    /// Asynchrnously checks whether a user is the directy superuser of another user
    /// </summary>
    /// <param name="superuserId">The id of the super user</param>
    /// <param name="subuserId">The id of the subuser</param>
    /// <returns>True if the user is the direct superuser of the other user</returns>
    Task<bool> IsDirectSuperuserOfAsync(long superuserId, long subuserId);

    /// <summary>
    /// Asynchrnously checks whether a user is a superuser of another user
    /// </summary>
    /// <param name="superuserId">The id of the super user</param>
    /// <param name="subuserId">The id of the subuser</param>
    /// <returns>True if the user is a superuser of the other user</returns>
    Task<bool> IsSuperuserOfAsync(long superuserId, long subuserId);

    #endregion Public Methods
}