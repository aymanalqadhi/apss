namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// An exception to be thrown when an invalid question answer is found
/// </summary>
/// <typeparam name="TAnswer"></typeparam>
public abstract class InvalidQuestionAnswerException<TAnswer> : Exception
{
    #region Fields

    private readonly TAnswer _answer;
    private readonly long _questionId;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="questionId">The id of the question having invalid answer</param>
    /// <param name="answer">The faulty answer</param>
    /// <param name="message">An optional messsage to clarify errors</param>
    public InvalidQuestionAnswerException(long questionId, TAnswer answer, string? message)
        : base(message)
    {
        _questionId = questionId;
        _answer = answer;
    }

    #endregion Public Constructors

    #region Properties

    /// <summary>
    /// Gets the faulty answer
    /// </summary>
    public TAnswer Answer => _answer;

    /// <summary>
    /// Gets the id of the question
    /// </summary>
    public long QuestionId => _questionId;

    #endregion Properties
}