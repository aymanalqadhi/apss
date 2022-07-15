using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

/// <summary>
/// An interface to be implemented by user services
/// </summary>
public interface IUsersService
{
    #region Public Methods

    /// <summary>
    /// Asynchronosly creates a new user
    /// </summary>
    /// <param name="accountId">
    /// The id of the account of the user to add the new user under
    /// </param>
    /// <param name="name">The name of the user to be created</param>
    /// <returns>The created user object</returns>
    Task<User> CreateAsync(long accountId, string name);

    /// <summary>
    /// Asynchronously gets a query for the users set
    /// </summary>
    /// <param name="accountId">The id of the account which to get the subusers for</param>
    /// <returns></returns>
    Task<IQueryBuilder<User>> GetSubuserAsync(int accountId);

    /// <summary>
    /// Asynchronously set the user status of a user
    /// </summary>
    /// <param name="accountId">The id of id account of the superuser</param>
    /// <param name="userId">The id of the user to change</param>
    /// <param name="newStatus">The new status value</param>
    /// <returns></returns>
    Task<User> SetUserStatusAsync(long accountId, long userId, UserStatus newStatus);

    /// <summary>
    /// Asynchronously updates a user
    /// </summary>
    /// <param name="accountId">The id of the superuser of the user to update</param>
    /// <param name="userId">The id of the user to udpate</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateAsync(long accountId, long userId, Action<User> updater);

    /// <summary>
    /// Asyncrhnnously gets a user's upwards hierarchy (all users above)
    /// </summary>
    /// <param name="userId">The id of the uesr to get the heirarchy for</param>
    /// <param name="builder">An optional query builder to manipulate the qurey</param>
    /// <returns>A stream of users in the hierarchy</returns>
    IAsyncEnumerable<User> GetUpwardHierarchyAsync(
        long userId,
        Func<IQueryBuilder<User>, IQueryBuilder<User>>? builder = null);

    #endregion Public Methods
}