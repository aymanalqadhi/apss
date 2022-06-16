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
    public  async Task<Family> AddFamily(string name, long userid, string livinglocation)
    {
        var family = new Family
        {
            Name = name,
            LivingLocation = livinglocation,
        };

        var user = await _uow.Users.Query().FindAsync(userid);
        if ((int)user.AccessLevel <= 6)
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

    public Task<Individual> AddIndividual(long userId, string name, string address, IndividualSex sex, DateTime DateOfBirth, string job, string NationalId, SocialStatus socialstatus)
    {
        throw new NotImplementedException();
    }

    public Task<Skill> Addskill(long IndividualId, string name, string description, string Field)
    {
        throw new NotImplementedException();
    }

    public Task<Voluntary> AddVoluntary(long IndividualId, string name, string Field)
    {
        throw new NotImplementedException();
    }

    public Task<Family> DeleteFamily(long Id)
    {
        throw new NotImplementedException();
    }

    public Task<Individual> DeleteIndividual(long Id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteIndividualFamily(long individualId)
    {
        throw new NotImplementedException();
    }

    public Task<Skill> Deleteskill(long skillId)
    {
        throw new NotImplementedException();
    }

    public Task<Voluntary> DeleteVoluntary(long id)
    {
        throw new NotImplementedException();
    }

    public IQueryBuilder<Family> GetFamily()
         => _uow.Families.Query();

    /// <inheritdoc/>
    public IQueryBuilder<FamilyIndividual> GetFamilyIndividuals(long FamilyId)
    {
        var familyindivdual = _uow.FamilyIndividuals.Query();

        return familyindivdual.Where(fi => fi.Family.Id == FamilyId);
    }

    /// <inheritdoc/>
    public IQueryBuilder<Individual> GetIndividual()
    => _uow.Individuals.Query();



    public Task<FamilyIndividual> IndividualOfFamily(long IndividualId, long FamilyId, bool isParent, bool isprovider)
    {
        throw new NotImplementedException();
    }

    public Task<Family> UpdateFamily(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Individual> UpdateIndividual(long id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateIndividualFamily(long famlyId)
    {
        throw new NotImplementedException();
    }

    public Task<Skill> Updateskill(long skillId)
    {
        throw new NotImplementedException();
    }

    public Task<Voluntary> UpdateVoluntary(long id)
    {
        throw new NotImplementedException();
    }

    #endregion

}
