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
        double quantity,
        double irrigationCount,
        IrrigationWaterSource irrigationWaterSource,
        IrrigationPowerSource irrigationPowerSource,
        bool isGovernmentFunded)
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
        var account = await _uow.Accounts
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
        var land = await _uow.Lands.Query().Include(l => l.OwnedBy).FindAsync(landId);
        await _permissionsSvc.ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Delete);

        _uow.Lands.Remove(land);
        await _uow.CommitAsync();

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> RemoveLandProductAsync(long accountId, long landProductId)
    {
        var landProduct = await _uow.LandProducts
            .Query()
            .Include(p => p.Producer)
            .Include(p => p.Producer.OwnedBy)
            .FindAsync(landProductId);

        await _permissionsSvc.ValidatePermissionsAsync(accountId, landProduct.Producer.OwnedBy.Id, PermissionType.Delete);

        _uow.LandProducts.Remove(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    /// <inheritdoc/>
    public async Task<Season> RemoveSeasonAsync(long accountId, long seasonId)
    {
        var account = await _uow.Accounts
            .Query()
            .Include(u => u.User)
            .FindAsync(accountId);
        if (account.User.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(
                accountId,
                $"user#{accountId} cannot get land becuase he dose not have read permissions on #{seasonId}");
        }

        var season = await _uow.Sessions.Query().FindAsync(seasonId);

        _uow.Sessions.Remove(season);
        await _uow.CommitAsync();
        return season;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Land>> GetLandAsync(long accountId, long landId)
    {
        var land = await _uow.Lands.Query().Include(l => l.OwnedBy).FindAsync(landId);

        await _permissionsSvc.ValidatePermissionsAsync(accountId, land.OwnedBy.Id, PermissionType.Read);

        return _uow.Lands.Query().Where(l => l.Id == landId);
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
    public async Task<Land> UpdateLandAsync(long userId, Land land)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var landlast = await _uow.Lands.Query().Include(l => l.OwnedBy).FindAsync(land.Id);
        if (user.Id != landlast.OwnedBy.Id || user.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot update land becuase he dose not have Write or root accessLevel on #{land.Id} ");
        }
        _uow.Lands.Update(land);
        await _uow.CommitAsync();
        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> UpdateLandProductAsync(long userId, LandProduct landProduct)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var owner = await _uow.LandProducts.Query().Include(l => l.Producer.OwnedBy).FindAsync(landProduct.Id);

        if (user.Id != owner.Producer.OwnedBy.Id || user.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot update landPtoduct becuase he dose not have Write or root accessLevel on #{landProduct.Id} ");
        }
        _uow.LandProducts.Update(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    #endregion Public Methods
}