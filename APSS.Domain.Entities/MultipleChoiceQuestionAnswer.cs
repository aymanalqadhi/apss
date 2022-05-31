namespace APSS.Domain.Entities;

/// <summary>
/// A class to represent
/// </summary>
public sealed class MultipleChoiceQuestionAnswer : QuestionAnswer
{
    /// <summary>
    /// Gets or sets the answers of the multiple choice question
    /// </summary>
    public ICollection<MultipleChoiceAnswerItem> Answers { get; set; } = new List<MultipleChoiceAnswerItem>();
}