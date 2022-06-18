using APSS.Domain.Entities;

namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// An exception to be thrown when a user tries to access, with lacking permissions, a resource
/// owned by another user
/// </summary>
public sealed class CannotAccessResouceOfException : Exception
{
    private readonly long _accessorUserId;
    private readonly long _ofUserId;
    private readonly long _resourceId;
    private readonly Type _resourcetype;
    private readonly PermissionType _permissions;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accessorUserId">The id of the user accessing the resource</param>
    /// <param name="ofUserId">The id of the user owning the resource</param>
    /// <param name="resourceId">The id of the accessed resource</param>
    /// <param name="resourcetype">The type of the accessed resource</param>
    /// <param name="permissions">The required permissions</param>
    public CannotAccessResouceOfException(
        long accessorUserId,
        long ofUserId,
        long resourceId,
        Type resourcetype,
        PermissionType permissions)
    {
        _accessorUserId = accessorUserId;
        _ofUserId = ofUserId;
        _resourceId = resourceId;
        _resourcetype = resourcetype;
        _permissions = permissions;
    }

    /// <summary>
    /// Gets required permissions
    /// </summary>
    public PermissionType Permissions => _permissions;

    /// <inheritdoc/>
    public override string Message
        => $"user #{_accessorUserId} cannot  {{ {string.Join(",", _permissions.GetPermissionValues())} }} {_resourcetype.Name.ToLower()} #{_resourceId} owned by user #{_ofUserId}";
}