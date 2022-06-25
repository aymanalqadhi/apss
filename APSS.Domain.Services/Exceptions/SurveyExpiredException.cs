namespace APSS.Domain.Services.Exceptions;

/// <summary>
/// An exception to be thrown when an expired survey is accessed
/// </summary>
public sealed class SurveyExpiredException : Exception
{
    private readonly long _accountId;
    private readonly long _surveyId;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accountId">The id of the account accessing the survey</param>
    /// <param name="surveyId">The id of the expired survey</param>
    public SurveyExpiredException(long accountId, long surveyId)
    {
        _accountId = accountId;
        _surveyId = surveyId;
    }

    /// <summary>
    /// Gets the id of the account accessing the expired survey
    /// </summary>
    public long AccountId => _accountId;

    /// <summary>
    /// Gets the id of the expired survey
    /// </summary>
    public long SurveyId => _surveyId;
}