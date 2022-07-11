using APSS.Domain.Entities;

namespace APSS.Domain.Services;

/// <summary>
/// An interface to be implemented by accounts service
/// </summary>
public interface IAccountsService
{
    /// <summary>
    /// Asynchrnously creates an account for a user
    /// </summary>
    /// <param name="superUserAccountId">The id of the account of the superuser of the user</param>
    /// <param name="userId">The id of the user to add the account for</param>
    /// <param name="holderName">The name of the holder of the account</param>
    /// <param name="password">The password of the account</param>
    /// <param name="permissions">The permissions of the account</param>
    /// <returns>The created account object</returns>
    Task<Account> CreateAccountAsync(
        long superUserAccountId,
        long userId,
        string holderName,
        string password,
        PermissionType permissions);
}