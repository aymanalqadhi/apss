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
    /// <param name="accountId">The Id of the account to add land for</param>
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

    Task<IQueryBuilder<Land>> GetLandAsync(long accountId, long landId);

    /// <summary>
    /// Update the land
    /// </summary>
    /// <param name="accountId">The id of the account who owns the land</param>
    /// <param name="land">The land</param>
    /// <returns>The updated land</returns>
    Task<Land> UpdateLandAsync(long accountId, Land land);

    /// <summary>
    /// Delete the land
    /// </summary>
    /// <param name="accountId">The id of the land owner`s superviser</param>
    /// <param name="landId">The id of land</param>
    /// <returns></returns>
    Task<Land> RemoveLandAsync(long accountId, long landId);

    /// <summary>
    /// Update the land product
    /// </summary>
    /// <param name="accountId">The id of the user who owns the land product</param>
    /// <param name="landProduct">The id of the land product</param>
    /// <returns>The updated land product</returns>
    Task<LandProduct> UpdateLandProductAsync(long accountId, LandProduct landProduct);

    /// <summary>
    /// Gets the user`s land products by land
    /// </summary>
    /// <param name="accountId">The id of the user who owns the land</param>
    /// <param name="landId">The id of the land</param>
    /// <returns>The land products that belong to the land</returns>
    Task<IQueryBuilder<LandProduct>> GetLandProductsAsync(long accountId, long landId);

    /// <summary>
    /// Delete the land product
    /// </summary>
    /// <param name="accountId">The id of the landProduct owner`s superviser</param>
    /// <param name="landProductId">the id of the land product</param>
    /// <returns></returns>
    Task<LandProduct> RemoveLandProductAsync(long accountId, long landProductId);

    /// <summary>
    ///  Asynchronously adds a new land product
    /// </summary>
    /// <param name="accountId">The id of the account who adds the product</param>
    /// <param name="quantity">The quantity of the product</param>
    /// <param name="irrigationCount">The count of irrigations</param>
    /// <param name="irrigationWaterSource">The suorce of the irrigation water </param>
    /// <param name="irrigationPowerSource">The power of the irrigation source</param>
    /// <param name="isGovernmentFunded">whether the land is funded by government or not</param>
    /// <returns></returns>
    Task<LandProduct> AddLandProductAsync(
        long accountId,
        long landId,
        long seasonId,
        long landProductUnitId,
        double quantity,
        double irrigationCount,
        IrrigationWaterSource irrigationWaterSource,
        IrrigationPowerSource irrigationPowerSource,
        bool isGovernmentFunded);

    /// <summary>
    ///  Asynchronously adds a new Season
    /// </summary>
    /// <param name="accountId">The id of the account who adds the Season</param>
    /// <param name="name">The name of the season</param>
    /// <param name="startsAt">The starting date of the season</param>
    /// <param name="endsAt">The end date of the season</param>
    /// <returns></returns>
    Task<Season> AddSeasonAsync(long accountId, string name, DateTime startsAt, DateTime endsAt);

    /// <summary>
    ///  Asynchronously deletes the chosen season
    /// </summary>
    /// <param name="accountId">The id of the account who deletes the season</param>
    /// <param name="seasonId">The id of the season</param>
    /// <returns></returns>
    Task<Season> RemoveSeasonAsync(long accountId, long seasonId);

    #endregion Public Methods
}