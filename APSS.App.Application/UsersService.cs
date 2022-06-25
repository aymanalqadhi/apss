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
    public Task<User> CreateUserAsync(
        long superUserId,
        string name,
        string password,
        AccessLevel accessLevel)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IQueryBuilder<User> GetSubuser(int superuserId)
        => _uow.Users.Query().Where(u => u.SupervisedBy!.Id == superuserId);

    /// <inheritdoc/>
    public async Task SetUserStatusAsync(long superuserId, long userId, bool newActiveStatus)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        user.UserStatus = newActiveStatus;
        await UpdateUserAsync(superuserId, user);
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
        long userId,
        Func<IQueryBuilder<User>, IQueryBuilder<User>>? builder = null)
    {
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