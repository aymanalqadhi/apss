using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

/// <summary>
/// An interface to represent survey managment service
/// </summary>
public interface ISurveysService
{
    #region Public Methods

    /// <summary>
    /// Asynchrnously adds a logical (true/false) question to a survey
    /// </summary>
    /// <param name="userId">The id of the user adding the question</param>
    /// <param name="surveyId">The id of the survey to add the question to</param>
    /// <param name="text">The text of the question</param>
    /// <param name="isRequired">Whether question is required or not</param>
    /// <returns>The created question object</returns>
    Task<LogicalQuestion> AddLogicalQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired);

    /// <summary>
    /// Asynchrnously adds a multiple-choice question to a survey
    /// </summary>
    /// <param name="userId">The id of the user adding the question</param>
    /// <param name="surveyId">The id of the survey to add the question to</param>
    /// <param name="text">The text of the question</param>
    /// <param name="isRequired">Whether question is required or not</param>
    /// <param name="canMultiSelect">Whether the question allows multi-select or not</param>
    /// <param name="candidateAnswers">The question candidate answers</param>
    /// <returns>The created question object</returns>
    Task<MultipleChoiceQuestion> AddMultipleChoiceQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired,
        bool canMultiSelect,
        params string[] candidateAnswers);

    /// <summary>
    /// Asynchrnously adds a text question to a survey
    /// </summary>
    /// <param name="userId">The id of the user adding the question</param>
    /// <param name="surveyId">The id of the survey to add the question to</param>
    /// <param name="text">The text of the question</param>
    /// <param name="isRequired">Whether question is required or not</param>
    /// <returns>The created question object</returns>
    Task<TextQuestion> AddTextQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired);

    /// <summary>
    /// Asynchrnously creates an answer for a logical question
    /// </summary>
    /// <param name="userId">The id of the user answering the question</param>
    /// <param name="entryId">The id of the entry to add the answer to</param>
    /// <param name="questionId">The id of the question</param>
    /// <param name="answer">The answer of the question</param>
    /// <returns>The created answer object</returns>
    Task<LogicalQuestionAnswer> AnswerLogicalQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        bool? answer);

    /// <summary>
    /// Asynchrnously creates an answer for a multiple choice question
    /// </summary>
    /// <param name="userId">The id of the user answering the question</param>
    /// <param name="entryId">The id of the entry to add the answer to</param>
    /// <param name="questionId">The id of the question</param>
    /// <param name="answerItemsIds">The ids of the selected answers</param>
    /// <returns>The created answer object</returns>
    Task<MultipleChoiceQuestionAnswer> AnswerMultipleChoiceQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        params long[] answerItemsIds);

    /// <summary>
    /// Asynchrnously creates an answer for a text question
    /// </summary>
    /// <param name="userId">The id of the user answering the question</param>
    /// <param name="entryId">The id of the entry to add the answer to</param>
    /// <param name="questionId">The id of the question</param>
    /// <param name="answer">The answer of the question</param>
    /// <returns>The created answer object</returns>
    Task<TextQuestionAnswer> AnswerTextQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        string? answer);

    /// <summary>
    /// Asynchronously creates a new survey
    /// </summary>
    /// <param name="userId">The id of the user creating the survey</param>
    /// <param name="name">The name of the survey</param>
    /// <param name="expiresAt">The expiration date of the survey</param>
    /// <returns>The created survey object</returns>
    Task<Survey> CreateSurveyAsync(long userId, string name, DateTime expiresAt);

    /// <summary>
    /// Asynchronsously creates a survey entry for a user
    /// </summary>
    /// <param name="userId">The id of the uesr creating the entry</param>
    /// <param name="surveyId">The id of the survey to create the entry for</param>
    /// <returns>The created survey object</returns>
    Task<SurveyEntry> CreateSurveyEntryAsync(long userId, long surveyId);

    /// <summary>
    /// Asynchrnously gets availble surveys for a specific user
    /// </summary>
    /// <param name="userId">The id of the user to get surveys for</param>
    /// <returns>A query builder with the matching surveys for the user</returns>
    Task<IQueryBuilder<Survey>> GetAvailableSurveysAsync(long userId);

    /// <summary>
    /// Asynchronously gets a survey by id
    /// </summary>
    /// <param name="userId">the id of the user who wants to access the survey</param>
    /// <param name="surveyId">The id of the survey to acess</param>
    /// <returns>A query builder to the relevant survey</returns>
    Task<IQueryBuilder<Survey>> GetSurveyAsync(long userId, long surveyId);

    /// <summary>
    /// Gets survey entries for a user
    /// </summary>
    /// <param name="userId">The user which to get the entries for</param>
    /// <returns>A query builder for the survey entries of the user</returns>
    IQueryBuilder<SurveyEntry> GetSurveyEntries(long userId);

    /// <summary>
    /// Asynchronously checks whether an entry is complete or not
    /// </summary>
    /// <param name="userId">The id of the user owning the entry</param>
    /// <param name="surveyEntryId">The id of the entry to check its completence</param>
    /// <returns>True if the entry is complete, false otherwise</returns>
    Task<bool> IsEntryComplete(long userId, long surveyEntryId);

    /// <summary>
    /// Asynchrnously removes a survey
    /// </summary>
    /// <param name="userId">The id of the user removing the survey</param>
    /// <param name="surveyId">The id of the servey to remove</param>
    /// <returns></returns>
    Task RemoveSurveyAsync(long userId, long surveyId);

    /// <summary>
    /// Asynchrnously sets the active status of a survey
    /// </summary>
    /// <param name="userId">The id of the user setting the survey active status</param>
    /// <param name="surveyId">The id of the survey to change its active status</param>
    /// <param name="activeStatus">The new active status</param>
    /// <returns>The updated survey object</returns>
    Task<Survey> SetSurveyActiveStatusAsync(long userId, long surveyId, bool activeStatus);

    /// <summary>
    /// Asynchrnously updates a survey
    /// </summary>
    /// <param name="userId">The id of the user updating the survey</param>
    /// <param name="survey">The updated version of the survey</param>
    /// <returns>The updated survey object</returns>
    Task<Survey> UpdateSurveyAsync(long userId, Survey survey);

    #endregion Public Methods
}