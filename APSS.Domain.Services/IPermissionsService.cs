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
    /// Asynchronously validates permissions of an account
    /// </summary>
    /// <param name="accountId">The id of the account to validate</param>
    /// <param name="userId">The id of the user to check access to</param>
    /// <param name="permissions">The permissions to check</param>
    /// <returns>The validated account</returns>
    /// <exception cref="InsufficientPermissionsException"></exception>
    Task<Account> ValidatePermissionsAsync(long accountId, long userId, PermissionType permissions);

    /// <summary>
    /// Asynchronously validates that an account is a parent of a user
    /// </summary>
    /// <param name="accountId">The id of the account to validate</param>
    /// <param name="userId">The id of the user to check if the account is a parent of</param>
    /// <param name="permissions">The permissions to check for</param>
    /// <exception cref="InsufficientPermissionsException"></exception>
    /// <returns>The validated account</returns>
    Task<Account> ValidateUserPatenthoodAsync(
        long accountId,
        long userId,
        PermissionType permissions);

    /// <summary>
    /// Asynchronously validates that an account is a parent of a user of an account
    /// </summary>
    /// <param name="superUserAccountId">The id of the account to validate</param>
    /// <param name="accountId">The id of the account to check if the account is a parent of</param>
    /// <param name="permissions">The permissions to check for</param>
    /// <exception cref="InsufficientPermissionsException"></exception>
    /// <returns>The validated account (1) alongside the child account (2)</returns>
    Task<(Account, Account)> ValidateAccountPatenthoodAsync(
        long superUserAccountId,
        long accountId,
        PermissionType permissions);

    #endregion Public Methods
}