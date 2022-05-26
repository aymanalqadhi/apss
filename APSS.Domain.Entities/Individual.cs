namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent an individual of the system
/// </summary>
public sealed class Individual : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the individual
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sex of the individual
    /// </summary>
    public IndividualSex Sex { get; set; }

    /// <summary>
    /// Gets or sets the datebirth of the individual
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the national id of the individual
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the individual
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the job of the individual
    /// </summary>
    public string Job { get; set; } = null!;

    /// <summary>
    /// Gets or sets the social status of the individual
    /// </summary>
    public string SocialStatus { get; set; } = null!;

    /// <summary>
    /// Gets or sets the address of the individual
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user associated with the individual
    /// </summary>
    public User AddedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the skills of the individual
    /// </summary>
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    /// <summary>
    /// Gets or sets the voluntary of the individual
    /// </summary>
    public ICollection<Voluntary> Voluntary { get; set; } = new List<Voluntary>();
}

/// <summary>
/// An enum to represnt the sex of the individuals
/// </summary>
public enum IndividualSex : int
{
    Male = 0,
    Female = 1,
}