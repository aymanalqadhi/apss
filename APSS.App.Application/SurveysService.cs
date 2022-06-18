using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

/// <summary>
/// An service to manage surveys on the application
/// </summary>
public sealed class SurveysService : ISurveysService
{
    #region Fields

    private readonly IUnitOfWork _uow;
    private readonly IPermissionsService _permissionsSvc;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">The unit of work of the application</param>
    /// <param name="permissionsSvc">The application permissions management service</param>
    public SurveysService(IUnitOfWork uow, IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public Task<LogicalQuestion> AddLogicalQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MultipleChoiceQuestion> AddMultipleChoiceQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired,
        bool canMultiSelect,
        params string[] candidateAnswers)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<TextQuestion> AddTextQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<LogicalQuestionAnswer> AnswerLogicalQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        bool? answer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MultipleChoiceQuestionAnswer> AnswerMultipleChoiceQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        params long[] answerItemsIds)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<TextQuestionAnswer> AnswerTextQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        string? answer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<Survey> CreateSurveyAsync(long userId, string name, DateTime expiresAt)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<SurveyEntry> CreateSurveyEntryAsync(long userId, long surveyId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IQueryBuilder<Survey> GetAvailableSurveys(long userId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<IQueryBuilder<Survey>> GetSurveyAsync(long userId, long surveyId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IQueryBuilder<SurveyEntry> GetSurveyEntries(long userId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<bool> IsEntryComplete(long userId, long surveyEntryId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task RemoveSurveyAsync(long userId, long surveyId)
    {
        var survey = await _uow.Surveys
            .Query()
            .Include(s => s.CreatedBy)
            .FindAsync(surveyId);

        await _permissionsSvc.ValidatePermissionsAsync(userId, survey.CreatedBy.Id, survey, PermissionType.Delete);

        _uow.Surveys.Remove(survey);
        await _uow.CommitAsync();
    }

    /// <inheritdoc/>
    public async Task<Survey> SetSurveyActiveStatusAsync(long userId, long surveyId, bool activeStatus)
    {
        var survey = await _uow.Surveys
            .Query()
            .Include(s => s.CreatedBy)
            .FindAsync(surveyId);

        await _permissionsSvc.ValidatePermissionsAsync(userId, survey.CreatedBy.Id, survey, PermissionType.Update);

        survey.IsActive = activeStatus;

        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        return survey;
    }

    /// <inheritdoc/>
    public async Task<Survey> UpdateSurveyAsync(long userId, Survey survey)
    {
        await _permissionsSvc.ValidatePermissionsAsync(userId, survey.CreatedBy.Id, survey, PermissionType.Update);

        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        return survey;
    }

    #endregion Public Methods
}