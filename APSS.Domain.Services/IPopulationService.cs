using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IPopulationService
{
    #region Public Methods

    /// <summary>
    /// Gets a query for the individual set
    /// </summary>
    /// <param name="userid">The id of the superuser which to get the added individual  by its</param>
    /// <returns></returns>
    IQueryBuilder<Individual> GetIndividual(long userid);

    /// <summary>
    /// Gets a query for the get family
    /// </summary>
    /// <param name="userid">The id of the superuser which to get the added fammily  by its</param>
    /// <returns></returns>
    IQueryBuilder<Family> GetFamily(long userid);

    /// <summary>
    /// Gets a query for the get individuals of family
    /// </summary>
    /// <param name="userid">The id of the superuser which to get the added fammily  by its</param>
    /// <param name="FamilyId">The id of the family which to get the individuals for </param>
    /// <returns></returns>
    IQueryBuilder<FamilyIndividual> GetFamilyIndividuals(long userid,long FamilyId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="name"></param>
    /// <param name="address"></param>
    /// <param name="sex"></param>
    /// <param name="DateOfBirth"></param>
    /// <param name="job"></param>
    /// <param name="NationalId"></param>
    /// <param name="socialstatus"></param>
    /// <returns></returns>
    Task<Individual> AddIndividualAsync(
        long userId, string name, string address,
        IndividualSex sex,
        DateTime DateOfBirth,
        string job,
        string NationalId,
        SocialStatus socialstatus);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="individual"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Individual> UpdateIndividualAsync(Individual individual,long userId);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="individualId"></param>
   /// <param name="userId"></param>
   /// <returns></returns>
    Task<Individual> DeleteIndividualAsync(long individualId,long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userid"></param>
    /// <param name="LivingLocation"></param>
    /// <returns></returns>
    Task<Family> AddFamilyAsync(string name, long userid, string livinglocation);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="family"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Family> UpdateFamilyAsync(Family family, long userId);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="familyId"></param>
   /// <param name="userId"></param>
   /// <returns></returns>
    Task<Family> DeleteFamilyAsync(long familyId, long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="IndividualId"></param>
    /// <param name="FamilyId"></param>
    /// <param name="isParent"></param>
    /// <param name="isprovider"></param>
    /// <returns></returns>
    Task<FamilyIndividual> IndividualOfFamilyAsync(
        long IndividualId,
        long FamilyId,
        bool isParent,
        bool isprovider);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyindividual"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task <FamilyIndividual>  UpdateIndividualFamilyAsync(FamilyIndividual familyindividual, long userId);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="familyIndividualId"></param>
   /// <param name="userId"></param>
   /// <returns></returns>
    Task<FamilyIndividual> DeleteIndividualFamilyAsync(long familyIndividualId, long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="IndividualId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="Field"></param>
    /// <returns></returns>
    Task<Skill> AddskillAsync(
        long userid,
        long IndividualId,
        string name,
        string description,
        string Field);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Skill> UpdateskillAsync(Skill skill, long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Skill> DeleteskillAsync(long skillId, long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="IndividualId"></param>
    /// <param name="name"></param>
    /// <param name="Field"></param>
    /// <returns></returns>
    Task<Voluntary> AddVoluntaryAsync(
        long userId,
        long IndividualId,
        string name,
        string Field);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voluntary"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<Voluntary> UpdateVoluntaryAsync(Voluntary voluntary, long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voluntary"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task <Voluntary>DeleteVoluntaryAsync(long voluntary, long userId);
   
    #endregion

}
