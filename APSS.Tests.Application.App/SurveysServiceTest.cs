using System;
using System.Linq;
using System.Threading.Tasks;

using APSS.Application.App;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Domain.ValueTypes;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Tests.Utils;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class SurveysServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly ISurveysService _surveySvc;

    #endregion Private fields

    #region Constructors

    public SurveysServiceTest(IUnitOfWork uow, ISurveysService surveySvc)
    {
        _uow = uow;
        _surveySvc = surveySvc;
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
        PermissionType permissions = PermissionType.Read,
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

    #endregion Tests
}