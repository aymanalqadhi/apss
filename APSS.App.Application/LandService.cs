using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Domain.ValueTypes;

namespace APSS.Application.App;

public class LandService : ILandService
{
    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;

    #region Public Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="uow">The unit of work of the application</param>
    /// <param name="permissionsSvc">the permissions service</param>
    public LandService(IUnitOfWork uow, IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public async Task<Land> AddLandAsync(
        long accountId,
        long area,
        Coordinates coordinates,
        string address,
        string name,
        bool isUsable = true,
        bool isUsed = false)
    {
        var account = await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);

        var newLand = new Land
        {
            Name = name,
            Area = area,
            Latitude = coordinates.Latitude,
            Longitude = coordinates.Longitude,
            Address = address,
            IsUsable = isUsable,
            IsUsed = isUsed,
            OwnedBy = account.User
        };

        _uow.Lands.Add(newLand);
        await _uow.CommitAsync();

        return newLand;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> AddLandProductAsync(
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
        string irrigationMethod)
    {
        var account = await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);
        var land = await _uow.Lands.Query().FindWithOwnershipValidationAync(landId, account);

        var product = new LandProduct
        {
            Quantity = quantity,
            IrrigationCount = irrigationCount,
            Producer = land,
            IrrigationPowerSource = irrigationPowerSource,
            IrrigationWaterSource = irrigationWaterSource,
            IsGovernmentFunded = isGovernmentFunded,
            ProducedIn = await _uow.Sessions.Query().FindAsync(seasonId),
            CropName = cropName,
            HarvestStart = harvestStart,
            HarvestEnd = HarvestEnd,
            HasGreenhouse = hasGreenhouse,
            Fertilizer = fertilizer,
            Insecticide = insecticide,
            Category = category,
            StoringMethod = storingMethod,
            IrrigationMethod = irrigationMethod
        };

        _uow.LandProducts.Add(product);
        await _uow.CommitAsync();

        return product;
    }

    /// <inheritdoc/>
    public async Task<Season> AddSeasonAsync(
        long accountId,
        string name,
        DateTime startsAt,
        DateTime endsAt)
    {
        await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Create);

        var season = new Season
        {
            Name = name,
            StartsAt = startsAt,
            EndsAt = endsAt,
        };

        _uow.Sessions.Add(season);
        await _uow.CommitAsync();

        return season;
    }

    /// <inheritdoc/>
    public async Task<Land> RemoveLandAsync(long accountId, long landId)
    {
        var land = await _uow.Lands.Query()
            .Include(l => l.OwnedBy)
            .FindAsync(landId);

        await _permissionsSvc.ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Delete);

        _uow.Lands.Remove(land);
        await _uow.CommitAsync();

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> RemoveLandProductAsync(long accountId, long landProductId)
    {
        var account = await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Delete);

        var landProduct = await _uow.LandProducts
            .Query()
            .Include(p => p.Producer)
            .Include(p => p.Producer.OwnedBy)
            .FindWithOwnershipValidationAync(landProductId, p => p.Producer.OwnedBy, account);

        _uow.LandProducts.Remove(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    /// <inheritdoc/>
    public async Task<Season> RemoveSeasonAsync(long accountId, long seasonId)
    {
        await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Delete);

        var season = await _uow.Sessions.Query().FindAsync(seasonId);

        _uow.Sessions.Remove(season);
        await _uow.CommitAsync();

        return season;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Land>> GetLandAsync(long accountId, long landId)
    {
        var land = await _uow.Lands.Query()
            .Include(l => l.OwnedBy)
            .FindAsync(landId);

        await _permissionsSvc
            .ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Read);

        return _uow.Lands.Query().Where(l => l.Id == landId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Land>> GetLandsAsync(long accountId)
    {
        var account = await _uow.Accounts
            .Query()
            .Include(u => u.User)
            .FindAsync(accountId);

        await _permissionsSvc
            .ValidatePermissionsAsync(accountId, account.User.Id, PermissionType.Read);

        return _uow.Lands.Query().Where(l => l.OwnedBy.Id == account.User.Id);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<LandProduct>> GetLandProductsAsync(long accountId, long landId)
    {
        var land = await _uow.Lands
            .Query()
            .Include(u => u.OwnedBy)
            .FindAsync(landId);

        await _permissionsSvc.ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Read);

        return _uow.LandProducts.Query().Where(p => p.Producer.Id == landId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<LandProduct>> GetLandProductAsync(long accountId, long landProductId)
    {
        var landProduct = await _uow.LandProducts
            .Query()
            .Include(l => l.Producer)
            .Include(u => u.Producer.OwnedBy)
            .FindAsync(landProductId);

        await _permissionsSvc.ValidatePermissionsAsync(accountId, landProduct.Producer.OwnedBy.Id, PermissionType.Read);

        return _uow.LandProducts.Query().Where(p => p.Id == landProductId);
    }

    /// <inheritdoc/>
    public async Task<Land> UpdateLandAsync(long accountId, Land land)
    {
        await _permissionsSvc.ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Update);

        _uow.Lands.Update(land);
        await _uow.CommitAsync();

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> UpdateLandProductAsync(long accountId, LandProduct landProduct)
    {
        await _permissionsSvc.ValidatePermissionsAsync(accountId, landProduct.Producer.OwnedBy.Id, PermissionType.Update);

        _uow.LandProducts.Update(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    public async Task<Season> UpdateSeasonAsync(long accountId, Season season)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Update);

        _uow.Sessions.Update(season);

        return season;
    }

    #endregion Public Methods
}