namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// A subtype of <see cref="InvalidQuestionAnswerException{TAnswer}"/> for multiple choice quesitons
/// </summary>
public sealed class InvalidMultipleChoiceQuestionAnswerException : InvalidQuestionAnswerException<IEnumerable<string>>
{
    /// <inheritdoc/>
    public InvalidMultipleChoiceQuestionAnswerException(long questionId, IEnumerable<string> answer, string? message)
        : base(questionId, answer, message)
    {
    }
}