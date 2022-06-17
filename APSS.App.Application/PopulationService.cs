using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

public sealed class PopulationService :IPopulationService
{

    #region Fields
    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;
    #endregion

    #region Public Constructors
    public PopulationService(IUnitOfWork uow,IPermissionsService permissions)
    {
        _uow = uow;
        _permissionsSvc = permissions;
    }
    #endregion

    #region Public Methods

    ///<inheritdoc/>
    public  async Task<Family> AddFamilyAsync(string name, long userid, string livinglocation)
    {
        var user = await _uow.Users.Query().FindAsync(userid);
        var family = new Family
        {
            Name = name,
            LivingLocation = livinglocation,
            AddedBy=user,
        };

        if ((int)user.AccessLevel == 6)
        {
            _uow.Families.Add(family);
            await _uow.CommitAsync();
        }

        else
        {
            throw new InsufficientPermissionsException(
                userid,
                $"user {userid} does not have a permission to Add new family ");
        }

        return family;
    }


    ///<inheritdoc/>
    public async Task<Individual> AddIndividualAsync(long userId, string name, string address, IndividualSex sex, DateTime DateOfBirth, string job, string NationalId, SocialStatus socialstatus)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var individual = new Individual
        {
            Name = name,
            Address = address,
            Sex = sex,
            DateOfBirth = DateOfBirth,
            Job = job,
            NationalId = NationalId,
            AddedBy = user
        };

        if ((int)user.AccessLevel != 6)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user {userId} does not have a permission to Add new Individual ");
        }

        _uow.Individuals.Add(individual);
        await _uow.CommitAsync();

        return individual;

    }


    ///<inheritdoc/>
    public async Task<Skill> AddskillAsync(long IndividualId, long userId, string name, string description, string field)
    {

        if (!await _permissionsSvc.IsSuperOfIndividualAsync(userId, IndividualId))
        {
            throw new InsufficientPermissionsException(
              userId,
              $"user {userId} does not have a permission to Add skill for Individual ");
        }
        var skill = new Skill
        {
            Name = name,
            Description = description,
            Field = field,
        };

        var individual = await _uow.Individuals.Query().FindAsync(IndividualId);
            skill.BelongsTo = individual;
            _uow.Skills.Add(skill);
            await _uow.CommitAsync();
   
     return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> AddVoluntaryAsync(long IndividualId,long userId, string name, string field)
    {
        var voluntary = new Voluntary
        {
            Name=name,
            Field = field,
        };

        if (!await _permissionsSvc.IsSuperOfIndividualAsync(userId, IndividualId))
        {
            throw new InsufficientPermissionsException(
                IndividualId,
                $"user {userId} does not have a permission to edite on  Individual{IndividualId} ");
        }
        else
        {
            var individual = await _uow.Individuals.Query().FindAsync(IndividualId);
            voluntary.OfferedBy = individual;
            _uow.Volantaries.Add(voluntary);
            await _uow.CommitAsync();
        }
        return voluntary;
    }

 
///<inheritdoc/>
    public async Task<Family> DeleteFamilyAsync(long familyId, long userId)
    {
        var family = await _uow.Families.Query().FindAsync(familyId);
        if (! await _permissionsSvc.IsSuperOfFamilyAsync(userId, familyId))
        {
            throw new InsufficientPermissionsException(
             familyId,
             $"user {userId} does not have a permission to delete on the Family {familyId}");
        }

        _uow.Families.Remove(family);
            await _uow.CommitAsync();

        return family;
    }


    ///<inheritdoc/>
    public async Task<Individual> DeleteIndividualAsync(long individualId, long userId)
    {
        var individual = await _uow.Individuals.Query().FindAsync(individualId);
        if (!await _permissionsSvc.IsSuperOfIndividualAsync(userId, individualId))
        
        {
            throw new InsufficientPermissionsException(
             individualId,
             $"user {userId} does not have a permission to delete on the Individual {individualId}");
        }

        _uow.Individuals.Remove(individual);
            await _uow.CommitAsync();

        return individual;
    }

    ///<inheritdoc/>
    public async Task<FamilyIndividual> DeleteIndividualFamilyAsync(long familyIndividualId, long userId)
    {
        var individualoffamily = await _uow.FamilyIndividuals.Query().Include(f=>f.Family).FindAsync(familyIndividualId);
        if (!await _permissionsSvc.IsSuperOfFamilyAsync(userId,individualoffamily.Family.Id))
        {
            throw new InsufficientPermissionsException(
             familyIndividualId,
             $"user {userId} does not have a permission to delete on the family's Individual {familyIndividualId}");
        }

        _uow.FamilyIndividuals.Remove(individualoffamily);
            await _uow.CommitAsync();

        return individualoffamily;
    }

    ///<inheritdoc/>
    public async Task<Skill> DeleteskillAsync(long skillId, long userId)
    {
        var skill = await _uow.Skills.Query().Include(s => s.BelongsTo).FindAsync(skillId);
        if(!await _permissionsSvc.IsSuperOfIndividualAsync(userId, skill.BelongsTo.Id))
        {
            throw new InsufficientPermissionsException(
            skill.BelongsTo.Id,
             $"user {userId} does not have a permission to delete on the skill of Individual {skill.BelongsTo.Id}");
        }

        _uow.Skills.Remove(skill);
            await _uow.CommitAsync();
       
        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> DeleteVoluntaryAsync(long voluntaryId, long userId)
    {
        var voluntary = await _uow.Volantaries.Query().Include(v => v.OfferedBy).FindAsync(voluntaryId);
        if (await _permissionsSvc.IsSuperOfIndividualAsync(userId, voluntary.OfferedBy.Id))
        {
            throw new InsufficientPermissionsException(
            voluntary.OfferedBy.Id,
             $"user {userId} does not have a permission to delete on the Individual {voluntary.OfferedBy.Id}");
        }

        _uow.Volantaries.Remove(voluntary);
        await _uow.CommitAsync();
        
        return voluntary;
    }

    ///<inheritdoc/>
    public IQueryBuilder<Family> GetFamily(long userid)
        => _uow.Families.Query().Where(f => f.AddedBy.Id == userid);

    ///<inheritdoc/>
    public IQueryBuilder<FamilyIndividual> GetFamilyIndividuals(long userid, long FamilyId)
        =>  _uow.FamilyIndividuals.Query().Where(fi => fi.Family.Id==FamilyId);
 

    ///<inheritdoc/>
    public  IQueryBuilder<Individual> GetIndividual(long userId)
    => _uow.Individuals.Query().Where(i => i.AddedBy.Id == userId);    


    ///<inheritdoc/>
    public async Task<FamilyIndividual> IndividualOfFamilyAsync(long IndividualId, long FamilyId, bool isparent, bool isprovider)
    {
        var individual =await _uow.Individuals.Query().FindAsync(IndividualId);
        var family = await _uow.Families.Query().FindAsync(FamilyId);

        var familyhas = new FamilyIndividual
        {
            Individual = individual,
            Family = family,
            IsParent = isparent,
            IsProvider = isprovider,
        };

        return familyhas;
    }


    ///<inheritdoc/>
    public async Task<Family> UpdateFamilyAsync(Family family, long userId)
    {
        if(!await _permissionsSvc.IsSuperOfFamilyAsync(family.Id, userId))
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} does not have a permission to update Family #{family.Id}");
        }

        _uow.Families.Update(family);
        await _uow.CommitAsync();

        return family;
    }

    ///<inheritdoc/>
    public async  Task<Individual> UpdateIndividualAsync(Individual individual, long userId)
    {
        if(! await _permissionsSvc.IsSuperOfIndividualAsync(userId, individual.Id))
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} does not have a permission to update individual #{individual.Id}");
        }
        _uow.Individuals.Update(individual);
        await _uow.CommitAsync();

        return individual;
    }

    ///<inheritdoc/>
    public async Task<FamilyIndividual> UpdateIndividualFamilyAsync(FamilyIndividual familyindividual, long userId)
    {
        if(! await _permissionsSvc.IsSuperOfFamilyAsync(userId, familyindividual.Id))
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} does not have a permission to update Family #{familyindividual.Id}");
        }

        _uow.FamilyIndividuals.Update(familyindividual);
        await _uow.CommitAsync();

        return familyindividual;
       
    }

    ///<inheritdoc/>
    public async Task<Skill> UpdateskillAsync(Skill skill, long userId)
    {
        if (!await _permissionsSvc.IsSuperOfIndividualAsync(userId, skill.BelongsTo.Id))
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} does not have a permission to update skill of individual #{skill.BelongsTo.Id}");
        }

        _uow.Skills.Update(skill);
        await _uow.CommitAsync();

        return skill;
    }

    ///<inheritdoc/>
    public async Task<Voluntary> UpdateVoluntaryAsync(Voluntary voluntary, long userId)
    {
        if (!await _permissionsSvc.IsSuperOfIndividualAsync(userId, voluntary.OfferedBy.Id))
        {
            throw new InsufficientPermissionsException(
                 userId,
                 $"user {userId} does not have a permission to update individual Voluntary #{voluntary.OfferedBy.Id}");
        }

        _uow.Volantaries.Update(voluntary);
        await _uow.CommitAsync();

        return voluntary;
    }
    #endregion

}
