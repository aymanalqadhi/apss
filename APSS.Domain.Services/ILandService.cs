using APSS.Domain.Entities;
using APSS.Domain.Repositories;

namespace APSS.Domain.Services;

public interface ILandService
{
    #region Public Methods

    /// <summary>
    /// Asynchronously adds a new land
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="area">The place of the land</param>
    /// <param name="longitude">The longitude of the land</param>
    /// <param name="latitude">The latitude of the land</param>
    /// <param name="address">The address of the land</param>
    /// <param name="name">The name of the land</param>
    /// <param name="isUsable">Whether the land can be used or not</param>
    /// <param name="isUsed">Whether the land is currently used or not</param>
    /// <returns>The added land information</returns>
    Task<Land> AddLandAsync(
        long userId,
        long area,
        double longitude,
        double latitude,
        string address,
        string name,
        bool isUsable = true,
        bool isUsed = false);

    /// <summary>
    /// Gets a query for the lands set
    /// </summary>
    /// <param name="UserId">The id of the user witch to get the land</param>
    /// <returns>The lands that matchs the query</returns>
    IQueryBuilder<Land> GetLands(long UserId);

    IQueryBuilder<Land> GetLand(long userId, long landId);

    /// <summary>
    /// Update the land
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="land">The land</param>
    /// <returns>The updated land</returns>
    Task<Land> UpdateLand(long userId, Land land);

    /// <summary>
    /// Delete the land
    /// </summary>
    /// <param name="userId">The id of the land owner`s superviser</param>
    /// <param name="landId">The id of land</param>
    /// <returns></returns>
    Task<Land> DeleteLandAsync(long userId, long landId);

    /// <summary>
    /// Update the land product
    /// </summary>
    /// <param name="userId">The id of the user who owns the land product</param>
    /// <param name="landProduct">The id of the land product</param>
    /// <returns>The updated land product</returns>
    Task<LandProduct> UpdateLandProductAsynic(long userId, LandProduct landProduct);

    /// <summary>
    /// Gets the user`s land products by land
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="landId">The id of the land</param>
    /// <returns>The land products that belong to the land</returns>
    IQueryBuilder<LandProduct> GetLandProducts(long userId, long landId);

    /// <summary>
    /// Gets single land product
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="landProdcutId">The id of the land product</param>
    /// <returns>The land that matchs the land id</returns>
    IQueryBuilder<LandProduct> GetLandProduct(long userId, long landProdcutId);

    /// <summary>
    /// Delete the land product
    /// </summary>
    /// <param name="userId">The id of the landProduct owner`s superviser</param>
    /// <param name="landProductId">the id of the land product</param>
    /// <returns></returns>
    Task<LandProduct> DeleteLandProductAsynic(long userId, long landProductId);

    #endregion Public Methods
}