using APSS.Domain.Entities;
using APSS.Domain.Repositories.Extensions.Exceptions;

namespace APSS.Domain.Repositories.Extensions;

public static class QueryBuilderSecurityExtensions
{
    /// <summary>
    /// Asynchronously gets an account by id, with access level and permissions validation
    /// </summary>
    /// <param name="self"></param>
    /// <param name="accountId">The id of the account to get and validate</param>
    /// <param name="accessLevel">The required access level</param>
    /// <param name="permissions">Optional permissions</param>
    /// <returns>The account with the supplied account</returns>
    /// <exception cref="InvalidAccessLevelException"></exception>
    public static async Task<Account> FindWithAccessLevelValidationAsync(
        this IQueryBuilder<Account> self,
        long accountId,
        AccessLevel accessLevel,
        PermissionType permissions)
    {
        var account = await self
            .Include(a => a.User)
            .FindAsync(accountId);

        if (account.User.AccessLevel != accessLevel || !account.Permissions.HasFlag(permissions))
        {
            throw new InvalidAccessLevelException(
                accountId,
                account.User.Id,
                accessLevel,
                account.User.AccessLevel,
                permissions,
                account.Permissions);
        }

        return account;
    }
}