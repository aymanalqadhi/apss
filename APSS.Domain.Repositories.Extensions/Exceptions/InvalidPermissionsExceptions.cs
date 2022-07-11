using APSS.Domain.Entities;

namespace APSS.Domain.Repositories.Extensions.Exceptions;

/// <summary>
/// An exception to be thrown when invalid permissions are detected in an account
/// </summary>
public sealed class InvalidPermissionsExceptions : Exception
{
    private readonly long _accountId;
    private readonly PermissionType _exceptedPermissoins;
    private readonly PermissionType _actualPermissoins;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accountId">The id of the account with invalid permissions</param>
    /// <param name="exceptedPermissoins">The excepted permissions</param>
    /// <param name="actualPermissoins">The permissions of the account</param>
    public InvalidPermissionsExceptions(long accountId, PermissionType exceptedPermissoins, PermissionType actualPermissoins)
        : base($"account #{accountId} has invalid permissions {actualPermissoins} (expected {exceptedPermissoins}")
    {
        _accountId = accountId;
        _exceptedPermissoins = exceptedPermissoins;
        _actualPermissoins = actualPermissoins;
    }

    /// <summary>
    /// Gets the id of the account with invalid permissions
    /// </summary>
    public long AccountId => _accountId;

    /// <summary>
    /// Gets the excepted permissions
    /// </summary>
    public PermissionType ExceptedPermissoins => _exceptedPermissoins;

    /// <summary>
    /// Gets the permissions found with the account
    /// </summary>
    internal PermissionType ActualPermissoins => _actualPermissoins;
}