namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent skills of individual
/// </summary>
public sealed class Skill : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the skill
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the skill
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the field of the skill
    /// </summary>
    public string Field { get; set; } = null!;

    /// <summary>
    /// Gets or sets the individuals which this skill belongs to
    /// </summary>
    public Individual BelongsTo { get; set; } = null!;
}