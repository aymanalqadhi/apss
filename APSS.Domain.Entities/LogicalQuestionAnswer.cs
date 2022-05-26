namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a logical question answer
/// </summary>
public sealed class LogicalQuestionAnswer : QuestionAnswer
{
    /// <summary>
    /// Gets or sets the answer of the logical question answer
    /// </summary>
    public bool Answer { get; set; } = false;
}
