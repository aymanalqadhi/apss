using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;


namespace APSS.Application.App;

public class AnimalService : IAnimalService
{
    #region private Properties
    private readonly IUnitOfWork _uow;
    private readonly IPermissionsService _permissionsService;

    #endregion private Properties

    #region Public constructor
    public AnimalService(IUnitOfWork uow,IPermissionsService permissionsService)
    {
        _uow = uow;
        _permissionsService = permissionsService;
    }
    #endregion Public Constructor

    #region Public Method

    /// <inheritdoc/>
    public async Task<AnimalProduct> AddAnimalProductAsync(
        long accountId,
        long animalGroupId,
        string name,
        double quantity,
        AnimalProductUnit unit,
        TimeSpan periodTaken
       
        )
    {
        var farmer = await _uow.Accounts.Query()
            .Include(u=>u.User)
            .FindWithAccessLevelValidationAsync(accountId,
            AccessLevel.Farmer,
            PermissionType.Create);

       

        
        var animal = await _uow.AnimalGroups.Query().FindWithOwnershipValidationAync(animalGroupId, farmer);

        var animalgroup = await _uow.AnimalGroups.Query().FindAsync(animalGroupId);
        AnimalProduct animalProduct = new AnimalProduct

        {
            Name = name,
            Quantity = quantity,
            Unit = unit,
            PeriodTaken = periodTaken,
            Producer = animalgroup,
            

        };

        _uow.AnimalProducts.Add(animalProduct);
        await _uow.CommitAsync();

        return animalProduct;
    }

  
    /// <inheritdoc/>
    public async Task<AnimalGroup> AddAnimalGroupAsync(
        long accountId,
        string type,
        string name,
        int quantity,
        AnimalSex animalSex
        )
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
    public async Task DeleteAnimalGroupAsync(long accountId, long animalGroupId)
    {
        
        var animalgroup=await _uow.AnimalGroups.Query().FindAsync(animalGroupId);

       await  _permissionsService.ValidatePermissionsAsync(accountId, animalgroup.OwnedBy.Id, PermissionType.Delete);

        _uow.AnimalGroups.Remove(animalgroup);
        await _uow.CommitAsync();

        
    }
    /// <inheritdoc/>
    public async Task DeleteAnimalProductAsync(long accountId, long animalProductId)
    {
        
        var account = await _uow.Accounts.Query().FindWithAccessLevelValidationAsync(accountId,AccessLevel.Farmer,PermissionType.Delete);

       
       var animalProduct = await _uow.AnimalProducts
            .Query()
            .Include(a => a.Producer)
            .Include(a => a.Producer.OwnedBy)
            .FindWithOwnershipValidationAync(animalProductId, a => a.Producer.OwnedBy, account);


        _uow.AnimalProducts.Remove(animalProduct);
        await _uow.CommitAsync();
        
    }
    /// <inheritdoc/>
    public async Task<IQueryBuilder<AnimalGroup>> GetAllAnimalGroupAsync(long accountId, long userId)
    {

        var validate = await _permissionsService.ValidatePermissionsAsync(accountId, userId, PermissionType.Read);

        return _uow.AnimalGroups.Query().Where(i => i.OwnedBy.Id == userId);
    }

    /// <inheritdoc/>
    public async Task<IQueryBuilder<AnimalGroup>> GetAnimalGroupAsync(long AccountId, long animalGroupId)
    {


        var animalgroup = await _uow.AnimalGroups.Query()
            .Include(i => i.OwnedBy)
            .FindAsync(animalGroupId);

         await _permissionsService.ValidatePermissionsAsync(AccountId, animalgroup.OwnedBy.Id, PermissionType.Read);

       

      return  _uow.AnimalGroups.Query().Where(i => i.Id==animalGroupId);
     
    }
    /// <inheritdoc/>
    public async Task<AnimalProduct> GetAnimalProductAsync(long accpountId, long animalProductId)
    {

        var animalproduct = await _uow.AnimalProducts.Query()
            .Include(a=>a.Producer.OwnedBy)
            .FindAsync(animalProductId);

        await _permissionsService.ValidatePermissionsAsync(accpountId, animalproduct.Producer.OwnedBy.Id, PermissionType.Read);


        return animalproduct;
    }
    /// <inheritdoc/>
    public async Task<AnimalGroup> UpdateAnimalGroupAsync(long accountId, AnimalGroup animalGroup)
    {

        

         await _permissionsService.ValidatePermissionsAsync(accountId, animalGroup.OwnedBy.Id, PermissionType.Update);

        
       _uow.AnimalGroups.Update(animalGroup);
        await _uow.CommitAsync();

        return animalGroup;


    }
    /// <inheritdoc/>
    public async Task<AnimalProduct> UpdateAnimalProductAsync(long accountId, AnimalProduct animalProduct)
    {


        
        await _permissionsService.ValidatePermissionsAsync(accountId, animalProduct.Producer.OwnedBy.Id,PermissionType.Update);
       


        _uow.AnimalProducts.Update(animalProduct);
        await _uow.CommitAsync();


        return animalProduct;
    }
    #endregion Public Method
}
