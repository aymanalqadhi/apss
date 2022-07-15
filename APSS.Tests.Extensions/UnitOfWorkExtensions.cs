using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Tests.Domain.Entities.Validators;

namespace APSS.Tests.Extensions;

public static class UnitOfWorkExtensions
{
    #region Public Methods

    /// <summary>
    /// Asynchronously adds a testing account
    /// </summary>
    /// <param name="self"></param>
    /// <param name="accessLevel">The access level of the account's user</param>
    /// <param name="permissions">The permissions of the account</param>
    /// <returns>The created account</returns>
    public static async Task<Account> CreateTestingAccountAsync(
        this IUnitOfWork self,
        AccessLevel accessLevel,
        PermissionType permissions)
    {
        var account = ValidEntitiesFactory.CreateValidAccount(permissions);
        account.User = ValidEntitiesFactory.CreateValidUser(accessLevel);

        self.Users.Add(GetUsersChain(account.User).Reverse().ToArray());
        self.Accounts.Add(account);
        await self.CommitAsync();

        return account;
    }

    /// <summary>
    /// Asynchronously adds a testing account for a specific user
    /// </summary>
    /// <param name="self"></param>
    /// <param name="userId">The id of the user to add the account for</param>
    /// <param name="permissions">The permissions of the account</param>
    /// <returns>The created account</returns>
    public static async Task<Account> CreateTestingAccountForUserAsync(
        this IUnitOfWork self,
        long userId,
        PermissionType permissions)
    {
        var user = await self.Users.Query().FindAsync(userId);
        var account = ValidEntitiesFactory.CreateValidAccount(permissions);

        account.User = user;
        self.Accounts.Add(account);
        await self.CommitAsync();

        return account;
    }

    /// <summary>
    /// Asynchronously adds an account above a specific user
    /// </summary>
    /// <param name="self"></param>
    /// <param name="userId">The id of the user to add the account above</param>
    /// <param name="accessLevel">The access level of the account user</param>
    /// <param name="permissions">The permissions of the account</param>
    /// <returns>The created account</returns>
    /// <exception cref="InvalidOperationException">Thrown when invalid access level is specified</exception>
    public static async Task<Account> CreateTestingAccountAboveUserAsync(
        this IUnitOfWork self,
        long userId,
        AccessLevel accessLevel,
        PermissionType permissions)
    {
        var user = await self.Users.Query().FindAsync(userId);

        if (user.AccessLevel.IsAboveOrEqual(accessLevel))
            throw new InvalidOperationException($"user #{userId} access level {accessLevel} is above or equal to {accessLevel}");

        var account = await self.CreateTestingAccountAsync(accessLevel, permissions);

        while (user!.AccessLevel != accessLevel.NextLevelBelow())
        {
            user = (await self.Users
                    .Query()
                    .Include(u => u.SupervisedBy!)
                    .FindAsync(user.Id)
                   ).SupervisedBy;
        }

        user.SupervisedBy = account.User;
        self.Users.Update(user);

        await self.CommitAsync();

        return account;
    }

    #endregion Public Methods

    #region Private Methods

    private static IEnumerable<User> GetUsersChain(User? user)
    {
        while (user is not null)
        {
            yield return user;
            user = user.SupervisedBy;
        }
    }

    #endregion Private Methods
}