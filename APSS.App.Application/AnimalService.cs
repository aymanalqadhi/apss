using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;

namespace APSS.Application.App;

public class AnimalService : IAnimalService
{
    #region Fields

    private readonly IPermissionsService _permissionsService;
    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    public AnimalService(IUnitOfWork uow, IPermissionsService permissionsService)
    {
        _uow = uow;
        _permissionsService = permissionsService;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <inheritdoc/>
    public async Task<AnimalGroup> AddAnimalGroupAsync(
        long accountId,
        string type,
        string name,
        int quantity,
        AnimalSex animalSex)
    {
        var farmer = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);

        AnimalGroup animalGroup = new AnimalGroup
        {
            Type = type,
            Name = name,
            Quantity = quantity,
            Sex = animalSex,
        };
        _uow.AnimalGroups.Add(animalGroup);
        await _uow.CommitAsync();
        return animalGroup;
    }

    /// <inheritdoc/>
    public async Task<AnimalProduct> AddAnimalProductAsync(
        long accountId,
        long animalGroupId,
        long animalProductUnitId,
        string name,
        double quantity,
        TimeSpan periodTaken)
    {
        var farmer = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId,
            AccessLevel.Farmer,
            PermissionType.Create);

        var animalGroup = await _uow.AnimalGroups.Query()
            .FindWithOwnershipValidationAync(animalGroupId, farmer);
        var unit = await _uow.AnimalProductUnits.Query().FindAsync(animalProductUnitId);

        AnimalProduct animalProduct = new AnimalProduct

        {
            Name = name,
            Quantity = quantity,
            PeriodTaken = periodTaken,
            Unit = unit,
            Producer = animalGroup,
        };

        _uow.AnimalProducts.Add(animalProduct);
        await _uow.CommitAsync();

        return animalProduct;
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<AnimalGroup>> GetAnimalGroupsAsync(long accountId, long userId)
    {
        await _permissionsService.ValidatePermissionsAsync(accountId, userId, PermissionType.Read);

        return _uow.AnimalGroups.Query().Where(i => i.OwnedBy.Id == userId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<AnimalProduct>> GetAnimalProductsAsync(
        long accountId,
        long userId,
        long animalGroupId)
    {
        await _permissionsService.ValidatePermissionsAsync(accountId, userId, PermissionType.Read);

        return _uow.AnimalProducts.Query()
            .Where(p => p.Producer.Id == animalGroupId && p.Producer.OwnedBy.Id == userId);
    }

    /// <inheritdoc/>
    public async Task RemoveAnimalGroupAsync(long accountId, long animalGroupId)
    {
        var account = await _uow.Accounts.Query()
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Delete);

        var animalgroup = await _uow.AnimalGroups.Query()
            .FindWithOwnershipValidationAync(animalGroupId, account);

        _uow.AnimalGroups.Remove(animalgroup);
        await _uow.CommitAsync();
    }

    /// <inheritdoc/>
    public async Task RemoveAnimalProductAsync(long accountId, long animalProductId)
    {
        var account = await _uow.Accounts.Query().FindWithPermissionsValidationAsync(accountId, PermissionType.Delete);

        var animalProduct = await _uow.AnimalProducts
             .Query()
             .Include(a => a.Producer)
             .Include(a => a.Producer.OwnedBy)
             .FindWithOwnershipValidationAync(animalProductId, a => a.Producer.OwnedBy, account);

        _uow.AnimalProducts.Remove(animalProduct);
        await _uow.CommitAsync();
    }

    public async Task<AnimalProduct> SetUpdate(long accountId, long animalProductId, Action<AnimalProduct> update)
    {
        var animalProduct = await _uow.AnimalProducts.Query().Include(a => a.Producer.OwnedBy).FindAsync(animalProductId);
        await _permissionsService.ValidatePermissionsAsync(accountId, animalProduct.Producer.OwnedBy.Id, PermissionType.Update);

        update(animalProduct);

        _uow.AnimalProducts.Update(animalProduct);
        await _uow.CommitAsync();
        return animalProduct;
    }

    /// <inheritdoc/>
    public async Task<AnimalGroup> UpdateAnimalGroupAsync(long accounId, long animalGroupId, Action<AnimalGroup> updater)
    {
        var account = await _uow.Accounts.Query().FindWithAccessLevelValidationAsync(accounId, AccessLevel.Farmer, PermissionType.Update);

        var animalGroup = await _uow.AnimalGroups.Query().FindWithOwnershipValidationAync(animalGroupId, a => a.OwnedBy, account);

        updater(animalGroup);

        _uow.AnimalGroups.Update(animalGroup);
        await _uow.CommitAsync();

        return animalGroup;
    }

    /// <inheritdoc/>
    public async Task<AnimalProduct> UpdateAnimalProductAsync(long accountId, long animalProductId, Action<AnimalProduct> updater)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);

        var animalProduct = await _uow.AnimalProducts.Query()
            .Include(a => a.Producer.OwnedBy)
            .FindWithOwnershipValidationAync(animalProductId, a => a.Producer.OwnedBy, account);

        updater(animalProduct);

        _uow.AnimalProducts.Update(animalProduct);
        await _uow.CommitAsync();

        return animalProduct;
    }

    #endregion Public Methods
}