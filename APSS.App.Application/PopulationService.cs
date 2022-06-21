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
    private readonly IUsersService _userSvc;

    #endregion Fields

    #region Public Constructors

    public PopulationService(IUnitOfWork uow, IPermissionsService permissions, IUsersService userSvc)
    {
        _uow = uow;
        _permissionsSvc = permissions;
        _userSvc = userSvc;
    }

    #endregion Public Constructors

    #region Public Methods

    ///<inheritdoc/>
    public async Task<Family> AddFamilyAsync(long userId, string name, string livingLocation)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        if (user.AccessLevel != AccessLevel.Group)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} does not have a permission to add a new family");
        }

        var family = new Family
        {
            Name = name,
            LivingLocation = livingLocation,
            AddedBy = user,
        };

        _uow.Families.Add(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async Task<Individual> AddIndividualAsync(
        long userId,
        long familyId,
        string name,
        string address,
        IndividualSex sex,
        bool isParent = false,
        bool isProvider = false)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        if (user.AccessLevel != AccessLevel.Group)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} does not have a permission to add a new family");
        }

        var family = await _uow.Families.Query()
            .Include(f => f.AddedBy)
            .FindAsync(familyId);

        if (family.AddedBy.Id != user.Id)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot add an individual to family #{family.Id}, which was creatd by user #{family.AddedBy.Id}");
        }

        var individual = new Individual
        {
            Name = name,
            Address = address,
            Sex = sex,
            AddedBy = user,
        };

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
        long userId,
        long individualId,
        string name,
        string field,
        string? description = null)
    {
        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindAsync(individualId);
     
        if (await _permissionsSvc.ValidatePermissionsAsync(userId,individual.AddedBy.Id,individual,PermissionType.Create)!=userId )
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot add a skill to individual #{individual.Id}, which was creatd by user #{individual.AddedBy.Id}");
        }

        var skill = new Skill
        {
            Name = name,
            Field = field,
            Description = description,
            BelongsTo = individual
        };

        _uow.Skills.Add(skill);
        individual.Skills.Add(skill);
        _uow.Individuals.Update(individual);

        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> AddVoluntaryAsync(long userId,long individualId, string name, string field)
    {
        var individual = await _uow.Individuals.Query()
            .Include(i => i.AddedBy)
            .FindAsync(individualId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId,individual.AddedBy.Id,individual,PermissionType.Create)!=userId )
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot add a Voluntary to individual #{individual.Id}, which was creatd by user #{individual.AddedBy.Id}");
        }

        var voluntary = new Voluntary
        {
            Name = name,
            Field = field,
             OfferedBy= individual
        };

        _uow.Volantaries.Add(voluntary);
        individual.Voluntary.Add(voluntary);
        _uow.Individuals.Update(individual);

        await _uow.CommitAsync();

        return voluntary;
    }

    ///<inheritdoc/>
    public async Task<Family> DeleteFamilyAsync(long userId, long familyId)
    {
        var family = await _uow.Families.Query().Include(f=>f.AddedBy).FindAsync(familyId);
        if (await _permissionsSvc.ValidatePermissionsAsync(userId,family.AddedBy.Id,family,PermissionType.Delete)!=userId)
        {
            throw new InsufficientPermissionsException(
             userId,
             $"user #{userId} cannot delete a family #{familyId}, which was creatd by user #{family.AddedBy.Id}");
        }

        _uow.Families.Remove(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async Task<Individual> DeleteIndividualAsync(long userId,long individualId)
    {
        var individual = await _uow.Individuals.Query().Include(i=>i.AddedBy).FindAsync(individualId);
        if (await _permissionsSvc.ValidatePermissionsAsync(userId,individual.AddedBy.Id,individual,PermissionType.Delete)!=userId)
        {
            throw new InsufficientPermissionsException(
             userId,
             $"user {userId} cannot delete a individual #{individualId},which was creatd by user #{individual.AddedBy.Id} ");
        }

        _uow.Individuals.Remove(individual);
        await _uow.CommitAsync();

        return individual;
    }
   
    ///<inheritdoc/>
    public async Task<Skill> DeleteSkillAsync(long userId,long skillId)
    {
        var skill = await _uow.Skills.Query().Include(s => s.BelongsTo).FindAsync(skillId);
        
        if (await _permissionsSvc.ValidatePermissionsAsync(userId, skill.BelongsTo.Id, skill, PermissionType.Delete) != userId)
        {
            throw new InsufficientPermissionsException(
            userId,
             $"user {userId} cannot delete skill of# {skill.BelongsTo.Id}  which was creatd by user #{skill.BelongsTo.AddedBy.Id}");
        }

        _uow.Skills.Remove(skill);
        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> DeleteVoluntaryAsync(long userId,long voluntaryId)
    {
        var voluntary = await _uow.Volantaries.Query().Include(v => v.OfferedBy).FindAsync(voluntaryId);
       
       if(await _permissionsSvc.ValidatePermissionsAsync(userId, voluntary.OfferedBy.Id, voluntary, PermissionType.Delete) != userId)
        {
            throw new InsufficientPermissionsException(
            voluntary.OfferedBy.Id,
             $"user {userId} does not have a permission to delete on the Individual " +
             $"{voluntary.OfferedBy.Id},which was creatd by user #{voluntary.OfferedBy.AddedBy.Id}");
        }

        _uow.Volantaries.Remove(voluntary);
        await _uow.CommitAsync();

        return voluntary;
    }

    ///<inheritdoc/>
    public  IQueryBuilder<Family>GetFamilies(long userId)
    {
        var family=  _uow.Families
            .Query()
            .Include(f=>f.AddedBy)
            .Where( f=> GetSubuserDistanceAsync(userId,f.AddedBy.Id).Result>=0); 
        return family;
    }


    ///<inheritdoc/>
    public async Task<IQueryBuilder<FamilyIndividual>> GetFamilyIndividuals(long userId, long familyId)
    {
        var familis = GetFamilies(userId);
        var family=await _uow.Families.Query().Include(f=>f.AddedBy).FindAsync(familyId);
      var user=await  _permissionsSvc.ValidatePermissionsAsync(userId, family.AddedBy.Id, family, PermissionType.Read);
       return _uow.FamilyIndividuals.Query().Where(f => f.Family.Id == familyId);

    }
    ///<inheritdoc/>
    public async Task<Family> UpdateFamilyAsync(long userId, Family family)
    {
        
        if (await _permissionsSvc.ValidatePermissionsAsync(userId,family.AddedBy.Id,family,PermissionType.Update)!=userId)
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} cannot update a family #{family.Id},which was creatd by user #{family.AddedBy.Id}");
        }

        _uow.Families.Update(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async Task<Individual> UpdateIndividualAsync(long userId,Individual individual)
    {
        if (await _permissionsSvc.ValidatePermissionsAsync(userId, individual.AddedBy.Id, individual, PermissionType.Update) != userId)
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} cannot  update a individual #{individual.Id},which was creatd by user #{individual.AddedBy.Id}");
        }
        _uow.Individuals.Update(individual);
        await _uow.CommitAsync();

        return individual;
    }

    ///<inheritdoc/>
    public async Task<Skill> UpdateSkillAsync(long userId,Skill skill)
    {
        if (await _permissionsSvc.ValidatePermissionsAsync(userId, skill.BelongsTo.Id, skill, PermissionType.Update) != userId)
        {
            throw new InsufficientPermissionsException(
                 userId,
                  $"user {userId} cannot  update askill of  individual #{skill.BelongsTo.Id},which was creatd by user #{skill.BelongsTo.AddedBy.Id}");
        }

        _uow.Skills.Update(skill);
        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> UpdateVoluntaryAsync(long userId,Voluntary voluntary)
    {
        if (await _permissionsSvc.ValidatePermissionsAsync(userId,voluntary.OfferedBy.Id,voluntary,PermissionType.Update)!=userId)
        {
            throw new InsufficientPermissionsException(
                 userId,
         $"user {userId} cannot  update a voluntary of individual #{voluntary.OfferedBy.Id},which was creatd by user #{voluntary.OfferedBy.AddedBy.Id}");
        }

        _uow.Volantaries.Update(voluntary);
        await _uow.CommitAsync();

        return voluntary;
    }

    #endregion Public Methods

    private async Task<int> GetSubuserDistanceAsync(long superuserId, long subuserId)
    {
        var superuser = await _uow.Users.Query().FindAsync(superuserId);

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


}