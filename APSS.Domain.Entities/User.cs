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
    /// Gets or sets the password hash of the user 
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password salt of the user 
    /// </summary>
    public string PasswordSalt { get; set; } = null!;

    /// <summary>
    /// Gets or sets the national id of the user 
    /// </summary>
    public long NationalId { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user 
    /// </summary>
    public long PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the social status of the user 
    /// </summary>
    public string SocialStatus { get; set; } = null!;

    /// <summary>
    /// Gets or sets the job of the user 
    /// </summary>
    public string Job { get; set; } = null!;

    /// <summary>
    /// Gets or sets the access level of the user 
    /// </summary>
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Gets or sets wether the user is a supervisor or not
    /// </summary>
    public bool IsSupervisor { get; set; } = false;

    /// <summary>
    /// Gets or sets the supervisor
    /// </summary>
    public User? SupervisedBy { get; set; } 
}
/// <summary>
/// An enum to represent the access level
/// </summary>
public enum AccessLevel
{
    Presedint,
    CityAdmin,
    DirectorateAdmin,
    DistrictAdmin,
    GroupAdmin,
    VillageAdmin,
    Farmer,
}