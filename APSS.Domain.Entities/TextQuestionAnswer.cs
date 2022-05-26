namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent a text question answer
/// </summary>
public sealed class TextQuestionAnswer : QuestionAnswer
{
    /// <summary>
    /// Gets or sets the answer of the text question
    /// </summary>
    public TextQuestion Answer { get; set; } = null!;
}
