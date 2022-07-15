using APSS.Domain.Entities;

namespace APSS.Domain.Repositories.Extensions.Exceptions;

/// <summary>
/// An exception to be thrown when an invalid access level is detected with a user
/// </summary>
public sealed class InvalidAccessLevelException : Exception
{
    private readonly long _accountId;
    private readonly long _userId;
    private readonly AccessLevel _expectedAccessLevel;
    private readonly AccessLevel _actualAccessLevel;
    private readonly PermissionType _expectedPermissions;
    private readonly PermissionType _actualPermissions;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accountId">The id of the account with invalid access level</param>
    /// <param name="userId">The id of the user with invalid access level</param>
    /// <param name="expectedAccessLevel">The expected access level</param>
    /// <param name="actualAccessLevel">The access level found</param>
    /// <param name="expectedPermissions">Optional permisions</param>
    /// <param name="actualPermissions">The permissions of the account</param>
    public InvalidAccessLevelException(
        long accountId,
        long userId,
        AccessLevel expectedAccessLevel,
        AccessLevel actualAccessLevel,
        PermissionType expectedPermissions,
        PermissionType actualPermissions) : base($"account #{accountId} of user #{userId} either does not have access level {expectedAccessLevel} (got {actualAccessLevel}) or has insufficient permissions {actualPermissions} (expected {actualPermissions})")
    {
        _accountId = accountId;
        _userId = userId;
        _expectedAccessLevel = expectedAccessLevel;
        _actualAccessLevel = actualAccessLevel;
        _expectedPermissions = expectedPermissions;
        _actualPermissions = actualPermissions;
    }

    /// <summary>
    /// Gets the id of the account whose user has invalid access level
    /// </summary>
    public long AccountId => _accountId;

    /// <summary>
    /// Gets the id of the user with invalid access level
    /// </summary>
    public long UserId => _userId;

    /// <summary>
    /// Gets the expected access level
    /// </summary>
    public AccessLevel ExpectedAccessLevel => _expectedAccessLevel;

    /// <summary>
    /// Gets the actual access level found in the user
    /// </summary>
    public AccessLevel ActualAccessLevel => _actualAccessLevel;

    /// <summary>
    /// Gets the optional permissions
    /// </summary>
    public PermissionType ExpectedPermissions => _expectedPermissions;

    /// <summary>
    /// Gets the permissions of the account
    /// </summary>
    public PermissionType ActualPermissions => _actualPermissions;
}