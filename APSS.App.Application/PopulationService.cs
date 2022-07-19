using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

public sealed class PopulationService : IPopulationService
{
    #region Fields

    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    public PopulationService(IUnitOfWork uow, IPermissionsService permissions)
    {
        _uow = uow;
        _permissionsSvc = permissions;
    }

    #endregion Public Constructors

    #region Public Methods

    ///<inheritdoc/>
    public async Task<Family> AddFamilyAsync(long accountId, string name, string livingLocation)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Create);

        var family = new Family
        {
            Name = name,
            LivingLocation = livingLocation,
            AddedBy = account.User,
        };

        _uow.Families.Add(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async Task<Individual> AddIndividualAsync(
        long accountId,
        long familyId,
        string name,
        string address,
        IndividualSex sex,
        bool isParent = false,
        bool isProvider = false)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Create);

        var individual = new Individual
        {
            Name = name,
            Address = address,
            Sex = sex,
            AddedBy = account.User,
        };

        var family = await _uow.Families
            .Query()
           .Include(f => f.AddedBy)
           .FindWithOwnershipValidationAync(familyId, f => f.AddedBy, account);

        var familyIndividual = new FamilyIndividual
        {
            Individual = individual,
            Family = family,
            IsParent = isParent,
            IsProvider = isProvider,
        };

        _uow.Individuals.Add(individual);
        _uow.FamilyIndividuals.Add(familyIndividual);
        await _uow.CommitAsync();

        return individual;
    }

    ///<inheritdoc/>
    public async Task<Skill> AddSkillAsync(
        long accountId,
        long individualId,
        string name,
        string field,
        string? description = null)
    {
        var account = await GetAuthorizedGroupAccountAsync(
            accountId,
            PermissionType.Create | PermissionType.Update);

        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindWithOwnershipValidationAync(individualId, i => i.AddedBy, account);

        var skill = new Skill
        {
            Name = name,
            Field = field,
            Description = description,
            BelongsTo = individual
        };

        _uow.Skills.Add(skill);
        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> AddVoluntaryAsync(long accountId, long individualId, string name, string field)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Create | PermissionType.Update);

        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindWithOwnershipValidationAync(individualId, i => i.AddedBy, account);

        var voluntary = new Voluntary
        {
            Name = name,
            Field = field,
            OfferedBy = individual
        };

        _uow.Volantaries.Add(voluntary);
        await _uow.CommitAsync();

        return voluntary;
    }

    ///<inheritdoc/>
    public async Task RemoveFamilyAsync(long accountId, long familyId)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Delete);

        var family = await _uow.Families.Query()
            .Include(f => f.AddedBy)
            .FindWithOwnershipValidationAync(familyId, f => f.AddedBy, account);

        _uow.Families.Remove(family);
        await _uow.CommitAsync();
    }

    ///<inheritdoc/>
    public async Task RemoveIndividualAsync(long accountId, long individualId)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Delete);

        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindWithOwnershipValidationAync(individualId, i => i.AddedBy, account);

        _uow.Individuals.Remove(individual);
        await _uow.CommitAsync();
    }

    ///<inheritdoc/>
    public async Task RemoveSkillAsync(long accountId, long skillId)
    {
        var account = await GetAuthorizedGroupAccountAsync(
            accountId,
            PermissionType.Delete | PermissionType.Update);

        var skill = await _uow.Skills.Query()
            .Include(s => s.BelongsTo.AddedBy)
            .FindWithOwnershipValidationAync(skillId, s => s.BelongsTo.AddedBy, account);

        _uow.Skills.Remove(skill);
        await _uow.CommitAsync();
    }

    ///<inheritdoc/>
    public async Task RemoveVoluntaryAsync(long accountId, long voluntaryId)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Delete);

        var voluntary = await _uow.Volantaries.Query()
            .Include(v => v.OfferedBy.AddedBy)
            .FindWithOwnershipValidationAync(voluntaryId, v => v.OfferedBy.AddedBy, account);

        _uow.Volantaries.Remove(voluntary);
        await _uow.CommitAsync();
    }

    ///<inheritdoc/>
    public IQueryBuilder<Family> GetFamiliesAsync(long accountId)
    {
        var family = _uow.Families
            .Query()
            .Include(f => f.AddedBy)
            .Where(f => GetSubuserDistanceAsync(accountId, f.AddedBy.Id).Result >= 0);

        return family;
    }

    ///<inheritdoc/>
    public async Task<IQueryBuilder<FamilyIndividual>> GetFamilyIndividualsAsync(long accountId, long familyId)
    {
        var family = await _uow.Families.Query()
            .Include(f => f.AddedBy)
            .FindAsync(familyId);

        await _permissionsSvc
            .ValidatePermissionsAsync(accountId, family.AddedBy.Id, PermissionType.Read);

        var account = await _uow.Accounts.Query().FindAsync(accountId);

        if (account.User.AccessLevel == AccessLevel.Farmer)
            throw new InsufficientPermissionsException(
                accountId, $"farmer #{account.User.Id} with account #{accountId} cannot add surveys");

        return _uow.FamilyIndividuals.Query().Where(f => f.Family.Id == familyId);
    }

    ///<inheritdoc/>
    public async Task<IQueryBuilder<Skill>> GetSkillOfindividualAsync(long accountId, long individualId)
    {
        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindAsync(individualId);

        await _permissionsSvc
            .ValidatePermissionsAsync(accountId, individual.AddedBy.Id, PermissionType.Read);

        var account = await _uow.Accounts.Query().FindAsync(accountId);

        if (account.User.AccessLevel == AccessLevel.Farmer)
            throw new InsufficientPermissionsException(
                accountId, $"farmer #{account.User.Id} with account #{accountId} cannot add surveys");

        return _uow.Skills.Query().Where(s => s.BelongsTo.Id == individualId);
    }

    ///<inheritdoc/>
    public async Task<IQueryBuilder<Voluntary>> GetVoluntaryOfindividualAsync(long accountId, long individualId)
    {
        var individual = await _uow.Individuals.Query()
           .Include(i => i.AddedBy)
           .FindAsync(individualId);

        await _permissionsSvc
            .ValidatePermissionsAsync(accountId, individual.AddedBy.Id, PermissionType.Read);

        var account = await _uow.Accounts.Query().FindAsync(accountId);

        if (account.User.AccessLevel == AccessLevel.Farmer)
            throw new InsufficientPermissionsException(
                accountId, $"farmer #{account.User.Id} with account #{accountId} cannot add surveys");

        return _uow.Volantaries.Query().Where(v => v.OfferedBy.Id == individualId);
    }

    ///<inheritdoc/>
    public async Task<Family> UpdateFamilyAsync(long accountId, long familyId, Action<Family> updater)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Update);

        var family = await _uow.Families.Query()
            .Include(f => f.AddedBy)
            .FindWithOwnershipValidationAync(familyId, f => f.AddedBy, account);

        updater(family);

        _uow.Families.Update(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async Task<Individual> UpdateIndividualAsync(long accountId, long individualId, Action<Individual> updater)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Update);

        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindWithOwnershipValidationAync(individualId, i => i.AddedBy, account);

        updater(individual);

        _uow.Individuals.Update(individual);
        await _uow.CommitAsync();

        return individual;
    }

    ///<inheritdoc/>
    public async Task<Skill> UpdateSkillAsync(long accountId, long skillId, Action<Skill> updater)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Update);

        var skill = await _uow.Skills.Query()
            .Include(s => s.BelongsTo.AddedBy)
            .FindWithOwnershipValidationAync(skillId, s => s.BelongsTo.AddedBy, account);

        updater(skill);

        _uow.Skills.Update(skill);
        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> UpdateVoluntaryAsync(long accountId, long voluntaryId, Action<Voluntary> updater)
    {
        var account = await GetAuthorizedGroupAccountAsync(accountId, PermissionType.Update);

        var voluntary = await _uow.Volantaries.Query()
            .Include(v => v.OfferedBy.AddedBy)
            .FindWithOwnershipValidationAync(voluntaryId, v => v.OfferedBy.AddedBy, account);

        updater(voluntary);

        _uow.Volantaries.Update(voluntary);
        await _uow.CommitAsync();

        return voluntary;
    }

    #endregion Public Methods

    #region Private Methods

    private async Task<int> GetSubuserDistanceAsync(long accountId, long subuserId)
    {
        var account = await _uow.Accounts.Query().Include(s => s.User).FindAsync(accountId);
        var superuser = account.User;

        if (superuser.AccessLevel == AccessLevel.Root)
            return 0;

        var subuser = await _uow.Users
            .Query()
            .Include(u => u.SupervisedBy!)
            .FindAsync(subuserId);

        if ((int)superuser.AccessLevel > (int)subuser.AccessLevel)
            return -1;

        for (int i = 0; ; ++i)
        {
            if (subuser.SupervisedBy is null)
                return -1;

            if (subuser.SupervisedBy.Id == superuser.Id)
                return i;

            subuser = await _uow.Users
                .Query()
                .Include(u => u.SupervisedBy!)
                .FindAsync(subuser.SupervisedBy.Id);
        }
    }

    private Task<Account> GetAuthorizedGroupAccountAsync(long accountId, PermissionType permissions)
        => _uow.Accounts.Query()
                .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Group, permissions);

    #endregion Private Methods
}