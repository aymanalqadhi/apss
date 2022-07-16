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
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class PopulationServiceTest : IDisposable
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
    [InlineData(PermissionType.Create, true)]
    [InlineData(PermissionType.Update, false)]
    [InlineData(PermissionType.Read, false)]
    [InlineData(PermissionType.Delete, false)]
    public async Task<(Account, Individual?)> IndividualAddedTheory(
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (FamilyAccount, family) = await FamilyaddedTheory();

        Assert.True(await _uow.Families.Query().ContainsAsync(family!));

        var account = await _uow.CreateTestingAccountForUserAsync(FamilyAccount.User.Id, permissions);
        var templateIndividual = ValidEntitiesFactory.CreateValidIndividual();

        var addIndividualTask = _populationSvc
            .AddIndividualAsync(
            account.Id,
            family!.Id,
            templateIndividual.Name,
            templateIndividual.Address,
            templateIndividual.Sex,
            false,
            false);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addIndividualTask);
            return (account, null);
        }

        var individual = await addIndividualTask;

        Assert.True(await _uow.FamilyIndividuals.Query().AnyAsync(f => f.Individual.Id == individual.Id && f.Family.Id == family.Id));
        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual));
        Assert.Equal(account.User.Id, individual.AddedBy.Id);
        Assert.Equal(templateIndividual.Name, individual.Name);
        Assert.Equal(templateIndividual.Sex, individual.Sex);
        Assert.Equal(templateIndividual.Address, individual.Address);

        return (account, individual);
    }

    [Theory]
    [InlineData(PermissionType.Create | PermissionType.Update, true)]
    [InlineData(PermissionType.Delete, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task<(Account, Skill?)> SkillAddedTheory(
        PermissionType permission = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (individualAccount, individual) = await IndividualAddedTheory();

        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual!));

        var account = await _uow.CreateTestingAccountForUserAsync(individualAccount.User.Id, permission);

        var templateSkill = ValidEntitiesFactory.CreateValidSkill();

        var addSkillTask = _populationSvc
            .AddSkillAsync(
            account.Id,
            individual!.Id,
            templateSkill.Name,
            templateSkill.Field,
            templateSkill.Description);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addSkillTask);
            return (account, null);
        }

        var skill = await addSkillTask;

        Assert.True(await _uow.Skills.Query().ContainsAsync(await addSkillTask));
        Assert.Contains(individual.Skills, s => s.Id == skill.Id);
        Assert.Equal(individual.Id, skill.BelongsTo.Id);
        Assert.Equal(templateSkill.Name, skill.Name);
        Assert.Equal(templateSkill.Field, skill.Field);
        Assert.Equal(templateSkill.Description, skill.Description);

        return (account, skill);
    }

    [Theory]
    [InlineData(PermissionType.Create | PermissionType.Update, true)]
    [InlineData(PermissionType.Delete, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task<(Account, Voluntary?)> VoluntaryAddedTheory(
        PermissionType permission = PermissionType.Update,
        bool shouldSucceed = true)
    {
        var (individualAccount, individual) = await IndividualAddedTheory();

        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual!));

        var account = await _uow.CreateTestingAccountForUserAsync(individualAccount.User.Id, permission);

        var templateVoluntary = ValidEntitiesFactory.CreateValidVoluntary();

        var addVoluntaryTask = _populationSvc
            .AddVoluntaryAsync(
            account.Id,
            individual!.Id,
            templateVoluntary.Name,
            templateVoluntary.Field);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addVoluntaryTask);
            return (account, null);
        }

        var voluntary = await addVoluntaryTask;

        Assert.True(await _uow.Volantaries.Query().ContainsAsync(await addVoluntaryTask));
        Assert.Contains(individual.Voluntary, v => v.Id == voluntary.Id);
        Assert.Equal(individual.Id, voluntary.OfferedBy.Id);
        Assert.Equal(templateVoluntary.Name, voluntary.Name);
        Assert.Equal(templateVoluntary.Field, voluntary.Field);

        return (account, voluntary);
    }

    [Theory]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Update, false)]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Read, false)]
    public async Task FamilyRemovedTheory(
        PermissionType permission,
        bool shouldSucceed)
    {
        var (account, family) = await FamilyaddedTheory(AccessLevel.Group, PermissionType.Create | permission, true);

        Assert.True(await _uow.Families.Query().ContainsAsync(family!));

        var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Delete);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _populationSvc.RemoveFamilyAsync(otherAccount.Id, family!.Id)
        );

        var removeFamilyTask = _populationSvc.RemoveFamilyAsync(account.Id, family!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeFamilyTask);
            return;
        }

        await removeFamilyTask;
        Assert.False(await _uow.Families.Query().ContainsAsync(family));
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Update, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task IndividualRemovedTheory(
       PermissionType permission,
       bool shouldSucceed)
    {
        var (account, individual) = await IndividualAddedTheory(PermissionType.Create | permission, true);

        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual!));

        var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Delete);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _populationSvc.RemoveIndividualAsync(otherAccount.Id, individual!.Id)
        );

        var removeIndividualTask = _populationSvc.RemoveIndividualAsync(account.Id, individual!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeIndividualTask);
            return;
        }

        await removeIndividualTask;

        Assert.False(await _uow.Individuals.Query().ContainsAsync(individual));
    }

    [Theory]
    [InlineData(PermissionType.Delete | PermissionType.Update, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task SkillRemovedTheory(
       PermissionType permission,
       bool shouldSucceed)
    {
        var (account, skill) = await SkillAddedTheory(PermissionType.Create | PermissionType.Update | permission, true);

        Assert.True(await _uow.Skills.Query().ContainsAsync(skill!));

        var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Delete | PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _populationSvc.RemoveSkillAsync(otherAccount.Id, skill!.Id)
        );

        var removeSkillTask = _populationSvc.RemoveSkillAsync(account.Id, skill!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeSkillTask);
            return;
        }

        await removeSkillTask;

        Assert.False(await _uow.Skills.Query().ContainsAsync(skill));
    }

    [Theory]
    [InlineData(PermissionType.Delete | PermissionType.Update, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task VoluntaryRemovedTheory(
       PermissionType permission,
       bool shouldSucceed)
    {
        var (account, voluntary) = await VoluntaryAddedTheory(
            PermissionType.Create | PermissionType.Update | permission, true);

        Assert.True(await _uow.Volantaries.Query().ContainsAsync(voluntary!));

        var otherAccount = await _uow
            .CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Delete | PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _populationSvc.RemoveVoluntaryAsync(otherAccount.Id, voluntary!.Id)
        );

        var removeVoluntaryTask = _populationSvc.RemoveVoluntaryAsync(account.Id, voluntary!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeVoluntaryTask);
            return;
        }

        await removeVoluntaryTask;

        Assert.False(await _uow.Volantaries.Query().ContainsAsync(voluntary));
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Delete, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task FamilyUpdatedTheory(PermissionType permission = PermissionType.Update,
        bool shouldSucceed = true)
    {
        var (familyAccount, family) = await FamilyaddedTheory();

        Assert.True(await _uow.Families.Query().ContainsAsync(family!));

        var account = await _uow.CreateTestingAccountForUserAsync(familyAccount.User.Id, permission);

        var name = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var living = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);

        var updateFamilyTask = _populationSvc
            .UpdateFamilyAsync(account.Id, family!.Id,
                    f => { f.Name = name; f.LivingLocation = living; });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateFamilyTask);
            return;
        }

        var otherAccount = await _uow
            .CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
        _populationSvc
            .UpdateFamilyAsync(otherAccount.Id, family!.Id,
                    f => { f.Name = name; f.LivingLocation = living; }));

        var familynew = await updateFamilyTask;
        Assert.True(await _uow.Families.Query().ContainsAsync(familynew));
        Assert.Equal(name, familynew.Name);
        Assert.Equal(living, familynew.LivingLocation);

        familynew = await _uow.Families.Query().FindAsync(familynew.Id);

        Assert.Equal(name, familynew.Name);
        Assert.Equal(living, familynew.LivingLocation);
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Delete, false)]
    [InlineData(PermissionType.Read, false)]
    public async Task individualUpdatedTheory(PermissionType permission = PermissionType.Update,
       bool shouldSucceed = true)
    {
        var (individualAccount, individual) = await IndividualAddedTheory();

        Assert.True(await _uow.Individuals.Query().ContainsAsync(individual!));

        var account = await _uow.CreateTestingAccountForUserAsync(individualAccount.User.Id, permission);

        var name = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var job = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var phone = RandomGenerator.NextString(RandomGenerator.NextInt(9, 15));
        var address = RandomGenerator.NextString(RandomGenerator.NextInt(10, 30), RandomStringOptions.Mixed);

        var updateIndividualTask = _populationSvc
            .UpdateIndividualAsync(account.Id, individual!.Id,
                    i =>
                    {
                        i.Name = name;
                        i.Job = job;
                        i.Address = address;
                        i.PhoneNumber = phone;
                    });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateIndividualTask);
            return;
        }

        var otherAccount = await _uow
            .CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
        _populationSvc
            .UpdateIndividualAsync(otherAccount.Id, individual!.Id,
                    i =>
                    {
                        i.Name = name;
                        i.Job = job;
                        i.Address = address;
                        i.PhoneNumber = phone;
                    }));

        var individualnew = await updateIndividualTask;
        Assert.Equal(name, individual.Name);
        Assert.Equal(job, individualnew.Job);
        Assert.Equal(phone, individualnew.PhoneNumber);
        Assert.Equal(address, individualnew.Address);

        individualnew = await _uow.Individuals.Query().FindAsync(individualnew.Id);

        Assert.Equal(name, individual.Name);
        Assert.Equal(job, individualnew.Job);
        Assert.Equal(phone, individualnew.PhoneNumber);
        Assert.Equal(address, individualnew.Address);
    }

    [Theory]
    [InlineData(PermissionType.Update)]
    [InlineData(PermissionType.Create)]
    [InlineData(PermissionType.Delete)]
    [InlineData(PermissionType.Read)]
    public async Task SkillUpdatedTheory(PermissionType permission = PermissionType.Update)
    {
        var (account, skill) = await SkillAddedTheory(PermissionType.Create | PermissionType.Update | permission, true);

        Assert.True(await _uow.Skills.Query().ContainsAsync(skill!));

        var name = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var field = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var description = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);

        var updateSkillTask = _populationSvc
            .UpdateSkillAsync(account.Id, skill!.Id,
                    s =>
                    {
                        s.Name = name;
                        s.Field = field;
                        s.Description = description;
                    });

        if (account.User.AccessLevel != AccessLevel.Group)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateSkillTask);
            return;
        }

        var otherAccount = await _uow
            .CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
        _populationSvc
            .UpdateSkillAsync(otherAccount.Id, skill!.Id,
                    s =>
                    {
                        s.Name = name;
                        s.Field = field;
                        s.Description = description;
                    }));

        var skillnew = await updateSkillTask;

        Assert.True(await _uow.Skills.Query().ContainsAsync(skillnew));
        Assert.Equal(name, skillnew.Name);
        Assert.Equal(field, skillnew.Field);
        Assert.Equal(description, skillnew.Description);

        skillnew = await _uow.Skills.Query().FindAsync(skillnew.Id);
        Assert.Equal(name, skillnew.Name);
        Assert.Equal(field, skillnew.Field);
        Assert.Equal(description, skillnew.Description);
    }

    [Theory]
    [InlineData(PermissionType.Update)]
    [InlineData(PermissionType.Create)]
    [InlineData(PermissionType.Delete)]
    [InlineData(PermissionType.Read)]
    public async Task VoluntaryUpdatedTheory(PermissionType permission = PermissionType.Update)
    {
        var (account, voluntary) = await VoluntaryAddedTheory(
                                PermissionType.Create | PermissionType.Update | permission, true);

        Assert.True(await _uow.Volantaries.Query().ContainsAsync(voluntary!));

        var name = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);
        var field = RandomGenerator.NextString(RandomGenerator.NextInt(5, 15), RandomStringOptions.Alpha);

        var updateVoluntaryTask = _populationSvc
            .UpdateVoluntaryAsync(account.Id, voluntary!.Id,
                    s =>
                    {
                        s.Name = name;
                        s.Field = field;
                    });

        if (account.User.AccessLevel != AccessLevel.Group)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateVoluntaryTask);
            return;
        }

        var otherAccount = await _uow
            .CreateTestingAccountAsync(AccessLevel.Group, PermissionType.Update);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
        _populationSvc
            .UpdateVoluntaryAsync(otherAccount.Id, voluntary!.Id,
                    s =>
                    {
                        s.Name = name;
                        s.Field = field;
                    }));

        var voluntarynew = await updateVoluntaryTask;
        Assert.True(await _uow.Volantaries.Query().ContainsAsync(voluntary));
        Assert.Equal(name, voluntarynew.Name);
        Assert.Equal(field, voluntarynew.Field);

        voluntarynew = await _uow.Volantaries.Query().FindAsync(voluntarynew.Id);
        Assert.Equal(name, voluntarynew.Name);
        Assert.Equal(field, voluntarynew.Field);
    }

    [Theory]
    [InlineData(AccessLevel.Group | AccessLevel.Root | AccessLevel.Village | AccessLevel.District
               | AccessLevel.Directorate | AccessLevel.Presedint | AccessLevel.Governorate,
                PermissionType.Read, true)]
    [InlineData(AccessLevel.Group | AccessLevel.Root | AccessLevel.Village | AccessLevel.District
               | AccessLevel.Directorate | AccessLevel.Presedint | AccessLevel.Governorate,
                PermissionType.Update | PermissionType.Create | PermissionType.Delete, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Full, false)]
    public async Task GetIndividualOfFamliesTheory(AccessLevel accessLevel,
        PermissionType permission = PermissionType.Read,
        bool shoulSucceed = true)
    {
        var templateFamlyIndividuals = ValidEntitiesFactory.CreateValidFamilyIndividual();

        _uow.FamilyIndividuals.Add(templateFamlyIndividuals);
        await _uow.CommitAsync();

        Assert.True(await _uow.FamilyIndividuals.Query().ContainsAsync(templateFamlyIndividuals));

        var account = _uow.CreateTestingAccountAsync(templateFamlyIndividuals.Family.AddedBy.AccessLevel, permission);

        var getFamlyIndividualTask = _populationSvc.GetFamilyIndividualsAsync(account.Id, templateFamlyIndividuals.Family.Id);

        if (!shoulSucceed & accessLevel != AccessLevel.Farmer)
        {
            await Assert
                .ThrowsAsync<InsufficientPermissionsException>(async () => await getFamlyIndividualTask);
            return;
        }

        var familyIndividuals = await getFamlyIndividualTask;
        Assert.NotNull(familyIndividuals);
        Assert.True(await familyIndividuals.ContainsAsync(templateFamlyIndividuals));
    }

    #endregion Tests

    #region IDisposable Members

    /// <inheritdoc/>
    public void Dispose()
    => _uow.Dispose();

    #endregion IDisposable Members
}