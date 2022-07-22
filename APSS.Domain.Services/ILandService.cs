using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.ValueTypes;

namespace APSS.Domain.Services;

public interface ILandService
{
    #region Public Methods

    /// <summary>
    /// Asynchronously adds a new land
    /// </summary>
    /// <param name="accountId">The account id of the user to add the land for</param>
    /// <param name="area">The area of the land (in m2)</param>
    /// <param name="coordinates">The coordinates of the land</param>
    /// <param name="address">The ad physical dress of the land</param>
    /// <param name="name">The name of the land</param>
    /// <param name="isUsable">Whether the land can be used or not</param>
    /// <param name="isUsed">Whether the land is currently used or not</param>
    /// <returns>The added land information</returns>
    Task<Land> AddLandAsync(
        long accountId,
        long area,
        Coordinates coordinates,
        string address,
        string name,
        bool isUsable = true,
        bool isUsed = false);

    /// <summary>
    /// Asynchronously Gets a land by the land id
    /// </summary>
    /// <param name="accountId">The acccount id of the user who whants to show the land information</param>
    /// <param name="landId">The id of the land</param>
    /// <returns>The land that the account whants to read</returns>
    Task<IQueryBuilder<Land>> GetLandAsync(long accountId, long landId);

    /// <summary>
    /// Asynchronously Gets the lands that owned by theloged in user
    /// </summary>
    /// <param name="accountId">The account id of user who owns the land</param>
    /// <returns>the lands that owned by the user</returns>
    Task<IQueryBuilder<Land>> GetLandsAsync(long accountId);

    /// <summary>
    /// Asynchronously Updates a land
    /// </summary>
    /// <param name="accountId">The account id of the user who owns the land</param>
    /// <param name="land">The land</param>
    /// <returns>The updated land</returns>
    Task<Land> UpdateLandAsync(long accountId, long landId, Action<Land> udapter);

    /// <summary>
    /// Asynchronously removes a land by the land id
    /// </summary>
    /// <param name="accountId">The account id of the land owner`s superviser user</param>
    /// <param name="landId">The id of land</param>
    /// <returns>The removed land</returns>
    Task<Land> RemoveLandAsync(long accountId, long landId);

    /// <summary>
    /// Asynchronously Updates a land product
    /// </summary>
    /// <param name="accountId">The account id of the user who owns the land product</param>
    /// <param name="landProduct">theupdated land product</param>
    /// <returns>The updated land product</returns>
    Task<LandProduct> UpdateLandProductAsync(long accountId, long landProductId, Action<LandProduct> udapter);

    /// <summary>
    /// Asynchronously Gets the user`s land products by land id
    /// </summary>
    /// <param name="accountId">The account id of the user who owns the land</param>
    /// <param name="landId">The id of the land</param>
    /// <returns>The land products that belong to the land</returns>
    Task<IQueryBuilder<LandProduct>> GetLandProductsAsync(long accountId, long landId);

    /// <summary>
    /// Asynchronously Gets the user`s land products by land product id
    /// </summary>
    /// <param name="landProductId">The id of land product </param>
    /// <param name="accountId">The account id of the user who owns the land product</param>
    /// <returns>The land products that belong to the land</returns>
    Task<IQueryBuilder<LandProduct>> GetLandProductAsync(long accountId, long landProductId);

    /// <summary>
    /// Asynchronously removes a land product by landProduct id
    /// </summary>
    /// <param name="accountId">The account id of the landProduct owner`s superviser</param>
    /// <param name="landProductId">the id of the land product</param>
    /// <returns>The removed land product</returns>
    Task<LandProduct> RemoveLandProductAsync(long accountId, long landProductId);

    /// <summary>
    ///  Asynchronously adds a new land product
    /// </summary>
    /// <param name="accountId">The account id of the user who adds the product</param>
    /// <param name="quantity">The quantity of the product</param>
    /// <param name="irrigationCount">The count of irrigations</param>
    /// <param name="irrigationWaterSource">The suorce of the irrigation water </param>
    /// <param name="irrigationPowerSource">The power of the irrigation source</param>
    /// <param name="isGovernmentFunded">whether the land is funded by government or not</param>
    /// <returns>The added land product</returns>
    Task<LandProduct> AddLandProductAsync(
        long accountId,
        long landId,
        long seasonId,
        long landProductUnitId,
        string cropName,
        DateTime harvestStart,
        DateTime HarvestEnd,
        double quantity,
        double irrigationCount,
        IrrigationWaterSource irrigationWaterSource,
        IrrigationPowerSource irrigationPowerSource,
        bool isGovernmentFunded,
        string storingMethod,
        string category,
        bool hasGreenhouse,
        string fertilizer,
        string insecticide,
        string irrigationMethod);

    /// <summary>
    ///  Asynchronously adds a new Season
    /// </summary>
    /// <param name="accountId">The account id of the user who adds the Season</param>
    /// <param name="name">The name of the season</param>
    /// <param name="dateTimeRange">The starting date of the season and The end date of the season</param>
    /// <returns>The added season</returns>
    Task<Season> AddSeasonAsync(long accountId, string name, DateTime startsAt, DateTime endsAt);

    /// <summary>
    ///  Asynchronously removes a season by the season id
    /// </summary>
    /// <param name="accountId">The id of the account who deletes the season</param>
    /// <param name="seasonId">The id of the season</param>
    /// <returns>The removed season</returns>
    Task<Season> RemoveSeasonAsync(long accountId, long seasonId);

    /// <summary>
    /// Asynchronously Updates a season
    /// </summary>
    /// <param name="accountId"> the account id of the user who wants to update</param>
    /// <param name="season">The updated season</param>
    /// <returns>The updated season</returns>
    Task<Season> UpdateSeasonAsync(long accountId, long seasonId, Action<Season> udapter);

    /// <summary>
    ///Asynchronously  Gets a single season by the season id
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to get the season</param>
    /// <param name="seasonId">The id of the season to get</param>
    /// <returns>The query that gets the season</returns>
    Task<IQueryBuilder<Season>> GetSeasonAsync(long accountId, long seasonId);

    /// <summary>
    /// Asynchronously Gets all seasons
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to get the seasons</param>
    /// <returns>The quey that gets all seasons</returns>
    Task<IQueryBuilder<Season>> GetSeasonsAsync(long accountId);

    /// <summary>
    /// Asynchronously adds a new land product unit
    /// </summary>
    /// <param name="accountid">The account id of the user who adds the land product unit </param>
    /// <param name="name"> The name of the land product unit</param>
    /// <returns>The added land porduct unit</returns>
    Task<LandProductUnit> AddLandProductUnitAsync(long accountid, string name);

    /// <summary>
    /// Asynchronously Updates a land product unit by landProductUnit id
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to update the land product unit</param>
    /// <param name="landProductUnit">The id of the land product unit</param>
    /// <returns>The updated land product unit</returns>
    Task<LandProductUnit> UpdateLandProductUnitAsync(long accountId, long landProductUnitId, Action<LandProductUnit> udapter);

    /// <summary>
    /// Asynchronously Removes a land product unit bu the landProductUnit id
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to remove the land product unit</param>
    /// <param name="landProductUnitId">The id of the land product unit to remove</param>
    /// <returns>The removed land product unit</returns>
    Task<LandProductUnit> RemoveLandProductUnitAsync(long accountId, long landProductUnitId);

    /// <summary>
    /// Asynchronously Gets a single land produuct unit by the landProductUnit id
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to get the land product unit</param>
    /// <param name="landProductUnitId">The id of the land product unit</param>
    /// <returns>The quey that gets the land product unit</returns>
    Task<IQueryBuilder<LandProductUnit>> GetLandProductUnitAsync(long accountId, long landProductUnitId);

    /// <summary>
    /// Asynchronously Gets all land produuct units
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to get the land product units</param>
    /// <param name="landProductUnitId">The id of the land product unit</param>
    /// <returns>The quey that gets all the land product unit</returns>
    Task<IQueryBuilder<LandProductUnit>> GetLandProductUnitsAsync();

    /// <summary>
    /// Asynchronously adds a new land product expense
    /// </summary>
    /// <param name="accountId">The account id of the farmer who adds the land product expense</param>
    /// <param name="landProductId">The id of land product</param>
    /// <param name="type">The type of the expense</param>
    /// <param name="price">The price of the expense</param>
    /// <returns>The added land product expense</returns>
    Task<ProductExpense> AddLandProductExpense(
        long accountId,
        long landProductId,
        string type,
        decimal price);

    /// <summary>
    /// Asynchronously Updates a product expense
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="productExpense"></param>
    /// <returns></returns>
    Task<ProductExpense> UpdateLandProductExpenseAsync(
        long accountId,
        long landProductExpenseId,
        Action<ProductExpense> udapter);

    /// <summary>
    /// Asynchronously removes a product expense
    /// </summary>
    /// <param name="accountId">The account id of the user who wants to remove the product expense</param>
    /// <param name="productExpenseId">The id of the product expense</param>
    /// <returns>The removed product expense</returns>
    Task<ProductExpense> RemoveLandProductExpenseAsync(long accountId, long productExpenseId);

    /// <summary>
    /// Asynchronously confirms or decline land information
    /// </summary>
    /// <param name="accountId">The account id of the land owner supervisor</param>
    /// <param name="landId">The id of the land</param>
    /// <param name="confirm">The status of the land</param>
    /// <returns>The confimred or declined land</returns>
    Task<Land> ConfirmLandAsync(long accountId, long landId, bool confirm);

    /// <summary>
    /// Asynchronously confirms or decline land product information
    /// </summary>
    /// <param name="accountId">The account id of the land product owner supervisor</param>
    /// <param name="landProductId">The id of the land product</param>
    /// <param name="confirm">The status of the land</param>
    /// <returns>The confimred or declined land</returns>
    Task<LandProduct> ConfirmLandProductAsync(long accountId, long landProductId, bool confirm);

    #endregion Public Methods
}