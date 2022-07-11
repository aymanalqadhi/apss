namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a user of the system
/// </summary>
public sealed class User : AuditableEntity
{
    /// <summary>
    /// Gets or stes the name of the user
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or stes the access level of the user
    /// </summary>
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Gets or sets the user status
    /// </summary>
    public UserStatus UserStatus { get; set; }

    /// <summary>
    /// Gets or sets the supervisor
    /// </summary>
    public User? SupervisedBy { get; set; }

    /// <summary>
    /// Gets or sets the collection of subusers under this user
    /// </summary>
    public ICollection<User> SubUsers { get; set; } = new List<User>();

    /// <summary>
    /// Gets or sets the collection of accounts under this user
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}

/// <summary>
/// An enum to represent the user status
/// </summary>
[Flags]
public enum AccessLevel : uint
{
    Farmer = 1 << 0,
    Group = 1 << 1,
    Village = 1 << 2,
    District = 1 << 3,
    Directorate = 1 << 4,
    Governorate = 1 << 5,
    Presedint = 1 << 6,
    Root = 0xFFFFFFFF,
}

/// <summary>
/// An enum to represent the access level
/// </summary>
public enum UserStatus
{
    Active,
    Inactive,
    Terminated,
}