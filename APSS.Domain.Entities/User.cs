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
    /// Gets or sets whether the user is active or not
    /// </summary>
    public bool IsActive { get; set; } = false;

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
/// An enum to represent the access level
/// </summary>
public enum AccessLevel : int
{
    Root = 0,
    Presedint = 1,
    Governorate = 2,
    Directorate = 3,
    District = 4,
    Village = 5,
    Group = 6,
    Farmer = 7,
}