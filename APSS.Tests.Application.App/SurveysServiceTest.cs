using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace APSS.Tests.Application.App;

public sealed class SurveysServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly ISurveysService _surveySvc;
    private readonly IPermissionsService _permissionsSvc;

    #endregion Private fields

    #region Constructors

    public SurveysServiceTest(
        IUnitOfWork uow,
        ISurveysService surveySvc,
        IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _surveySvc = surveySvc;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Constructors

    #region Tests

    [Theory]
    [InlineData(AccessLevel.Farmer, PermissionType.Create | PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create, true)]
    [InlineData(AccessLevel.Group, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, true)]
    [InlineData(AccessLevel.Village, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, true)]
    [InlineData(AccessLevel.District, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, true)]
    [InlineData(AccessLevel.Directorate, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, true)]
    [InlineData(AccessLevel.Governorate, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, true)]
    [InlineData(AccessLevel.Presedint, PermissionType.Delete | PermissionType.Read | PermissionType.Update, false)]
    public async Task<(Account, Survey?)> SurveyAddedTheory(
        AccessLevel accessLevel = AccessLevel.Group,
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissions);

        var templateSurvey = ValidEntitiesFactory.CreateValidSurvey(TimeSpan.FromHours(3));

        var addSurveyTask = _surveySvc.CreateSurveyAsync(
            account.Id,
            templateSurvey.Name,
            templateSurvey.ExpirationDate);

        if (!shouldSucceed)
        {
            if (accessLevel == AccessLevel.Farmer)
            {
                await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await addSurveyTask);
                return (account, null);
            }
            else if (permissions != PermissionType.Create)
            {
                await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await addSurveyTask);
                return (account, null);
            }
        }

        var survey = await addSurveyTask;

        Assert.True(await _uow.Surveys.Query().ContainsAsync(survey));
        Assert.Equal(account.User.Id, survey.CreatedBy.Id);
        Assert.Equal(templateSurvey.Name, survey.Name);
        Assert.Equal(templateSurvey.ExpirationDate, survey.ExpirationDate);

        return (account, survey);
    }

    [Theory]
    [InlineData(PermissionType.Read | PermissionType.Create, true)]
    [InlineData(PermissionType.Delete | PermissionType.Update, false)]
    public async Task<(Account, SurveyEntry?)> SurveyEntryAddedTheory(
        PermissionType permissions = PermissionType.Read | PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, permissions);
        var (surveyAccount, survey) = await SurveyAddedTheory(RandomGenerator.NextAccessLevel(min: AccessLevel.Group));

        var accountUser = account.User;

        while (accountUser.AccessLevel != surveyAccount.User.AccessLevel.NextLevelBelow())
            accountUser = accountUser.SupervisedBy!;

        accountUser.SupervisedBy = surveyAccount.User;
        await _uow.CommitAsync();

        Assert.Equal(surveyAccount.User.Id, accountUser.SupervisedBy!.Id);

        var templateEntry = new SurveyEntry
        {
            MadeBy = account.User,
            Survey = survey!,
        };

        var addEntryTask = _surveySvc.CreateSurveyEntryAsync(
            account.Id,
            templateEntry.Survey.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await addEntryTask);
            return (account, null);
        }

        var entry = await addEntryTask;

        Assert.True(await _uow.SurveyEntries.Query().ContainsAsync(entry));
        Assert.Equal(account.User.Id, entry.MadeBy.Id);
        Assert.Equal(survey!.Id, entry.Survey.Id);
        Assert.Equal(surveyAccount.User.Id, entry.Survey.CreatedBy.Id);

        return (account, entry);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create, true)]
    [InlineData(PermissionType.Read | PermissionType.Delete, false)]
    public async Task<(Account, LogicalQuestion?)> LogicalQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var surveyAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group),
            permissions);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var templateQuestion = ValidEntitiesFactory.CreateValidLogicalQuestion(true);

        var validAddLogicalQuestionTask = _surveySvc.AddLogicalQuestionAsync(
            surveyAccount.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validAddLogicalQuestionTask);
            return (surveyAccount, null);
        }

        var validQuestion = await validAddLogicalQuestionTask;

        Assert.True(await _uow.LogicalQuestions.Query().ContainsAsync(validQuestion));
        Assert.Equal(surveyAccount.User.Id, validQuestion.Survey.CreatedBy.Id);
        Assert.Equal(survey.Id, validQuestion.Survey.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Create | PermissionType.Update);

        var invalidAddLogicalQuestionTask = _surveySvc.AddLogicalQuestionAsync(
           account.Id,
           survey!.Id,
           templateQuestion.Text,
           templateQuestion.IsRequired);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidAddLogicalQuestionTask);

        return (surveyAccount, validQuestion);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create, true)]
    [InlineData(PermissionType.Read | PermissionType.Delete, false)]
    public async Task<(Account, MultipleChoiceQuestion?)> MultipleChoiceQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var surveyAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group),
            permissions);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var templateQuestion = ValidEntitiesFactory.CreateValidMultipleChoiceQuestion(true);

        var candidateAnswers = Enumerable
            .Range(3, RandomGenerator.NextInt(5, 10))
            .Select(_ => RandomGenerator.NextString(30))
            .ToArray();

        var validQuestionTask = _surveySvc.AddMultipleChoiceQuestionAsync(
            surveyAccount.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired,
            templateQuestion.CanMultiSelect,
            candidateAnswers);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validQuestionTask);
            return (surveyAccount, null);
        }

        var validQuestion = await validQuestionTask;

        Assert.True(await _uow.MultipleChoiceQuestions.Query().ContainsAsync(validQuestion));
        Assert.Equal(surveyAccount.User.Id, validQuestion.Survey.CreatedBy.Id);
        Assert.Equal(survey.Id, validQuestion.Survey.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Update | PermissionType.Create);

        var invalidQuestionTask = _surveySvc.AddMultipleChoiceQuestionAsync(
          account.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired,
            templateQuestion.CanMultiSelect,
            candidateAnswers);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidQuestionTask);

        return (surveyAccount, validQuestion);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create, true)]
    [InlineData(PermissionType.Read | PermissionType.Delete, false)]
    public async Task<(Account, TextQuestion?)> TextQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var surveyAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group),
            permissions);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var templateQuestion = ValidEntitiesFactory.CreateValidTextQuestion(true);

        var validQuestionTask = _surveySvc.AddTextQuestionAsync(
            surveyAccount.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validQuestionTask);
            return (surveyAccount, null);
        }

        var validQuestion = await validQuestionTask;

        Assert.True(await _uow.TextQuestions.Query().ContainsAsync(validQuestion));
        Assert.Equal(surveyAccount.User.Id, validQuestion.Survey.CreatedBy.Id);
        Assert.Equal(survey.Id, validQuestion.Survey.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Create | PermissionType.Update);

        var invalidQuestionTask = _surveySvc.AddTextQuestionAsync(
            account.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidQuestionTask);

        return (surveyAccount, validQuestion);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create | PermissionType.Read, true)]
    [InlineData(PermissionType.Delete, false)]
    public async Task<(Account, LogicalQuestionAnswer?)> AnswerLogicalQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create | PermissionType.Read,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var entryAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            PermissionType.Create | PermissionType.Read);

        var surveyAccount = await _uow.CreateTestingAccountAboveUserAsync(
            entryAccount.User.Id,
            RandomGenerator.NextAccessLevel(min: entryAccount.User.AccessLevel.NextLevelUpove()),
            PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var entry = await _surveySvc.CreateSurveyEntryAsync(
          entryAccount.Id,
          survey!.Id);

        var questionAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            permissions);

        entry.MadeBy = questionAccount.User;
        _uow.SurveyEntries.Update(entry);
        await _uow.CommitAsync();

        var surveyAccount2 = await _uow.CreateTestingAccountAboveUserAsync(
           entry.MadeBy.Id,
           RandomGenerator.NextAccessLevel(min: entry.MadeBy.AccessLevel + 1),
           PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount2.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var templateQuestion = ValidEntitiesFactory.CreateValidLogicalQuestion(true);
        var templateAnswer = ValidEntitiesFactory.CreateValidLogicalQuestionAnswer();

        var question = await _surveySvc.AddLogicalQuestionAsync(
            surveyAccount2.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired);

        var answer = _surveySvc.AnswerLogicalQuestionAsync(
            questionAccount.Id,
            entry.Id,
            question.Id,
            templateAnswer.Answer);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await answer);
            return (questionAccount, null);
        }

        var validAnswer = await answer;

        Assert.True(await _uow.LogicalQuestionAnswers.Query().ContainsAsync(validAnswer));
        Assert.Equal(entry.Survey.Id, validAnswer.Question.Survey.Id);
        Assert.Equal(entry.MadeBy.Id, questionAccount.User.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Create | PermissionType.Read | PermissionType.Update);

        var invalidAnswerTask = _surveySvc.AnswerLogicalQuestionAsync(
           account.Id,
           entry.Id,
           question.Id,
           templateAnswer.Answer);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidAnswerTask);

        return (entryAccount, validAnswer);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create | PermissionType.Read, true)]
    [InlineData(PermissionType.Delete, false)]
    public async Task<(Account, TextQuestionAnswer?)> AnswerTextQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create | PermissionType.Read,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var entryAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            PermissionType.Create | PermissionType.Read);

        var surveyAccount = await _uow.CreateTestingAccountAboveUserAsync(
            entryAccount.User.Id,
            RandomGenerator.NextAccessLevel(min: entryAccount.User.AccessLevel + 1),
            PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var entry = await _surveySvc.CreateSurveyEntryAsync(
          entryAccount.Id,
          survey!.Id);

        var questionAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            permissions);

        entry.MadeBy = questionAccount.User;
        _uow.SurveyEntries.Update(entry);
        await _uow.CommitAsync();

        var surveyAccount2 = await _uow.CreateTestingAccountAboveUserAsync(
           entry.MadeBy.Id,
           RandomGenerator.NextAccessLevel(min: entry.MadeBy.AccessLevel + 1),
           PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount2.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var templateQuestion = ValidEntitiesFactory.CreateValidTextQuestion(true);
        var templateAnswer = ValidEntitiesFactory.CreateValidTextQuestionAnswer();

        var question = await _surveySvc.AddTextQuestionAsync(
            surveyAccount2.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired);

        var answer = _surveySvc.AnswerTextQuestionAsync(
            questionAccount.Id,
            entry.Id,
            question.Id,
            templateAnswer.Answer);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await answer);
            return (questionAccount, null);
        }

        var validAnswer = await answer;

        Assert.True(await _uow.TextQuestionAnswers.Query().ContainsAsync(validAnswer));
        Assert.Equal(entry.Survey.Id, validAnswer.Question.Survey.Id);
        Assert.Equal(entry.MadeBy.Id, questionAccount.User.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Create | PermissionType.Read | PermissionType.Update);

        var invalidAnswerTask = _surveySvc.AnswerTextQuestionAsync(
           account.Id,
           entry.Id,
           question.Id,
           templateAnswer.Answer);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidAnswerTask);

        return (entryAccount, validAnswer);
    }

    [Theory]
    [InlineData(PermissionType.Update | PermissionType.Create | PermissionType.Read, true)]
    [InlineData(PermissionType.Delete, false)]
    public async Task<(Account, MultipleChoiceQuestionAnswer?)> AnswerMultipleChoiceQuestionAddedTheory(
        PermissionType permissions = PermissionType.Update | PermissionType.Create | PermissionType.Read,
        bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();

        var entryAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            PermissionType.Create | PermissionType.Read);

        var surveyAccount = await _uow.CreateTestingAccountAboveUserAsync(
            entryAccount.User.Id,
            RandomGenerator.NextAccessLevel(min: entryAccount.User.AccessLevel + 1),
            PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var entry = await _surveySvc.CreateSurveyEntryAsync(
          entryAccount.Id,
          survey!.Id);

        var questionAccount = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Governorate),
            permissions);

        entry.MadeBy = questionAccount.User;
        _uow.SurveyEntries.Update(entry);
        await _uow.CommitAsync();

        var surveyAccount2 = await _uow.CreateTestingAccountAboveUserAsync(
           entry.MadeBy.Id,
           RandomGenerator.NextAccessLevel(min: entry.MadeBy.AccessLevel.NextLevelUpove()),
           PermissionType.Create | PermissionType.Update);

        survey!.CreatedBy = surveyAccount2.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var candidateQuestions = Enumerable
           .Range(3, RandomGenerator.NextInt(5, 10))
           .Select(_ => RandomGenerator.NextString(30))
           .ToArray();

        var templateQuestion = ValidEntitiesFactory.CreateValidMultipleChoiceQuestion(true);

        var question = await _surveySvc.AddMultipleChoiceQuestionAsync(
            surveyAccount2.Id,
            survey!.Id,
            templateQuestion.Text,
            templateQuestion.IsRequired,
            true,
            candidateQuestions);

        var answer = _surveySvc.AnswerMultipleChoiceQuestionAsync(
            questionAccount.Id,
            entry.Id,
            question.Id,
           question.CanMultiSelect? question.CandidateAnswers.Take(RandomGenerator.NextInt(2, candidateQuestions.Length)).Select(a => a.Id).ToArray() :
           question.CandidateAnswers.Take(1).Select(a => a.Id).ToArray());

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await answer);
            return (questionAccount, null);
        }

        var validAnswer = await answer;

        Assert.True(await _uow.MultipleChoiceQuestionAnswers.Query().ContainsAsync(validAnswer));
        Assert.Equal(entry.Survey.Id, validAnswer.Question.Survey.Id);
        Assert.Equal(entry.MadeBy.Id, questionAccount.User.Id);

        var account = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Create | PermissionType.Read | PermissionType.Update);

        var invalidAnswerTask = _surveySvc.AnswerMultipleChoiceQuestionAsync(
           account.Id,
           entry.Id,
           question.Id,
           question.CanMultiSelect ? question.CandidateAnswers.Take(RandomGenerator.NextInt(2, candidateQuestions.Length)).Select(a => a.Id).ToArray() :
           question.CandidateAnswers.Take(1).Select(a => a.Id).ToArray());

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await invalidAnswerTask);

        return (entryAccount, validAnswer);
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
    public async Task<IQueryBuilder<Survey>?> GetAvaliableSurveysTheory(
       PermissionType permissions = PermissionType.Read ,
       bool shouldSucceed = true)
    {
        var (_, parentSurvey) = await SurveyAddedTheory();
        var (_, anotherParentSurvey) = await SurveyAddedTheory();
        var (_, notParentSurvey) = await SurveyAddedTheory();


        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Directorate),
            permissions);

        var parentAccount = await _uow.CreateTestingAccountAboveUserAsync(
           account.User.Id,
           RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove(), max: AccessLevel.Governorate),
           PermissionType.Create | PermissionType.Update);

        var anotherParentAccount = await _uow.CreateTestingAccountAboveUserAsync(
           parentAccount.User.Id,
           RandomGenerator.NextAccessLevel(min: parentAccount.User.AccessLevel.NextLevelUpove()),
           PermissionType.Create | PermissionType.Update);
       
        var notParentAccount = await _uow.CreateTestingAccountAsync(
           RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove()),
           PermissionType.Create | PermissionType.Update);

        parentSurvey!.CreatedBy = parentAccount.User;
        _uow.Surveys.Update(parentSurvey);
        anotherParentSurvey!.CreatedBy = anotherParentAccount.User;
        _uow.Surveys.Update(anotherParentSurvey);
        notParentSurvey!.CreatedBy = notParentAccount.User;
        _uow.Surveys.Update(notParentSurvey);
        await _uow.CommitAsync();


        var surveysTask = _surveySvc.GetAvailableSurveysAsync(account.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await surveysTask );
            return null;
        }

        var surveys = await surveysTask;

        Assert.True(await surveys.ContainsAsync(parentSurvey));
        Assert.True(await surveys.ContainsAsync(anotherParentSurvey));
        Assert.False(await surveys.ContainsAsync(notParentSurvey));
        
        return surveys;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
    public async Task<IQueryBuilder<Survey>?> GetSurveysTheory(
       PermissionType permissions = PermissionType.Read,
       bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();
        var (_, secondSurvey) = await SurveyAddedTheory();

        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group, max: AccessLevel.Presedint),
            permissions );
       
        survey!.CreatedBy = account.User;
        _uow.Surveys.Update(survey);
        await _uow.CommitAsync();

        var surveysTask = _surveySvc.GetSurveyAsync(account.Id, survey.Id);
        var secondSurveysTask = _surveySvc.GetSurveyAsync(account.Id, secondSurvey!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await surveysTask);
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await secondSurveysTask);
            return null;
        }

        var surveys = await surveysTask;
        var secondSurveys = await secondSurveysTask;
       
        await foreach (var s in surveys.AsAsyncEnumerable())
         Assert.Equal(s.Id, survey.Id);

        await foreach (var s in secondSurveys.AsAsyncEnumerable())
            Assert.Null(s);


        return surveys;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
    public async Task<IQueryBuilder<SurveyEntry>?> GetSurveyEntriesTheory(
       PermissionType permissions = PermissionType.Read,
       bool shouldSucceed = true)
    {
        var (_, surveyEntry) = await SurveyEntryAddedTheory();
        var (_, secondSurveyEntry) = await SurveyEntryAddedTheory();
        var (_, falseSurveyEntry) = await SurveyEntryAddedTheory();



        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group),
            permissions);

        surveyEntry!.MadeBy = account.User;
        _uow.SurveyEntries.Update(surveyEntry);
        secondSurveyEntry!.MadeBy = account.User;
        _uow.SurveyEntries.Update(secondSurveyEntry);
        await _uow.CommitAsync();


        var entriesTask = _surveySvc.GetSurveyEntriesAsync(account.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await entriesTask);
            return null;
        }

        var entries = await entriesTask;

        await foreach (var e in entries.AsAsyncEnumerable())
            Assert.Equal(e.MadeBy.Id, account.User.Id);

        Assert.True(await entries.ContainsAsync(surveyEntry));
        Assert.True(await entries.ContainsAsync(secondSurveyEntry));
        Assert.False(await entries.ContainsAsync(falseSurveyEntry!));

        return entries;
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Update | PermissionType.Read | PermissionType.Create, false)]
    public async Task RemoveSurveyTheory(
       PermissionType permissions = PermissionType.Delete,
       bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();
        var (_, secondSurvey) = await SurveyAddedTheory();
        var (_, fourthSurvey) = await SurveyAddedTheory();

        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group, max: AccessLevel.Governorate),
            permissions);

        var parentAccount = await _uow.CreateTestingAccountAboveUserAsync(
           account.User.Id,
           account.User.AccessLevel.NextLevelUpove(),
           permissions);

        var notParentAccount = await _uow.CreateTestingAccountAsync(
             RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove(), max: AccessLevel.Presedint),
            PermissionType.Delete);

        await _uow.CommitAsync();

        survey!.CreatedBy = account.User;
        secondSurvey!.CreatedBy = account.User;
        fourthSurvey!.CreatedBy = account.User;
        _uow.Surveys.Update(survey!);
        _uow.Surveys.Update(secondSurvey!);
        _uow.Surveys.Update(fourthSurvey!);
        await _uow.CommitAsync();

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => 
                    await _surveySvc.RemoveSurveyAsync(account.Id, survey!.Id));
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () =>
                    await _surveySvc.RemoveSurveyAsync(parentAccount.Id, secondSurvey!.Id));

            return;
        }

        Assert.True(await _uow.Surveys.Query().ContainsAsync(survey!));

        await _surveySvc.RemoveSurveyAsync(account.Id, survey!.Id);

        Assert.False(await _uow.Surveys.Query().ContainsAsync(survey!));


        Assert.True(await _uow.Surveys.Query().ContainsAsync(secondSurvey!));

        await _surveySvc.RemoveSurveyAsync(parentAccount.Id, secondSurvey!.Id);

        Assert.False(await _uow.Surveys.Query().ContainsAsync(secondSurvey!));


        Assert.True(await _uow.Surveys.Query().ContainsAsync(fourthSurvey!));

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () =>
                    await _surveySvc.RemoveSurveyAsync(notParentAccount.Id, fourthSurvey!.Id));

        Assert.True(await _uow.Surveys.Query().ContainsAsync(fourthSurvey!));


        return;
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Delete | PermissionType.Read | PermissionType.Create, false)]
    public async Task<Survey?> SetSurveyActiveStatusTheory(
      PermissionType permissions = PermissionType.Delete,
      bool shouldSucceed = true)
    {
        var (_, survey) = await SurveyAddedTheory();
        var (_, secondSurvey) = await SurveyAddedTheory();
        var (_, thirdSurvey) = await SurveyAddedTheory();

        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(min: AccessLevel.Group, max: AccessLevel.Governorate),
            permissions);

        var parentAccount = await _uow.CreateTestingAccountAboveUserAsync(
           account.User.Id,
           account.User.AccessLevel.NextLevelUpove(),
           permissions);

        var notParentAccount = await _uow.CreateTestingAccountAsync(
             RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove(), max: AccessLevel.Presedint),
            PermissionType.Delete);

        await _uow.CommitAsync();

        survey!.CreatedBy = account.User;
        survey!.IsActive = true;
        secondSurvey!.CreatedBy = account.User;
        secondSurvey!.IsActive = false;
        thirdSurvey!.CreatedBy = account.User;
        thirdSurvey!.IsActive = false;
        _uow.Surveys.Update(survey!);
        _uow.Surveys.Update(secondSurvey!);
        _uow.Surveys.Update(thirdSurvey!);
        await _uow.CommitAsync();

        Assert.True(survey!.IsActive == true);
        Assert.True(secondSurvey!.IsActive == false);
        Assert.True(thirdSurvey!.IsActive == false);

        var updateTask = _surveySvc.SetSurveyActiveStatusAsync(account.Id, survey!.Id, false);
        var secondUpdateTask = _surveySvc.SetSurveyActiveStatusAsync(parentAccount.Id, secondSurvey!.Id, true);
        var thirdUpdateTask = _surveySvc.SetSurveyActiveStatusAsync(notParentAccount.Id, thirdSurvey!.Id, true);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await updateTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await secondUpdateTask);

            return null;
        }


        var updated = await updateTask;

        Assert.True(updated.IsActive == false);

        var secondUpdated = await secondUpdateTask;

        Assert.True(secondUpdated.IsActive == true);

        var thirdupdated = await updateTask;

        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await thirdUpdateTask);
        Assert.True(thirdupdated.IsActive == false);




        return updated;
    }

    //[Theory]
    //[InlineData(PermissionType.Update, true)]
    //[InlineData(PermissionType.Delete | PermissionType.Read | PermissionType.Create, false)]
    //public async Task<Survey?> UpdateSurveyTheory(
    //  PermissionType permissions = PermissionType.Delete,
    //  bool shouldSucceed = true)
    //{
    //    var (_, survey) = await SurveyAddedTheory();
    //    var (_, secondSurvey) = await SurveyAddedTheory();
    //    var (_, thirdSurvey) = await SurveyAddedTheory();
    //    var (_, toBeSurvey) = await SurveyAddedTheory();

    //    var account = await _uow.CreateTestingAccountAsync(
    //        RandomGenerator.NextAccessLevel(min: AccessLevel.Group, max: AccessLevel.Governorate),
    //        permissions);

    //    var parentAccount = await _uow.CreateTestingAccountAboveUserAsync(
    //       account.User.Id,
    //       account.User.AccessLevel.NextLevelUpove(),
    //       permissions);

    //    var notParentAccount = await _uow.CreateTestingAccountAsync(
    //         RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove(), max: AccessLevel.Presedint),
    //        PermissionType.Delete);

    //    await _uow.CommitAsync();

    //    survey!.CreatedBy = account.User;
    //    secondSurvey!.CreatedBy = account.User;
    //    thirdSurvey!.CreatedBy = account.User;
    //    _uow.Surveys.Update(survey!);
    //    _uow.Surveys.Update(secondSurvey!);
    //    _uow.Surveys.Update(thirdSurvey!);
    //    await _uow.CommitAsync();

    //    Assert.True(survey!.IsActive == true);
    //    Assert.True(secondSurvey!.IsActive == false);
    //    Assert.True(thirdSurvey!.IsActive == false);

    //    var updateTask = _surveySvc.SetSurveyActiveStatusAsync(account.Id, survey!.Id, false);
    //    var secondUpdateTask = _surveySvc.SetSurveyActiveStatusAsync(parentAccount.Id, secondSurvey!.Id, true);
    //    var thirdUpdateTask = _surveySvc.SetSurveyActiveStatusAsync(notParentAccount.Id, thirdSurvey!.Id, true);

    //    if (!shouldSucceed)
    //    {
    //        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await updateTask);
    //        await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await secondUpdateTask);

    //        return null;
    //    }


    //    var updated = await updateTask;

    //    Assert.True(updated.IsActive == false);

    //    var secondUpdated = await secondUpdateTask;

    //    Assert.True(secondUpdated.IsActive == true);

    //    var thirdupdated = await updateTask;

    //    await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await thirdUpdateTask);
    //    Assert.True(thirdupdated.IsActive == false);




    //    return updated;
    //}

    #endregion Tests
}