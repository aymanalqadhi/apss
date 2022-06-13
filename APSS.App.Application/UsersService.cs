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
    public async Task SetUserActiveStatusAsync(long superuserId, long userId, bool newActiveStatus)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        user.IsActive = newActiveStatus;
        await UpdateUserAsync(superuserId, user);
    }

    /// <inheritdoc/>
    public async Task<User> UpdateUserAsync(long superuserId, User user)
    {
        if (!await _permissionsSvc.HasWriteAccessAsync(superuserId, user.Id))
        {
            throw new InsufficientPermissionsException(
                superuserId,
                $"user {superuserId} does not have a permission to update user #{user.Id}");
        }

        _uow.Users.Update(user);
        await _uow.CommitAsync();

        return user;
    }

    #endregion Public Methods
}