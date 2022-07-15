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

    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;
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
        long accountId,
        long surveyId,
        string text,
        bool isRequired)
    {
        return AddQuestionAsync(_uow.LogicalQuestions, accountId, surveyId, text, isRequired);
    }

    /// <inheritdoc/>
    public async Task<MultipleChoiceQuestion> AddMultipleChoiceQuestionAsync(
        long accountId,
        long surveyId,
        string text,
        bool isRequired,
        bool canMultiSelect,
        params string[] candidateAnswers)
    {
        await using var tx = await _uow.BeginTransactionAsync();

        var answers = candidateAnswers.Select(a => new MultipleChoiceAnswerItem
        {
            Value = a,
        }).ToArray();

        _uow.MultipleChoiceAnswerItems.Add(answers);

        var question = await AddQuestionAsync(
            _uow.MultipleChoiceQuestions,
            accountId,
            surveyId,
            text,
            isRequired,
            q =>
            {
                q.CandidateAnswers = answers;
                q.CanMultiSelect = canMultiSelect;
            });

        await _uow.CommitAsync(tx);

        return question;
    }

    /// <inheritdoc/>
    public Task<TextQuestion> AddTextQuestionAsync(
        long accountId,
        long surveyId,
        string text,
        bool isRequired)
    {
        return AddQuestionAsync(_uow.TextQuestions, accountId, surveyId, text, isRequired);
    }

    /// <inheritdoc/>
    public async Task<LogicalQuestionAnswer> AnswerLogicalQuestionAsync(
        long accountId,
        long entryId,
        long questionId,
        bool? answer)
    {
        var (account, entry) = await GetAnswerEntryAsync(accountId, entryId);

        var question = await _uow.LogicalQuestions.Query()
            .Where(q => q.Survey.Id == entry.Survey.Id)
            .FindAsync(questionId);

        if (question.IsRequired && answer is null)
        {
            throw new InvalidLogicalQuestionAnswerException(
                questionId,
                answer,
                $"user #{account.User.Id} with account #{accountId} has tried to answer a required logical question #{questionId} of survey #{entry.Survey.Id} on entry #{entry.Id} with a null value");
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
        long accountId,
        long entryId,
        long questionId,
        params long[] answerItemsIds)
    {
        var (account, entry) = await GetAnswerEntryAsync(accountId, entryId);

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
                $"user #{account.User.Id} with account #{accountId} has tried to answer a required multiple choice question #{questionId} of survey #{entry.Survey.Id} on entry #{entry.Id} with an empty value");
        }
        else if (!question.CanMultiSelect && answerItems.Length > 1)
        {
            throw new InvalidMultipleChoiceQuestionAnswerException(
                questionId,
                answerItems.Select(i => i.Value),
                $"user #{account.User.Id} with account #{accountId} has tried to answer a multiple-choice question #{questionId} (no multi-select) of survey #{entry.Survey.Id} on entry #{entry.Id} with multiple values");
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
        long accountId,
        long entryId,
        long questionId,
        string? answer)
    {
        var (account, entry) = await GetAnswerEntryAsync(accountId, entryId);

        var question = await _uow.TextQuestions.Query()
            .Where(q => q.Survey.Id == entry.Survey.Id)
            .FindAsync(questionId);

        if (question.IsRequired && answer is null)
        {
            throw new InvalidTextQuestionAnswerException(
                questionId,
                answer,
                $"user #{account.User.Id} with account #{accountId} has tried to answer a required text question #{questionId} of survey #{entry.Survey.Id} on entry #{entry.Id} with a null value");
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
    public async Task<Survey> CreateSurveyAsync(long accountId, string name, DateTime expiresAt)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Create);

        if (account.User.AccessLevel == AccessLevel.Farmer)
            throw new InsufficientPermissionsException(accountId, $"farmer #{account.User.Id} with account #{accountId} cannot add surveys");

        var survey = new Survey
        {
            CreatedBy = account.User,
            Name = name,
            ExpirationDate = expiresAt,
        };

        _uow.Surveys.Add(survey);
        await _uow.CommitAsync();

        return survey;
    }

    /// <inheritdoc/>
    public async Task<SurveyEntry> CreateSurveyEntryAsync(long accountId, long surveyId)
    {
        var account = await _uow.Accounts.Query()
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read | PermissionType.Create);

        var survey = await (await DoGetAvailableSurveysAsync(account.Id)).FindAsync(surveyId);

        var entry = new SurveyEntry
        {
            MadeBy = account.User,
            Survey = survey,
        };

        _uow.SurveyEntries.Add(entry);
        await _uow.CommitAsync();

        return entry;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Survey>> GetAvailableSurveysAsync(long accountId)
    {
        var account = await _uow.Accounts.Query()
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        return await DoGetAvailableSurveysAsync(account.Id);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Survey>> GetSurveyAsync(long accountId, long surveyId)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        return _uow.Surveys
            .Query()
            .Where(s => s.Id == surveyId && s.CreatedBy.Id == account.User.Id);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<SurveyEntry>> GetSurveyEntriesAsync(long accountId)
    {
        var account = await _uow.Accounts.Query()
            .Include(a => a.User)
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Read);

        return _uow.SurveyEntries.Query().Where(e => e.MadeBy.Id == account.User.Id);
    }

    /// <inheritdoc/>
    public Task<bool> IsEntryComplete(long accountId, long surveyEntryId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task RemoveSurveyAsync(long accountId, long surveyId)
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(accountId, surveyId, PermissionType.Delete);

        _uow.Surveys.Remove(survey);
        await _uow.CommitAsync();
    }

    /// <inheritdoc/>
    public async Task<Survey> SetSurveyActiveStatusAsync(long accountId, long surveyId, bool activeStatus)
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(accountId, surveyId, PermissionType.Update);

        survey.IsActive = activeStatus;

        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        return survey;
    }

    /// <inheritdoc/>
    public async Task<Survey> UpdateSurveyAsync(long accountId, Survey survey)
    {
        await _permissionsSvc.ValidatePermissionsAsync(accountId, survey.CreatedBy.Id, PermissionType.Update);

        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        return survey;
    }

    #endregion Public Methods

    #region Private Methods

    /// <inheritdoc/>
    private async Task<TQuesiton> AddQuestionAsync<TQuesiton>(
        IRepository<TQuesiton> repo,
        long accountId,
        long surveyId,
        string text,
        bool isRequired,
        Action<TQuesiton>? builder = null) where TQuesiton : Question, new()
    {
        var (survey, _) = await GetSurveyWithAuthorizationAsync(
            accountId,
            surveyId,
            PermissionType.Create | PermissionType.Update);

        var question = new TQuesiton
        {
            Text = text,
            IsRequired = isRequired,
            Survey = survey,
        };

        builder?.Invoke(question);

        repo.Add(question);
        survey.Questions.Add(question);
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        return question;
    }

    private async Task<IQueryBuilder<Survey>> DoGetAvailableSurveysAsync(long accountId)
    {
        var usersHierarchyIds = await _usersSvc
            .GetUpwardHierarchyAsync(accountId)
            .Select(u => u.Id)
            .ToListAsync();

        return _uow.Surveys.Query()
            .Where(s => s.ExpirationDate > DateTime.Now && usersHierarchyIds.Contains(s.CreatedBy.Id));
    }

    private async Task<(Account, SurveyEntry)> GetAnswerEntryAsync(long accountId, long entryId)
    {
        var entry = await _uow.SurveyEntries.Query()
            .Include(e => e.MadeBy)
            .Include(e => e.Survey)
            .FindAsync(entryId);

        var account = await _permissionsSvc.ValidatePermissionsAsync(
            accountId,
            entry.MadeBy.Id,
            PermissionType.Read | PermissionType.Update | PermissionType.Create);

        return (account, entry);
    }

    private async Task<(Survey, Account)> GetSurveyWithAuthorizationAsync(long accountId, long surveyId, PermissionType permissions)
    {
        var survey = await _uow.Surveys
            .Query()
            .Include(s => s.CreatedBy)
            .FindAsync(surveyId);

        var account = await _permissionsSvc.ValidatePermissionsAsync(accountId, survey.CreatedBy.Id, permissions);

        return (survey, account);
    }

    #endregion Private Methods
}