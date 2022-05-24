namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent the taken survey
/// </summary>
public sealed class TakenSurvey : AuditableEntity
{
    /// <summary>
    /// Gets or sets who has taken the survey
    /// </summary>
    public User TakenBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the survey 
    /// </summary>
    public Survey Survey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the attempt date to take the survey
    /// </summary>
    public DateTime AttemptDate { get; set; }   
}
