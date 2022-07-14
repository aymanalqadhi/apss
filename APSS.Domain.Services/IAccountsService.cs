using APSS.Domain.Entities;

namespace APSS.Domain.Services;

/// <summary>
/// An interface to be implemented by accounts service
/// </summary>
public interface IAccountsService
{
    #region Public Methods

    /// <summary>
    /// Asynchrnously creates an account for a user
    /// </summary>
    /// <param name="superUserAccountId">The id of the account of the superuser of the user</param>
    /// <param name="userId">The id of the user to add the account for</param>
    /// <param name="holderName">The name of the holder of the account</param>
    /// <param name="password">The password of the account</param>
    /// <param name="permissions">The permissions of the account</param>
    /// <returns>The created account object</returns>
    Task<Account> CreateAsync(
        long superUserAccountId,
        long userId,
        string holderName,
        string password,
        PermissionType permissions);

    /// <summary>
    /// Asynchronously removes an account
    /// </summary>
    /// <param name="superUserAccountId">The id of the account to remove with</param>
    /// <param name="accountId">The id of the account to remove</param>
    /// <returns></returns>
    Task RemoveAsync(long superUserAccountId, long accountId);

    /// <summary>
    /// Asynchronously enables/disables an account
    /// </summary>
    /// <param name="superUserAccountId">The id of the account to make the change with</param>
    /// <param name="accountId">The id of the account to enable/disable</param>
    /// <param name="newActiveStatus">The new active status</param>
    /// <returns>The updated account</returns>
    Task<Account> SetActiveAsync(long superUserAccountId, long accountId, bool newActiveStatus);

    /// <summary>
    /// Asynchronously sets permissions of an account
    /// </summary>
    /// <param name="superUserAccountId">The id of the account to make the change with</param>
    /// <param name="accountId">The id of the account whose permissoins are to be set</param>
    /// <param name="permissions">The new permissoins</param>
    /// <returns>The updated account</returns>
    Task<Account> SetPermissionsAsync(long superUserAccountId, long accountId, PermissionType permissions);

    /// <summary>
    /// Asynchronously updates an account
    /// </summary>
    /// <param name="superUserAccountId">The id of the account to make the change with</param>
    /// <param name="accountId">The id of the account to be updated</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The updated account</returns>
    Task<Account> UpdateAsync(long superUserAccountId, long accountId, Action<Account> updater);

    #endregion Public Methods
}