using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IPopulationService
{
    #region Public Methods

    /// <summary>
    /// Gets a query for the get family
    /// </summary>
    /// <param name="userId">The id of the superuser which to get the added fammily  by its</param>
    /// <returns></returns>
    IQueryBuilder<Family> GetFamilies(long userId);

    /// <summary>
    /// Gets a query for the get individuals of family
    /// </summary>
    /// <param name="userId">The id of the superuser which to get the added fammily  by its</param>
    /// <param name="familyId">The id of the family which to get the individuals for </param>
    /// <returns></returns>
    IQueryBuilder<FamilyIndividual> GetFamilyIndividuals(long userId, long familyId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="familyId"></param>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="sex"></param>
    /// <param name="DateOfBirth"></param>
    /// <returns></returns>
    Task<Individual> AddIndividualAsync(
        long userId,
        long familyId,
        string name,
        string address,
        IndividualSex sex,
        bool isParent = false,
        bool isProvider = false
    );

    /// <summary>
    ///
    /// </summary>
    /// <param name="individual"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Individual> UpdateIndividualAsync(long userId, Individual individual);

    /// <summary>
    ///
    /// </summary>
    /// <param name="individualId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Individual> DeleteIndividualAsync(long userId, long individualId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userid"></param>
    /// <param name="LivingLocation"></param>
    /// <returns></returns>
    Task<Family> AddFamilyAsync(long userid, string name, string livingLocation);

    /// <summary>
    ///
    /// </summary>
    /// <param name="family"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Family> UpdateFamilyAsync(long userId, Family family);

    /// <summary>
    ///
    /// </summary>
    /// <param name="familyId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Family> DeleteFamilyAsync(long userId, long familyId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="individualId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<Skill> AddSkillAsync(
        long userId,
        long individualId,
        string name,
        string field,
        string? description = null);

    /// <summary>
    ///
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Skill> UpdateSkillAsync(long userId, Skill skill);

    /// <summary>
    ///
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Skill> DeleteSkillAsync(long userId, long skillId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="IndividualId"></param>
    /// <param name="name"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<Voluntary> AddVoluntaryAsync(
        long userId,
        long IndividualId,
        string name,
        string field);

    /// <summary>
    ///
    /// </summary>
    /// <param name="voluntary"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Voluntary> UpdateVoluntaryAsync(long userId, Voluntary voluntary);

    /// <summary>
    ///
    /// </summary>
    /// <param name="voluntary"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Voluntary> DeleteVoluntaryAsync(long userId, long voluntary);

    #endregion Public Methods
}