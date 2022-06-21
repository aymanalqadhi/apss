using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

namespace APSS.Application.App;

internal class LandService : ILandService
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
        long userId,
        long area,
        double longitude,
        double latitude,
        string address,
        string name,
        bool isUsable = true,
        bool isUsed = false)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        if (user.AccessLevel != AccessLevel.Farmer)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot add lands becuase he dose not have farmer access level");
        }
        var newLand = new Land
        {
            Name = name,
            Area = area,
            Longitude = longitude,
            Latitude = latitude,
            Address = address,
            IsUsable = isUsable,
            IsUsed = isUsed,
            OwnedBy = user
        };

        _uow.Lands.Add(newLand);
        await _uow.CommitAsync();

        return newLand;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> AddLandProductAsync(
        long userId,
        long landId,
        long seasonId,
        long landProductUnitId,
        double quantity,
        double irrigationCount,
        IrrigationWaterSource irrigationWaterSource,
        IrrigationPowerSource irrigationPowerSource,
        bool isGovernmentFunded)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var land = await _uow.Lands.Query().FindAsync(landId);

        if (user.Id != land.OwnedBy.Id)
        {
            throw new InsufficientPermissionsException(userId,
                $"user #{userId} cannot add land product because he dose not own land #{landId}");
        }
        else if (user.AccessLevel != AccessLevel.Farmer)
        {
            throw new InsufficientPermissionsException(userId,
                $"user #{userId} cannot add land product because he dose not have farmer access level");
        }

        var season = await _uow.Sessions.Query().FindAsync(seasonId);
        var landproduct = new LandProduct
        {
            Quantity = quantity,
            IrrigationCount = irrigationCount,
            Producer = land,
            IrrigationPowerSource = irrigationPowerSource,
            IrrigationWaterSource = irrigationWaterSource,
            IsGovernmentFunded = isGovernmentFunded,
            ProducedIn = season,
        };

        _uow.LandProducts.Add(landproduct);
        await _uow.CommitAsync();

        return landproduct;
    }

    /// <inheritdoc/>
    public async Task<Season> AddSeasonAsync(
        long userId,
        string name,
        DateTime startsAt,
        DateTime endsAt)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        if (user.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(userId,
                $"user #{userId} cannot add season because he dose not have root access level");
        }
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
    public async Task<Land> RemoveLandAsync(long userId, long landId)
    {
        var superUser = await _uow.Users.Query().FindAsync(userId);
        var land = await _uow.Lands.Query().Include(l => l.OwnedBy).FindAsync(landId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, land.OwnedBy.Id,PermissionType.Delete)!=userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user {userId} does not have a permission to delete land #{landId}");
        }

        _uow.Lands.Remove(land);
        await _uow.CommitAsync();

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> RemoveLandProductAsync(long userId, long landProductId)
    {
        var superUser = await _uow.Users.Query().FindAsync(userId);
        var landProduct = await _uow.LandProducts
            .Query()
            .Include(p => p.Producer)
            .Include(p => p.Producer.OwnedBy)
            .FindAsync(landProductId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, landProduct.Producer.OwnedBy.Id,PermissionType.Delete)!=userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user {userId} does not have a permission to delete landProduct #{landProductId}");
        }

        _uow.LandProducts.Remove(landProduct);
        await _uow.CommitAsync();

        return landProduct;
    }

    /// <inheritdoc/>
    public async Task<Season> RemoveSeasonAsync(long userId, long seasonId)
    {
        var superUser = await _uow.Users.Query().FindAsync(userId);
        var season = await _uow.LandProducts
            .Query()
            .Include(p => p.ProducedIn)
            .Include(p => p.ProducedIn.Id == seasonId)
            .Include(o => o.Producer)
            .Include(o => o.Producer.OwnedBy)
            .FirstAsync();

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, season.Producer.OwnedBy.Id,PermissionType.Delete)!=userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user {userId} does not have a permission to delete season #{seasonId}");
        }

        var season1 = await _uow.Sessions.Query().FindAsync(seasonId);
        _uow.Sessions.Remove(season1);
        await _uow.CommitAsync();

        return season1;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<Land>> GetLand(long userId, long landId)
    {
       
        var land = await _uow.Lands.Query().Include(l => l.OwnedBy).FindAsync(landId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, land.OwnedBy.Id,PermissionType.Read)!=userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot Get land becuase he dose not have Read accessLevel on #{landId}");
        }

        return _uow.Lands.Query().Where(l=>l.Id==landId);
    }

    /// <inheritdoc/>
    public async Task <IQueryBuilder<LandProduct>>GetLandProduct(long userId, long landProductId)
    {
        var landProduct = await _uow.LandProducts.Query().Include(l => l.Producer).Include(l=>l.Producer.OwnedBy).FindAsync(landProductId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, landProduct.Producer.OwnedBy.Id, PermissionType.Read) != userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot Get landproduct becuase he dose not have Read accessLevel on #{landProductId}");
        }

        return _uow.LandProducts.Query().Where(p=>p.Id==landProductId);

    }

    /// <inheritdoc/>
    public async Task <IQueryBuilder<LandProduct>> GetLandProducts(long userId, long landId)
    {
        var land = await _uow.Lands.Query().Include(l=>l.OwnedBy).FindAsync(landId);

        if (await _permissionsSvc.ValidatePermissionsAsync(userId, land.OwnedBy.Id, PermissionType.Read) != userId)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot Get products of land#{landId} becuase he dose not have Read permission on #{landId}");
        }

        return _uow.LandProducts.Query().Where(p => p.Producer.Id == landId);
    }

    /// <inheritdoc/>
    public IQueryBuilder<Land> GetLands(long UserId)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<Land> UpdateLand(long userId, Land land)
    {
        var user = await _uow.Users.Query().FindAsync(userId);

        if (user.Id != land.OwnedBy.Id || user.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot update land becuase he dose not have Write or root accessLevel on #{land.Id} ");
        }

        return land;
    }

    /// <inheritdoc/>
    public async Task<LandProduct> UpdateLandProductAsynic(long userId, LandProduct landProduct)
    {
        var user = await _uow.Users.Query().FindAsync(userId);
        var owner = await _uow.LandProducts.Query().Include(l => l.Producer.OwnedBy).FindAsync(landProduct.Id);

        if (user.Id != owner.Producer.OwnedBy.Id || user.AccessLevel != AccessLevel.Root)
        {
            throw new InsufficientPermissionsException(
                userId,
                $"user #{userId} cannot update landPtoduct becuase he dose not have Write or root accessLevel on #{landProduct.Id} ");
        }

        return landProduct;
    }

    #endregion Public Methods
}