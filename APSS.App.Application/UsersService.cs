using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

public sealed class UsersService : IUsersService
{
    #region Fields

    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">The unit of work of the application</param>
    /// <param name="permissionsSvc">the permissions service</param>
    public UsersService(IUnitOfWork uow, IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public async Task<User> CreateAsync(long accountId, string name)
    {
        var superAccount = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindAsync(accountId);

        if (superAccount.User.AccessLevel == AccessLevel.Farmer)
        {
            throw new InsufficientPermissionsException(
                accountId,
                $"farmer #{superAccount.User.Id} with account #{accountId} cannot add subusers");
        }

        var user = new User
        {
            Name = name,
            SupervisedBy = superAccount.User,
            AccessLevel = (AccessLevel)(((int)superAccount.User.AccessLevel) + 1)
        };

        _uow.Users.Add(user);
        await _uow.CommitAsync();

        return user;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<User>> GetSubuserAsync(int accountId)
    {
        await _uow.Accounts.Query()
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        return _uow.Users.Query().Where(u => u.SupervisedBy!.Id == accountId);
    }

    /// <inheritdoc/>
    public async Task<User> SetUserStatusAsync(
        long accountId,
        long userId,
        UserStatus newStatus)
    {
        return await UpdateAsync(accountId, userId, u => u.UserStatus = newStatus);
    }

    /// <inheritdoc/>
    public async Task<User> UpdateAsync(long accountId, long userId, Action<User> updater)
    {
        await _permissionsSvc.ValidatePermissionsAsync(accountId, userId, PermissionType.Update);

        var user = await _uow.Users.Query().FindAsync(userId);

        updater(user);

        _uow.Users.Update(user);
        await _uow.CommitAsync();

        return user;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<User> GetUpwardHierarchyAsync(
        long accountId,
        Func<IQueryBuilder<User>, IQueryBuilder<User>>? builder = null)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        var userId = account.User.Id;

        while (true)
        {
            var query = _uow.Users.Query().Include(u => u.SupervisedBy!);

            if (builder is not null)
                query = builder(query);

            var user = await query.FindAsync(userId);

            yield return user;

            if (user.SupervisedBy is null)
                yield break;

            userId = user.SupervisedBy.Id;
        }
    }

    #endregion Public Methods
}