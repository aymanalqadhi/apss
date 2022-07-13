using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IPopulationService
{
    #region Public Methods

    /// <summary>
    /// Asynnchrosuly Gets a query for the get family
    /// </summary>
    /// <param name="accountId">The id of the account which to get the added fammily  by its</param>
    /// <returns></returns>
    IQueryBuilder<Family> GetFamiliesAsync(long accountId);

    /// <summary>
    /// Asynnchrosuly Gets a query for the get individuals of family
    /// </summary>
    /// <param name="accountId">The id of the account which to get the added fammily  by its</param>
    /// <param name="familyId">The id of the family which to get the individuals for </param>
    /// <returns></returns>
    Task<IQueryBuilder<FamilyIndividual>> GetFamilyIndividualsAsync(long accountId, long familyId);

    /// <summary>
    ///  Asynchronosuly add a new Individuals
    /// </summary>
    /// <param name="accountId">The id of the account to add the new Individual</param>
    /// <param name="familyId">Thre id of the family of new individal</param>
    /// <param name="name"> The name of new individual</param>
    /// <param name="address">The adress of new individual</param>
    /// <param name="sex">The sex of the new indivaul</param>
    /// <param name="isParent">Whether the individual is parent(father or mother)</param>
    /// <param name="isProvider">Whether the individual provider on family</param>
    /// <returns>The created individual</returns>
    Task<Individual> AddIndividualAsync(
        long accountId,
        long familyId,
        string name,
        string address,
        IndividualSex sex,
        bool isParent = false,
        bool isProvider = false
    );
    
    /// <summary>
    ///  Asynchronosuly update a Individual
    /// </summary>
    /// <param name="accountId">The id of the account of the individual to update</param>
    /// <param name="individualId">The id of individual to update</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The Updated individual</returns>
    Task<Individual> UpdateIndividualAsync(long accountId, long individualId,Action<Individual> updater);

    /// <summary>
    /// Asynchronosuly  delete a Individual
    /// </summary>
    /// <param name="accountId">The id of the account of the individual to delete</param>
    /// <param name="individualId">The individual to delete</param>
    /// <returns></returns>
    Task RemoveIndividualAsync(long accountId, long individualId);

    /// <summary>
    /// Asynchronosy add  a new family 
    /// </summary>
    /// <param name="accountId"> The id of account to add the new family </param>
    /// <param name="name">The name of new family</param>
    /// <param name="livingLocation">the address of new family</param>
    /// <returns>The added family</returns>
    Task<Family> AddFamilyAsync(long accountId, string name, string livingLocation);

    /// <summary>
    /// Asynchronosy update a family
    /// </summary>
    /// <param name="accountId">The id of account of family to update</param>
    /// <param name="familyId">The id of family to update</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The updated family</returns>
    Task<Family> UpdateFamilyAsync(long accountId, long familyId, Action<Family> updater);

    /// <summary>
    /// Asynchronosy delete a family
    /// </summary>
    /// <param name="accountId">The id of account of family to delete </param>
    /// <param name="familyId">The family to delete</param>
    /// <returns></returns>
    Task  RemoveFamilyAsync(long accountId, long familyId);

    /// <summary>
    /// Asynchronosy add a skill of individual
    /// </summary>
    /// <param name="accountId">The id of  account of individual to add skill </param>
    /// <param name="individualId">The id of individual which this skill belongs to</param>
    /// <param name="name"> The name of the new skill</param>
    /// <param name="description">The description of the new skill </param>
    /// <param name="field">The field of the new skill</param>
    /// <returns>The added new skill</returns>
    Task<Skill> AddSkillAsync(
        long accountId,
        long individualId,
        string name,
        string field,
        string? description = null);

    /// <summary>
    /// Asynchronosy update a skill
    /// </summary>
    /// <param name="accountId">The id of account of the individual which this skill to</param>
    /// <param name="skillId">The id of skill to update</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The updated family</returns>
    Task<Skill> UpdateSkillAsync(long accountId, long skillId, Action<Skill>updater);

    /// <summary>
    /// Asynchronosy delete a skill
    /// </summary>
    /// <param name="accountId"> The id of account of individual which this skill to</param>
    /// <param name="skillId">The id of skill to delete</param>
    /// <returns></returns>
    Task RemoveSkillAsync(long accountId, long skillId);

    /// <summary>
    /// Asynchronosy add a new voluntary
    /// </summary>
    /// <param name="accountId"> The id of account to add the new voluntry </param>
    /// <param name="IndividualId">The id of individual which this voluntry to</param>
    /// <param name="name">The name of voluntry</param>
    /// <param name="field">The field of voluntry</param>
    /// <returns>The added voluntry</returns>
    Task<Voluntary> AddVoluntaryAsync(
        long accountId,
        long IndividualId,
        string name,
        string field);
    /// <summary>
    /// Asyncronosy update a voluntry
    /// </summary>
    /// <param name="accountId">The id of account of individual which this voluntry to </param>
    /// <param name="voluntary">The voluntry to update</param>
    /// <returns>The updated voluntry</returns>
    /// <summary>
    /// Asyncronosy update a voluntry
    /// </summary>
    /// <param name="accountId">The id of account of individual which this voluntry to </param>
    /// <param name="voluntaryId">The id of voluntry to update</param>
    /// <param name="updater">The updating callback</param>
    /// <returns>The updated family</returns>
    Task<Voluntary> UpdateVoluntaryAsync(long accountId, long voluntaryId,Action<Voluntary> updater);

    /// <summary>
    /// Asyncronosy delete a voluntry
    /// </summary>
    /// <param name="accountId">The id of account of individual which this voluntry to </param>
    /// <param name="voluntaryId">The id of voluntry to delete </param>
    /// <returns></returns>
    Task RemoveVoluntaryAsync(long accountId, long voluntaryId);

    #endregion Public Methods
}