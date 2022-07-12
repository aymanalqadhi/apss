using APSS.Application.App;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Domain.ValueTypes;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services.Exceptions;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using APSS.Tests.Utils;

namespace APSS.Tests.Application.App;

public sealed class PopulationServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly IPopulationService _populationSvc;

    #endregion Private fields

    #region Constructors

    public PopulationServiceTest(IUnitOfWork uow, IPopulationService populationSvc)
    {
        _uow = uow;
        _populationSvc = populationSvc;
    }

    #endregion Constructors

    #region Tests

    [Theory]
    [InlineData(AccessLevel.Group, PermissionType.Create, true)]
    [InlineData(AccessLevel.Group, PermissionType.Update, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Create, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Read, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Delete, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
    public async Task<(Account, Family?)> FamilyaddedTheory(
        AccessLevel accessLevel = AccessLevel.Group,
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissions);
        var templateFamily = ValidEntitiesFactory.CreateValidFamily();

        var addFamilyTask = _populationSvc.AddFamilyAsync(
            account.Id,
           templateFamily.Name,
           templateFamily.LivingLocation);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addFamilyTask);
            return (account, null);
        }

        var family = await addFamilyTask;

        Assert.True(await _uow.Families.Query().ContainsAsync(family));
        Assert.Equal(account.User.Id, family.AddedBy.Id);
        Assert.Equal(templateFamily.Name, family.Name);
        Assert.Equal(templateFamily.LivingLocation, family.LivingLocation);

        return (account, family);
    }


    [Theory]
    [InlineData(AccessLevel.Group, PermissionType.Create, true)]
    [InlineData(AccessLevel.Farmer, PermissionType.Create, false)]
    [InlineData(AccessLevel.Group, PermissionType.Read, false)]
    [InlineData(AccessLevel.Group, PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Delete, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]

    public async Task IndividualAddedTheory(
        AccessLevel accessLevel = AccessLevel.Group,
        PermissionType permission = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (account, family) = await FamilyaddedTheory(AccessLevel.Group | accessLevel, PermissionType.Create | permission, true);
        var templateIndividual = ValidEntitiesFactory.CreateValidIndividual();
        var addIndividualTask = _populationSvc
            .AddIndividualAsync(
            account.Id,
            family!.Id,
            templateIndividual.Name,
            templateIndividual.Address,
            templateIndividual.Sex,
            false,
            false
            );

        Assert.True(await _uow.Families.Query().ContainsAsync(family!));

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addIndividualTask);
           
        }

        var individual = await addIndividualTask;

        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual));
        Assert.Equal(account.User.Id, individual.AddedBy.Id);
        Assert.Equal(templateIndividual.Name, individual.Name);
        Assert.Equal(templateIndividual.Sex, individual.Sex);
        Assert.Equal(templateIndividual.Address, individual.Address);

       
    }
    #endregion Tests

}
