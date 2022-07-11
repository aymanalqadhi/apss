using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

public sealed class PermissionsService : IPermissionsService
{
    #region Fields

    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">The unit of work of the application</param>
    public PermissionsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public async Task<bool> HasPermissionsAsync(long accountId, long userId, PermissionType permissions)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindAsync(accountId);
        var distance = await GetSubuserDistanceAsync(account.User.Id, userId);

        return CorrelateDistanceWithPermissions(distance, permissions);
    }

    /// <inheritdoc/>
    public async Task<Account> ValidatePermissionsAsync(long accountId, long userId, PermissionType permissions)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindAsync(accountId);

        var distance = await GetSubuserDistanceAsync(account.User.Id, userId);

        if (!CorrelateDistanceWithPermissions(distance, permissions))
        {
            throw new InsufficientPermissionsException(
                accountId,
                $"account #{accountId} of user #{account.User.Id} with permissions {account.Permissions.ToFormattedString()} does not have permissions {permissions.ToFormattedString()} on user #{userId}");
        }

        return account;
    }

    #endregion Public Methods

    #region Private Methods

    private static bool CorrelateDistanceWithPermissions(int distance, PermissionType permissions)
    {
        if (distance < 0)
            return false;

        if (distance == 0)
            return true;

        return permissions.HasFlag(PermissionType.Read);
    }

    private async Task<int> GetSubuserDistanceAsync(long superuserId, long subuserId)
    {
        if (superuserId == subuserId)
            return 0;

        var superuser = await _uow.Users.Query().FindAsync(superuserId);

        if (superuser.AccessLevel == AccessLevel.Root)
            return 0;

        var subuser = await _uow.Users
            .Query()
            .Include(u => u.SupervisedBy!)
            .FindAsync(subuserId);

        if (superuser.AccessLevel.IsBelow(subuser.AccessLevel))
            return -1;

        for (int i = 0; ; ++i)
        {
            if (subuser.SupervisedBy is null)
                return -1;

            if (subuser.SupervisedBy.Id == superuser.Id)
                return i;

            subuser = await _uow.Users
                .Query()
                .Include(u => u.SupervisedBy!)
                .FindAsync(subuser.SupervisedBy.Id);
        }
    }

    #endregion Private Methods
}