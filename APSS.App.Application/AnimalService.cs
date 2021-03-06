using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;

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

        AnimalGroup animalGroup = new()
        {
            Type = type,
            Name = name,
            Quantity = quantity,
            Sex = animalSex,
            OwnedBy = farmer.User,
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

        AnimalProduct animalProduct = new()
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
        var account = await _uow.Accounts.Query().Include(u => u.User)
            .FindWithPermissionsValidationAsync(accountId, PermissionType.Delete);

        var animalProduct = await _uow.AnimalProducts
             .Query()
             .Include(a => a.Producer)
             .Include(a => a.Producer.OwnedBy)
             .FindWithOwnershipValidationAync(animalProductId, a => a.Producer.OwnedBy, account);

        _uow.AnimalProducts.Remove(animalProduct);
        await _uow.CommitAsync();
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

    public async Task<ProductExpense> UpdateProductExpensesAsync(long accountId, long productExpenseId, Action<ProductExpense> updater)
    {
        var account = await _uow.Accounts.Query()
           .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);

        var productExpense = await _uow.ProductExpenses.Query()
            .Include(e => e.SpentOn.Expenses).FindAsync(productExpenseId);

        updater(productExpense);

        return productExpense;
    }

    public async Task<ProductExpense> CreateProductExpenseAsync(long accountId, long productId, string type, decimal price)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);

        var product = await _uow.AnimalProducts.Query()
            .Include(u => u.Producer.OwnedBy)
            .FindWithOwnershipValidationAync(productId, u => u.Producer.OwnedBy, account);

        var expense = new ProductExpense
        {
            Price = price,
            Type = type,
            SpentOn = product
        };

        _uow.ProductExpenses.Add(expense);
        await _uow.CommitAsync();

        return expense;
    }

    async Task<AnimalProductUnit> IAnimalService.CreateAnimalProductUnit(long accountId, string name)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);

        AnimalProductUnit animalProductUnit = new()
        {
            Name = name,
        };
        _uow.AnimalProductUnits.Add(animalProductUnit);
        await _uow.CommitAsync();

        return animalProductUnit;
    }

    public async Task<IQueryBuilder<AnimalProductUnit>> GetAnimalProductUnit(long accountId)
    {
        var account = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer
            , PermissionType.Read);

        return _uow.AnimalProductUnits.Query().Where(i => i.Id >= 0);
    }

    public async Task RemoveAnimalProductUnitAsync(long accountId, long productUnitId)
    {
        var account = await _uow.Accounts.Query().FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Delete);
        var unit = await _uow.AnimalProductUnits.Query().FindAsync(productUnitId);

        _uow.AnimalProductUnits.Remove(unit);
        await _uow.CommitAsync();
    }

    public async Task<AnimalProductUnit> UpdateProductUnit(long accountId, long productUnitId, Action<AnimalProductUnit> updater)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Update);

        var unit = await _uow.AnimalProductUnits.Query().FindAsync(productUnitId);
        updater(unit);
        return unit;
    }

    public async Task<AnimalProductUnit> CreateAnimalProductUnits(long accountId, string name)
    {
        var account = await _uow.Accounts.Query()
            .FindWithAccessLevelValidationAsync(accountId, AccessLevel.Farmer, PermissionType.Create);

        AnimalProductUnit animalProductUnit = new()
        {
            Name = name,
        };
        _uow.AnimalProductUnits.Add(animalProductUnit);
        await _uow.CommitAsync();

        return animalProductUnit;
    }

    #endregion Public Methods
}