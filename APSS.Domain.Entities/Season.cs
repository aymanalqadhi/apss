namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a season
/// </summary>
public sealed class Season : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the season
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the starting date of the season
    /// </summary>
    public DateTime StartsAt { get; set; }

    /// <summary>
    /// Gets or sets the starting date of the season
    /// </summary>
    public DateTime EndsAt { get; set; }
}
