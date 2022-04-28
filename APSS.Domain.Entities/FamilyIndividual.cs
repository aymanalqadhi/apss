namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent the association between family and individual
/// </summary>
public sealed class FamilyIndividual : AuditableEntity
{
    /// <summary>
    /// Gets or sets the inidviduals
    /// </summary>
    public Individual Individual { get; set; } = null!;

    /// <summary>
    /// Gets or sets the family
    /// </summary>
    public Family Family { get; set; } = null!;

    /// <summary>
    /// Gets or stes the role of the individual in the family
    /// </summary>
    public string Role { get; set; } = null!;

    /// <summary>
    /// Gets or sets wether the individual is a provider in the family or not
    /// </summary>
    public bool IsProvider { get; set; } = false;
}

