using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface IAnimalService
{
    #region Public Methods

    /// <summary>
    /// Asynchrnously adds a Animal group
    /// </summary>
    /// <param name="accountId">the id of account adding animal group</param>
    /// <param name="type">the type of animal group</param>
    /// <param name="name">the name of animal group</param>
    /// <param name="quantity">the quantity of animal</param>
    /// <param name="animalSex">the sex of animal</param>
    /// <returns>Create animal group object</returns>
    Task<AnimalGroup> AddAnimalGroupAsync(
        long accountId,
        string type,
        string name,
        int quantity,
        AnimalSex animalSex);

    /// <summary>
    /// Asynchrnously adds a Animal product
    /// </summary>
    /// <param name="accountId">the id of account adding animal product</param>
    /// <param name="animalGroupId">the id of animal group that produce thise product</param>
    /// <param name="name">the name of animal product</param>
    /// <param name="quantity">the quantity of animal product</param>
    /// <param name="unit">the unit of animal product</param>
    /// <param name="periodTaken">the period taken for animal product</param>
    /// <returns>create aniamal product object</returns>
    Task<AnimalProduct> AddAnimalProductAsync(
        long accountId,
        long animalGroupId,
        long animalProductUnitId,
        string name,
        double quantity,
        TimeSpan periodTaken);

    /// <summary>
    /// Asynchrnously get all animal groups
    /// </summary>
    /// <param name="accountId">the id of account getting animal product</param>
    /// <param name="userId">the id of user getting animal group</param>
    /// <returns>animal group objects</returns>
    Task<IQueryBuilder<AnimalGroup>> GetAnimalGroupsAsync(long accountId, long userId);

    /// <summary>
    /// Asynchrnously get animal product
    /// </summary>
    /// <param name="accountId">the id of account getting animal product</param>
    /// <param name="userId">The id of the user who owns the animal group</param>
    /// <param name="animalGroupId">the id of animal group</param>
    /// <returns>animal product object</returns>
    Task<IQueryBuilder<AnimalProduct>> GetAnimalProductsAsync(long accountId, long userId, long animalGroupId);

    /// <summary>
    /// Asynchrnously delete animal group
    /// </summary>
    /// <param name="userId">the id of account deleting animal group</param>
    /// <param name="animalGroupId">the id of animal group</param>
    /// <returns></returns>
    Task RemoveAnimalGroupAsync(long userId, long animalGroupId);

    /// <summary>
    /// Asynchrnously delete animal product
    /// </summary>
    /// <param name="accountId">the id of account deleting aninal product</param>
    /// <param name="animalProductId">the id of animal group</param>
    /// <returns></returns>
    Task RemoveAnimalProductAsync(long accountId, long animalProductId);

    /// <summary>
    /// Asynchrnously deleupdatete animal product
    /// </summary>
    /// <param name="accounId">the id of account updating animal group</param>
    /// <param name="animalGroup">the id of animal group</param>
    /// <returns></returns>
    Task<AnimalGroup> UpdateAnimalGroupAsync(long accounId, long animalGroupId, Action<AnimalGroup> updater);

    /// <summary>
    /// Asynchrnously update  animal product
    /// </summary>
    /// <param name="AccountId">the id of account getting animal product</param>
    /// <param name="animalProduct">the object of animal product</param>
    /// <returns></returns>
    Task<AnimalProduct> UpdateAnimalProductAsync(long accountId, long animalProductId, Action<AnimalProduct> updater);

    Task<ProductExpense> CreateProductExpenseAsync(long accountId, long productId, string type, decimal price);

    Task<ProductExpense> UpdateProductExpensesAsync(long accountId, long productExpenseId, Action<ProductExpense> updater);

    Task<AnimalProductUnit> CreateAnimalProductUnit(long accountId, string name);

    Task<AnimalProductUnit> UpdateProductUnit(long accountId, long productUnitId, Action<AnimalProductUnit> updater);

    Task<IQueryBuilder<AnimalProductUnit>> GetAnimalProductUnit(long accountId);

    Task RemoveAnimalProductUnitAsync(long accountId, long productUnitId);

    #endregion Public Methods
}