using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IPopulationService
{
    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IQueryBuilder<Individual> GetIndividual();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IQueryBuilder<Family> GetFamily();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="FamilyId"></param>
    /// <returns></returns>
    IQueryBuilder<FamilyIndividual> GetFamilyIndividuals(long FamilyId);
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
    Task<Individual> AddIndividual(
        long userId, string name, string address,
        IndividualSex sex,
        DateTime DateOfBirth,
        string job,
        string NationalId,
        SocialStatus socialstatus);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Individual> UpdateIndividual(long id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Individual> DeleteIndividual(long Id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="userid"></param>
    /// <param name="LivingLocation"></param>
    /// <returns></returns>
    Task<Family> AddFamily(string name, long userid, string livinglocation);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Family> UpdateFamily(long id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    Task<Family> DeleteFamily(long Id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="IndividualId"></param>
    /// <param name="FamilyId"></param>
    /// <param name="isParent"></param>
    /// <param name="isprovider"></param>
    /// <returns></returns>
    Task<FamilyIndividual> IndividualOfFamily(
        long IndividualId,
        long FamilyId,
        bool isParent,
        bool isprovider);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="famlyId"></param>
    /// <returns></returns>
    Task   UpdateIndividualFamily(long famlyId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="individualId"></param>
    /// <returns></returns>
    Task DeleteIndividualFamily(long individualId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="IndividualId"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="Field"></param>
    /// <returns></returns>
    Task<Skill> Addskill(
        long IndividualId,
        string name,
        string description,
        string Field);

    Task<Skill> Updateskill(long skillId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    Task<Skill> Deleteskill(long skillId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="IndividualId"></param>
    /// <param name="name"></param>
    /// <param name="Field"></param>
    /// <returns></returns>
    Task<Voluntary> AddVoluntary(
        long IndividualId,
        string name,
        string Field);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Voluntary> UpdateVoluntary(long id);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task <Voluntary>DeleteVoluntary(long id);
    #endregion

}
