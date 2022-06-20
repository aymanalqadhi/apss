namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// A subtype of <see cref="InvalidQuestionAnswerException{TAnswer}"/> for text quesitons
/// </summary>
public sealed class InvalidTextQuestionAnswerException : InvalidQuestionAnswerException<string?>
{
    /// <inheritdoc/>
    public InvalidTextQuestionAnswerException(long questionId, string? answer, string? message)
        : base(questionId, answer, message)
    {
    }
}