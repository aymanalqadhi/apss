namespace APSS.Domain.Entities;

/// <summary>
/// An abstract class to represent a question answer
/// </summary>
public abstract class QuestionAnswer : AuditableEntity
{
    /// <summary>
    /// Gets or sets the Question
    /// </summary>
    public Question Question { get; set; } = null!;
}
