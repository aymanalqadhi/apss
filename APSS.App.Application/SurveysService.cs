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
    private readonly IUsersService _usersSvc;

    #endregion Fields

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">The unit of work of the application</param>
    /// <param name="permissionsSvc">The application permissions management service</param>
    /// <param name="usersSvc">Users managment service</param>
    public SurveysService(IUnitOfWork uow, IPermissionsService permissionsSvc, IUsersService usersSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
        _usersSvc = usersSvc;
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
        return AddQuestionAsync(_uow.LogicalQuestions, userId, surveyId, text, isRequired);
    }

    /// <inheritdoc/>
    public async Task<MultipleChoiceQuestion> AddMultipleChoiceQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired,
        bool canMultiSelect,
        params string[] candidateAnswers)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var question = await AddQuestionAsync(_uow.MultipleChoiceQuestions, userId, surveyId, text, isRequired);
        var answers = candidateAnswers.Select(a => new MultipleChoiceAnswerItem
        {
            Value = a,
        }).ToArray();

        _uow.MultipleChoiceAnswerItems.Add(answers);
        question.CandidateAnswers = answers;
        question.CanMultiSelect = canMultiSelect;
        _uow.MultipleChoiceQuestions.Update(question);

        await _uow.CommitAsync(tx);

        return question;
    }

    /// <inheritdoc/>
    public Task<TextQuestion> AddTextQuestionAsync(
        long userId,
        long surveyId,
        string text,
        bool isRequired)
    {
        return AddQuestionAsync(_uow.TextQuestions, userId, surveyId, text, isRequired);
    }

    /// <inheritdoc/>
    public async Task<TQuesiton> AddQuestionAsync<TQuesiton>(
        IRepository<TQuesiton> repo,
        long userId,
        long surveyId,
        string text,
        bool isRequired) where TQuesiton : Question, new()
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(userId, surveyId, PermissionType.Create);
        var question = new TQuesiton
        {
            Text = text,
            IsRequired = isRequired,
            Survey = survey,
        };

        repo.Add(question);
        survey.Questions.Add(question);
        await _uow.CommitAsync();

        return question;
    }

    /// <inheritdoc/>
    public async Task<LogicalQuestionAnswer> AnswerLogicalQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        bool? answer)
    {
        var entry = await GetSurveyEntries(userId)
            .Include(e => e.Survey)
            .FindAsync(entryId);

        var question = await _uow.LogicalQuestions.Query()
            .Where(q => q.Survey.Id == entry.Survey.Id)
            .FindAsync(questionId);

        if (question.IsRequired && answer is null)
        {
            throw new InvalidLogicalQuestionAnswerException(
                questionId,
                answer,
                $"user #{userId} has tried to answer a required logical question #{questionId} with a null value");
        }

        var answerObj = await _uow.LogicalQuestionAnswers.Query()
            .Where(a => a.Question.Id == questionId)
            .FirstOrNullAsync();

        if (answerObj is not null)
        {
            answerObj.Answer = answer;
            _uow.LogicalQuestionAnswers.Update(answerObj);
        }
        else
        {
            answerObj = new LogicalQuestionAnswer
            {
                Question = question,
                Answer = answer,
            };

            entry.Answers.Add(answerObj);

            _uow.LogicalQuestionAnswers.Add(answerObj);
            _uow.SurveyEntries.Update(entry);
        }

        await _uow.CommitAsync();

        return answerObj;
    }

    /// <inheritdoc/>
    public async Task<MultipleChoiceQuestionAnswer> AnswerMultipleChoiceQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        params long[] answerItemsIds)
    {
        var entry = await GetSurveyEntries(userId)
            .Include(e => e.Survey)
            .FindAsync(entryId);

        var question = await _uow.MultipleChoiceQuestions.Query()
            .Where(q => q.Survey.Id == entry.Survey.Id)
            .FindAsync(questionId);

        var answerObj = await _uow.MultipleChoiceQuestionAnswers.Query()
            .Where(a => a.Question.Id == questionId)
            .FirstOrNullAsync();

        var answerItems = await Task.WhenAll(answerItemsIds
            .Select(i => _uow.MultipleChoiceAnswerItems.Query().FindAsync(i)));

        if (question.IsRequired && answerItems.Length == 0)
        {
            throw new InvalidMultipleChoiceQuestionAnswerException(
                questionId,
                answerItems.Select(i => i.Value),
                $"user #{userId} has tried to answer a required multiple choice question #{questionId} with an empty value");
        }
        else if (!question.CanMultiSelect && answerItems.Length > 1)
        {
            throw new InvalidMultipleChoiceQuestionAnswerException(
                questionId,
                answerItems.Select(i => i.Value),
                $"user #{userId} has tried to answer a multiple-choice question #{questionId} (no multi-select) with multiple values");
        }

        if (answerObj is not null)
        {
            answerObj.Answers = answerItems;
            _uow.MultipleChoiceQuestionAnswers.Update(answerObj);
        }
        else
        {
            answerObj = new MultipleChoiceQuestionAnswer
            {
                Question = question,
                Answers = answerItems,
            };

            entry.Answers.Add(answerObj);

            _uow.MultipleChoiceQuestionAnswers.Add(answerObj);
            _uow.SurveyEntries.Update(entry);
        }

        await _uow.CommitAsync();

        return answerObj;
    }

    /// <inheritdoc/>
    public async Task<TextQuestionAnswer> AnswerTextQuestionAsync(
        long userId,
        long entryId,
        long questionId,
        string? answer)
    {
        var entry = await GetSurveyEntries(userId)
            .Include(e => e.Survey)
            .FindAsync(entryId);

        var question = await _uow.TextQuestions.Query()
            .Where(q => q.Survey.Id == entry.Survey.Id)
            .FindAsync(questionId);

        if (question.IsRequired && answer is null)
        {
            throw new InvalidTextQuestionAnswerException(
                questionId,
                answer,
                $"user #{userId} has tried to answer a required text question #{questionId} with a null value");
        }

        var answerObj = await _uow.TextQuestionAnswers.Query()
            .Where(a => a.Question.Id == questionId)
            .FirstOrNullAsync();

        if (answerObj is not null)
        {
            answerObj.Answer = answer;
            _uow.TextQuestionAnswers.Update(answerObj);
        }
        else
        {
            answerObj = new TextQuestionAnswer
            {
                Question = question,
                Answer = answer,
            };

            entry.Answers.Add(answerObj);

            _uow.TextQuestionAnswers.Add(answerObj);
            _uow.SurveyEntries.Update(entry);
        }

        await _uow.CommitAsync();

        return answerObj;
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
        var survey = await (await GetAvailableSurveysAsync(userId)).FindAsync(surveyId);

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
    public async Task<IQueryBuilder<Survey>> GetAvailableSurveysAsync(long userId)
    {
        var usersHierarchyIds = await _usersSvc
            .GetUserUpwardHierarchyAsync(userId)
            .Select(u => u.Id)
            .ToListAsync();

        return _uow.Surveys.Query()
            .Where(s => s.ExpirationDate > DateTime.Now)
            .Where(s => usersHierarchyIds.Contains(s.CreatedBy.Id));
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