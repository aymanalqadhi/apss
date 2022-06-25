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
    /// Asynchronously checks whether an account has permissions of a user
    /// </summary>
    /// <param name="accountId">The id of the user account to check its permissions</param>
    /// <param name="userId">The id of the user to check accesss permissions of</param>
    /// <param name="permissions">The permission to check for</param>
    /// <returns>True if the user account has permissions, false otherise</returns>
    Task<bool> HasPermissionsAsync(long accountId, long userId, PermissionType permissions);

    /// <summary>
    /// Asynchronously validates
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="userId"></param>
    /// <param name="permissions"></param>
    /// <returns></returns>
    Task<Account> ValidatePermissionsAsync(long accountId, long userId, PermissionType permissions);

    #endregion Public Methods
}