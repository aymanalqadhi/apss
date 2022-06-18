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
    public async Task<TextQuestionAnswer> AnswerTextQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        string? answer)
    {
        //var entry = await GetSurveyEntries(userId).FindAsync(entryId);
        //var question = await _uow..Que
        //    .Where(t => t.)
    }

    /// <inheritdoc/>
    public async Task<Survey> CreateSurveyAsync(long userId, string name, DateTime expiresAt)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        if (user.AccessLevel == AccessLevel.Farmer)
            throw new InsufficientPermissionsException(userId, $"farmer #{userId} cannot add surveys");

        var survey = new Survey
        {
            Name = name,
            ExpirationDate = expiresAt,
        };

        _uow.Surveys.Add(survey);
        await _uow.CommitAsync();

        return survey;
    }

    /// <inheritdoc/>
    public async Task<SurveyEntry> CreateSurveyEntryAsync(long userId, long surveyId)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var survey = await GetAvailableSurveys(userId).FindAsync(surveyId);

        var entry = new SurveyEntry
        {
            MadeBy = user,
            Survey = survey,
        };

        _uow.SurveyEntries.Add(entry);
        await _uow.CommitAsync();

        return entry;
    }

    /// <inheritdoc/>
    public IQueryBuilder<Survey> GetAvailableSurveys(long userId)
    {
        return _uow.Surveys
            .Query()
            .Where(s => s.CreatedBy.Id == userId)
            .Where(s => s.ExpirationDate > DateTime.Now);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Survey>> GetSurveyAsync(long userId, long surveyId)
    {
        await GetSurveyWithAuthorizationAsync(userId, surveyId, PermissionType.Read);

        return _uow.Surveys
            .Query()
            .Where(s => s.CreatedBy.Id == userId);
    }

    /// <inheritdoc/>
    public IQueryBuilder<SurveyEntry> GetSurveyEntries(long userId)
        => _uow.SurveyEntries.Query().Where(e => e.MadeBy.Id == userId);

    /// <inheritdoc/>
    public Task<bool> IsEntryComplete(long userId, long surveyEntryId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task RemoveSurveyAsync(long userId, long surveyId)
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(userId, surveyId, PermissionType.Delete);

        _uow.Surveys.Remove(survey);
        await _uow.CommitAsync();
    }

    /// <inheritdoc/>
    public async Task<Survey> SetSurveyActiveStatusAsync(long userId, long surveyId, bool activeStatus)
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(userId, surveyId, PermissionType.Update);

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

    #region Private Methods

    private async Task<(Survey, long)> GetSurveyWithAuthorizationAsync(long userId, long surveyId, PermissionType permissions)
    {
        var survey = await _uow.Surveys
            .Query()
            .Include(s => s.CreatedBy)
            .FindAsync(surveyId);

        var id = await _permissionsSvc.ValidatePermissionsAsync(userId, survey.CreatedBy.Id, survey, permissions);

        return (survey, id);
    }

    #endregion Private Methods
}