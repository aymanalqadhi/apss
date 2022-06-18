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
    public async Task<PermissionInheritance> GivePermissionsAsync(
        long superuserId,
        long fromId,
        long toId,
        PermissionType permissions,
        DateTime validUntil)
    {
        var superuser = await _uow.Users.Query().FindAsync(superuserId);

        if (superuser.AccessLevel != AccessLevel.Root)
            throw new InsufficientPermissionsException(superuserId, "only root can transfer permissions");

        if (validUntil <= DateTime.Now)
            throw new InvalidDateTimeException(validUntil, "permission inheritance expiry date cannot be in the past");

        var inheritance = new PermissionInheritance
        {
            From = await _uow.Users.Query().FindAsync(fromId),
            To = await _uow.Users.Query().FindAsync(toId),
            Permissions = permissions,
            ValidUntil = validUntil,
        };

        _uow.PermissionInheritances.Add(inheritance);
        await _uow.CommitAsync();

        return inheritance;
    }

    /// <inheritdoc/>
    public async Task<bool> HasPermissionsOfAsync(long userId, long ofUserId, PermissionType permissions)
    {
        if (userId == ofUserId)
            return true;

        if (await GetSubuserDistanceAsync(userId, ofUserId) is var distance && distance >= 0)
        {
            if (distance == 0 || permissions == PermissionType.Read)
                return true;
        }

        var user = await _uow.Users.Query().FindAsync(userId);
        var inheritance = await _uow.PermissionInheritances
            .Query()
            .Where(i => i.ValidUntil >= DateTime.Now)
            .Where(i => i.From.Id == ofUserId && i.To.Id == userId)
            .FirstOrNullAsync();

        if (inheritance is null)
            return false;

        return inheritance.Permissions.HasFlag(permissions);
    }

    /// <inheritdoc/>
    public async Task ValidatePermissionsAsync<TEntity>(
        long userId,
        long ofUserId,
        TEntity resource,
        PermissionType permissions) where TEntity : AuditableEntity
    {
        if (await HasPermissionsOfAsync(userId, ofUserId, permissions))
            return;

        throw new CannotAccessResouceOfException(userId, ofUserId, resource.Id, typeof(TEntity), permissions);
    }

    #endregion Public Methods

    #region Private Methods

    private async Task<int> GetSubuserDistanceAsync(long superuserId, long subuserId)
    {
        var superuser = await _uow.Users.Query().FindAsync(superuserId);

        if (superuser.AccessLevel == AccessLevel.Root)
            return 0;

        var subuser = await _uow.Users
            .Query()
            .Include(u => u.SupervisedBy!)
            .FindAsync(subuserId);

        if ((int)superuser.AccessLevel > (int)subuser.AccessLevel)
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