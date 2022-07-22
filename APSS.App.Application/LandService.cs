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
            OwnedBy = account.User,
            CreatedAt = DateTime.Now,
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
            IrrigationMethod = irrigationMethod,
            CreatedAt = DateTime.Now
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
            CreatedAt = DateTime.Now
        };

        _uow.Sessions.Add(season);
        await _uow.CommitAsync();

        return season;
    }

    /// <inheritdoc/>
    public async Task<ProductExpense> AddLandProductExpense(
        long accountId,
        long landProductId,
        string type,
        decimal price)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);
        var landProduct = await _uow.LandProducts.Query()
            .Include(l => l.Producer)
            .Include(u => u.Producer.OwnedBy)
            .FindAsync(landProductId);

        if (landProduct.Producer.OwnedBy.Id != account.User.Id)
        {
            throw new InsufficientPermissionsException(
                account.Id,
                $"user #{account.User.Id} with account #{account.Id} does not own #{landProductId}");
        }

        var landProductExpense = new ProductExpense
        {
            Type = type,
            Price = price,
            CreatedAt = DateTime.Now,
            SpentOn = landProduct
        };

        _uow.ProductExpenses.Add(landProductExpense);
        await _uow.CommitAsync();

        return landProductExpense;
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
    public async Task<IQueryBuilder<LandProduct>> GetLandProductAsync(
        long accountId,
        long landProductId)
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
    public async Task<IQueryBuilder<Season>> GetSeasonAsync(long accountId, long seasonId)
    {
        await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Read);

        return _uow.Sessions.Query().Where(i => i.Id == seasonId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Season>> GetSeasonsAsync(long accountId)
    {
        await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Read);

        return _uow.Sessions.Query();
    }

    /// <inheritdoc/>
    public async Task<Land> UpdateLandAsync(long accountId, long landId, Action<Land> udapter)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);
        var land = await _uow.Lands.Query()
            .Include(u => u.OwnedBy)
            .FindWithOwnershipValidationAync(landId, account);

        udapter(land);

        _uow.Lands.Update(land);
        await _uow.CommitAsync();

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> UpdateLandProductAsync(long accountId, long landProductId, Action<LandProduct> udapter)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);
        var landProduct = await _uow.LandProducts.Query()
            .FindWithOwnershipValidationAync(landProductId, u => u.Producer.OwnedBy, account);

        udapter(landProduct);

        _uow.LandProducts.Update(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    /// <inheritdoc/>
    public async Task<Season> UpdateSeasonAsync(long accountId, long seasonId, Action<Season> udapter)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Update);
        var season = await _uow.Sessions.Query()
            .FindAsync(seasonId);

        udapter(season);

        _uow.Sessions.Update(season);

        return season;
    }

    /// <inheritdoc/>
    public async Task<ProductExpense> UpdateLandProductExpenseAsync(
        long accountId,
        long landProductExpenseId,
        Action<ProductExpense> udapter)
    {
        var account = await _uow.Accounts.Query()
           .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);

        var productExpense = await _uow.ProductExpenses.Query()
            .Include(e => e.SpentOn.Expenses).FindAsync(landProductExpenseId);

        udapter(productExpense);

        _uow.ProductExpenses.Update(productExpense);
        await _uow.CommitAsync();

        return productExpense;
    }

    /// <inheritdoc/>
    public async Task<LandProductUnit> AddLandProductUnitAsync(
        long accountId,
        string name)
    {
        await _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Create);

        var landProductUnit = new LandProductUnit
        {
            Name = name,
            CreatedAt = DateTime.Now,
        };

        _uow.LandProductUnits.Add(landProductUnit);
        await _uow.CommitAsync();

        return landProductUnit;
    }

    /// <inheritdoc/>
    public async Task<LandProductUnit> UpdateLandProductUnitAsync(
        long accountId,
        long landProductUnitId,
        Action<LandProductUnit> udapter)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Update);
        var landProductUnit = await _uow.LandProductUnits.Query()
            .FindAsync(landProductUnitId);

        udapter(landProductUnit);

        _uow.LandProductUnits.Update(landProductUnit);

        return landProductUnit;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<LandProductUnit>> GetLandProductUnitAsync(
        long accountId,
        long landProductUnitId)
    {
        await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Read);

        return _uow.LandProductUnits.Query().Where(i => i.Id == landProductUnitId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<LandProductUnit>> GetLandProductUnitsAsync()
    {
        return _uow.LandProductUnits.Query();
    }

    /// <inheritdoc/>
    public async Task<LandProductUnit> RemoveLandProductUnitAsync(
        long accountId,
        long landProductUnitId)
    {
        await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Root, PermissionType.Delete);

        var landProductUnit = await _uow.LandProductUnits.Query().FindAsync(landProductUnitId);

        _uow.LandProductUnits.Remove(landProductUnit);
        await _uow.CommitAsync();

        return landProductUnit;
    }

    /// <inhertdoc/>
    public async Task<Land> ConfirmLandAsync(long accountId, long landId, bool confirm)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Group, PermissionType.Update);
        var land = await _uow.Lands.Query()
            .Include(u => u.OwnedBy)
            .FindAsync(landId);

        await _permissionsSvc.ValidateUserPatenthoodAsync(
            accountId,
            land.OwnedBy.Id,
            PermissionType.Update);

        if (confirm)
        {
            _uow.Lands.Confirm(land);
        }
        else
        {
            _uow.Lands.Decline(land);
        }

        return land;
    }

    /// <inhertdoc/>
    public async Task<LandProduct> ConfirmLandProductAsync(long accountId, long landProductId, bool confirm)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Group, PermissionType.Update);
        var landProduct = await _uow.LandProducts.Query()
            .Include(l => l.Producer)
            .Include(u => u.Producer.OwnedBy)
            .FindAsync(landProductId);

        await _permissionsSvc.ValidateUserPatenthoodAsync(
            accountId,
            landProduct.Producer.OwnedBy.Id,
            PermissionType.Update);

        if (confirm)
        {
            _uow.LandProducts.Confirm(landProduct);
        }
        else
        {
            _uow.LandProducts.Decline(landProduct);
        }

        return landProduct;
    }

    #endregion Public Methods
}