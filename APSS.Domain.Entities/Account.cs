namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent an account of the user
/// </summary>
public sealed class Account : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the peron who holds this account
    /// </summary>
    public string HolderName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password hash of the user
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the national id of the user
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the social status of the user
    /// </summary>
    public SocialStatus SocialStatus { get; set; } = SocialStatus.Unspecified;

    /// <summary>
    /// Gets or sets the job of the user
    /// </summary>
    public string? Job { get; set; }

    /// <summary>
    /// Gets or sets the user of the account
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user who created the account
    /// </summary>
    public User AddedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the permissions of the account
    /// </summary>
    public PermissionType Permissions { get; set; } 

}

/// <summary>
/// An enum to represent social status
/// </summary>
public enum SocialStatus
{
    Unspecified,
    Unmarried,
    Married,
    Divorced,
    Widowed,
}