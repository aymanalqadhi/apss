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
    /// <returns></returns>
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
    /// <returns></returns>
    IQueryBuilder<Land> GetLands(long UserId);

    /// <summary>
    /// Update the land
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="land">The land</param>
    /// <returns></returns>
    Task<Land> UpdateLand(long userId, Land land);

    /// <summary>
    /// Delete the land
    /// </summary>
    /// <param name="userId">The id of the farmer`s superviser</param>
    /// <returns></returns>
    Task<Land> DeleteLandAsync(long userId);

    /// <summary>
    /// Update the land product
    /// </summary>
    /// <param name="userId">The id of the user who owns the land product</param>
    /// <param name="landProduct">The id of the land product</param>
    /// <returns></returns>
    Task<LandProduct> UpdateLandProductAsynic(long userId, LandProduct landProduct);

    /// <summary>
    /// Gets the user`s land products by land
    /// </summary>
    /// <param name="userId">The id of the user who owns the land</param>
    /// <param name="landId">The id of the land</param>
    /// <returns></returns>
    IQueryBuilder<LandProduct> GetLandProducts(long userId, long landId);

    #endregion Public Methods
}