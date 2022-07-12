using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;


namespace APSS.Application.App;

public class AnimalService : IAnimalService
{

    private readonly IUnitOfWork _uow;
    private readonly IPermissionsService _permissionsService;
    public AnimalService(IUnitOfWork uow,IPermissionsService permissionsService)
    {
        _uow = uow;
        _permissionsService = permissionsService;
    }
    public async Task<AnimalProduct> AddAnimalProductAsync(
        long userId,
        long animalGroupId,
        string name,
        double quantity,
        AnimalProductUnit unit,
        TimeSpan periodTaken
       
        )
    {
        var farmer = await _uow.Accounts.Query()
            .Include(u=>u.User)
            .FindWithAccessLevelValidationAsync(userId,
            AccessLevel.Farmer,
            PermissionType.Create);
       

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

  

    public async Task<AnimalGroup> AddAnimalGroupAsync(
        long userId,
        string type,
        string name,
        int quantity,
        AnimalSex animalSex
        )
    {

        var farmer = await _uow.Accounts.Query()
            .Include(u => u.User)
            .FindWithAccessLevelValidationAsync(userId, AccessLevel.Farmer, PermissionType.Create);

    
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

    public async Task DeleteAnimalGroupAsync(long userId, long animalGroupId)
    {
        
        var animalgroup=await _uow.AnimalGroups.Query().FindAsync(animalGroupId);

       await  _permissionsService.ValidatePermissionsAsync(userId, animalgroup.OwnedBy.Id, PermissionType.Delete);

        _uow.AnimalGroups.Remove(animalgroup);
        await _uow.CommitAsync();

        
    }

    public async Task DeleteAnimalProductAsync(long userId, long animalProductId)
    {
        
        var animalProduct = await _uow.AnimalProducts.Query().FindAsync(animalProductId);

         await _permissionsService.ValidatePermissionsAsync(userId,animalProduct.Producer.OwnedBy.Id , PermissionType.Delete);

        _uow.AnimalProducts.Remove(animalProduct);
        await _uow.CommitAsync();
        
    }

    public async Task<IQueryBuilder<AnimalGroup>> GetAllAnimalGroupAsync(long accountId, long userId)
    {

        var validate = await _permissionsService.ValidatePermissionsAsync(accountId, userId, PermissionType.Read);

        return _uow.AnimalGroups.Query().Where(i => i.OwnedBy.Id == userId);
    }

  



    public async Task<IQueryBuilder<AnimalGroup>> GetAnimalGroupAsync( long AccountId, long animalGroupId)
    {


        var animalgroup = await _uow.AnimalGroups.Query().Include(i => i.OwnedBy).FindAsync(animalGroupId);

         await _permissionsService.ValidatePermissionsAsync(AccountId, animalgroup.OwnedBy.Id, PermissionType.Read);

       

      return  _uow.AnimalGroups.Query().Where(i => i.Id==animalGroupId);
     
    }

    public async Task<AnimalProduct> GetAnimalProductAsync(long farmerId, long animalProductId)
    {

        var animalproduct = await _uow.AnimalProducts.Query().FindAsync(animalProductId);
        await _permissionsService.ValidatePermissionsAsync(farmerId, animalproduct.Producer.OwnedBy.Id, PermissionType.Read);


        return animalproduct;
    }

    public async Task<AnimalGroup> UpdateAnimalGroupAsync(long userId, AnimalGroup animalGroup)
    {

         await _permissionsService.ValidatePermissionsAsync(userId, animalGroup.OwnedBy.Id, PermissionType.Update);


       _uow.AnimalGroups.Update(animalGroup);
        await _uow.CommitAsync();

        return animalGroup;


    }

    public async Task<AnimalProduct> UpdateAnimalProductAsync(long userId, AnimalProduct animalProduct)
    {
        
        await _permissionsService.ValidatePermissionsAsync(userId, animalProduct.Producer.OwnedBy.Id,PermissionType.Update);
       


        _uow.AnimalProducts.Update(animalProduct);
        await _uow.CommitAsync();


        return animalProduct;
    }
}
