namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a family
/// </summary>
public sealed class Family : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the family
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the living location of the family
    /// </summary>
    public string LivingLocation { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user associated with the family
    /// </summary>
    public User AddedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of individuals who belong to this family
    /// </summary>
    public ICollection<FamilyIndividual> Individuals { get; set; } = new List<FamilyIndividual>();
}
