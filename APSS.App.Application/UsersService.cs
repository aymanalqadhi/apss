using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;

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
    public Task<User> CreateUserAsync(
        long accountId,
        string name,
        string password,
        AccessLevel accessLevel)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<User>> GetSubuserAsync(int accountId)
    {
        await _uow.Accounts.Query().FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        return _uow.Users.Query().Where(u => u.SupervisedBy!.Id == accountId);
    }

    /// <inheritdoc/>
    public async Task<User> SetUserStatusAsync(long accountId, long userId, UserStatus newStatus)
    {
        await _permissionsSvc.ValidatePermissionsAsync(accountId, userId, PermissionType.Update);
        var user = await _uow.Users.Query().FindAsync(userId);

        user.UserStatus = newStatus;
        await UpdateUserAsync(accountId, user);

        return user;
    }

    /// <inheritdoc/>
    public async Task<User> UpdateUserAsync(long superuserId, User user)
    {
        await _permissionsSvc.ValidatePermissionsAsync(superuserId, user.Id, PermissionType.Update);

        _uow.Users.Update(user);
        await _uow.CommitAsync();

        return user;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<User> GetUserUpwardHierarchyAsync(
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