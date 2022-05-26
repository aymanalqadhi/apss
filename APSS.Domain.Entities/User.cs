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
    /// Gets or sets the name of the peron who holds this account
    /// </summary>
    public string? HolderName { get; set; }

    /// <summary>
    /// Gets or sets the password hash of the user 
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the national id of the user 
    /// </summary>
    public string NationalId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the phone number of the user 
    /// </summary>
    public string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the social status of the user 
    /// </summary>
    public SocialStatus SocialStatus { get; set; }

    /// <summary>
    /// Gets or sets the job of the user 
    /// </summary>
    public string Job { get; set; } = null!;

    /// <summary>
    /// Gets or sets the access level of the user 
    /// </summary>
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Gets or sets the supervisor
    /// </summary>
    public User? SupervisedBy { get; set; }

    /// <summary>
    /// Gets or sets the collection of subusers under this user
    /// </summary>
    public ICollection<User> SubUsers { get;set; } = new List<User>();
}

/// <summary>
/// An enum to represent the access level
/// </summary>
public enum AccessLevel : int
{
    Root = 0,
    Presedint = 1,
    City = 2,
    Directorate = 3,
    District = 4,
    Village = 5,
    Group = 6,
    Farmer = 7,
}

/// <summary>
/// An enum to represent social status
/// </summary>
public enum SocialStatus
{
    Unmarried,
    Married,
    Divorced,
    Widowed,
}