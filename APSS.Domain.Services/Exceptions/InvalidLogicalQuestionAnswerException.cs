namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// A subtype of <see cref="InvalidQuestionAnswerException{TAnswer}"/> for logical quesitons
/// </summary>
public sealed class InvalidLogicalQuestionAnswerException : InvalidQuestionAnswerException<bool?>
{
    /// <inheritdoc/>
    public InvalidLogicalQuestionAnswerException(long questionId, bool? answer, string? message)
        : base(questionId, answer, message)
    {
    }
}