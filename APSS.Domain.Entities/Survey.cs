namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a survey
/// </summary>
public sealed class Survey : AuditableEntity
{
    /// <summary>
    /// Gets or sets the name of the survey
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the expiration date of the survey
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Gets or sets the user who created the survey
    /// </summary>
    public User CreatedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of entries for this surveys
    /// </summary>
    public ICollection<SurveyEntry> Entries { get; set; } = new List<SurveyEntry>();
}
