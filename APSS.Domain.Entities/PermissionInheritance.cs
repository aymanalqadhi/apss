namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent permission inheritance
/// </summary>
public sealed class PermissionInheritance : AuditableEntity
{
    /// <summary>
    /// Gets or set the user whose permissions inherited from
    /// </summary>
    public User From { get; set; } = null!;

    /// <summary>
    /// Gets or set the user who inherited the permissions
    /// </summary>
    public User To { get; set; } = null!;

    /// <summary>
    /// Gets or sets the time validity of the permission inherited
    /// </summary>
    public DateTime ValidUntil { get; set; }
}
