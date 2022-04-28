namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a report log
/// </summary>
public sealed class ReportLog : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the report log
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the creation date of the report log
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the type of the report log
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the survey
    /// </summary>
    public Survey Survey { get; set; } = null!;
}
