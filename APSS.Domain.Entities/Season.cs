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
    /// Gets or sets the begining month of the season
    /// </summary>
    public int BeginMonth { get; set; }

    /// <summary>
    /// Gets or sets the begining day of the season
    /// </summary>
    public int BeginDay { get; set; }

    /// <summary>
    /// Gets or sets the end month of the season
    /// </summary>
    public int EndMonth { get; set; }

    /// <summary>
    /// Gets or sets the end day of the season
    /// </summary>
    public int EndDay { get; set; }
}

