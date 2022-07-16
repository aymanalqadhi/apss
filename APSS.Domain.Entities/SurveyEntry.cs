namespace APSS.Domain.Entities;

public class SurveyEntry : AuditableEntity
{
    /// <summary>
    /// Gets or sets the user who made the entry
    /// </summary>
    public User MadeBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the survey
    /// </summary>
    public Survey Survey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the answers of the survey
    /// </summary>
    public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
}