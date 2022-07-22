using System.Linq;
using System.Threading.Tasks;

using APSS.Application.App;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Domain.ValueTypes;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class LandServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly ILandService _landSvc;

    #endregion Private fields

    #region Constructors

    public LandServiceTest(IUnitOfWork uow, ILandService landsSvc)
    {
        _uow = uow;
        _landSvc = landsSvc;
    }

    #endregion Constructors

    #region Tests

    [Theory]
    [InlineData(AccessLevel.Farmer, PermissionType.Create, true)]
    [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
    public async Task<(Account, Land?)> LandAddedTheory(
        AccessLevel accessLevel = AccessLevel.Farmer,
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissions);
        var templateLand = ValidEntitiesFactory.CreateValidLand();

        if (accessLevel != AccessLevel.Farmer)
        {
            var anotherAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissions);
            await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
                _landSvc.AddLandAsync(
                anotherAccount.Id,
                templateLand.Area,
                new Coordinates(templateLand.Latitude, templateLand.Longitude),
                templateLand.Address,
                templateLand.Name,
                templateLand.IsUsable,
                templateLand.IsUsed));
        }

        var addLandTask = _landSvc.AddLandAsync(
            account.Id,
            templateLand.Area,
            new Coordinates(templateLand.Latitude, templateLand.Longitude),
            templateLand.Address,
            templateLand.Name,
            templateLand.IsUsable,
            templateLand.IsUsed);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addLandTask);
            return (account, null);
        }

        var land = await addLandTask;

        Assert.True(await _uow.Lands.Query().ContainsAsync(land));
        Assert.Equal(account.User.Id, land.OwnedBy.Id);
        Assert.Equal(templateLand.Area, land.Area);
        Assert.Equal(templateLand.Name, land.Name);
        Assert.Equal(templateLand.Address, land.Address);
        Assert.Equal(templateLand.IsUsable, land.IsUsable);
        Assert.Equal(templateLand.IsUsed, land.IsUsed);

        return (account, land);
    }

    [Theory]
    [InlineData(PermissionType.Create, true)]
    [InlineData(PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task<(Account, LandProduct)> LandProductAddedTheroy(
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (account, land) = await LandAddedTheory();
        var (season, Aaccount) = await SeasonAddedTheory();
        var (AAacount, landProductUnit) = await LandProductUnitAddedTheory();

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissions);

        var templateLandProduct = ValidEntitiesFactory.CreateValidLandProduct();

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Create);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.AddLandProductAsync(
            anotherAccount.Id,
            land!.Id,
            season.Id,
            landProductUnit.Id!,
            templateLandProduct.CropName,
            templateLandProduct.HarvestStart,
            templateLandProduct.HarvestEnd,
            templateLandProduct.Quantity,
            templateLandProduct.IrrigationCount,
            templateLandProduct.IrrigationWaterSource,
            templateLandProduct.IrrigationPowerSource,
            templateLandProduct.IsGovernmentFunded,
            templateLandProduct.StoringMethod,
            templateLandProduct.Category,
            templateLandProduct.HasGreenhouse,
            templateLandProduct.Fertilizer,
            templateLandProduct.Insecticide,
            templateLandProduct.IrrigationMethod));

        var addLandProductTask = _landSvc.AddLandProductAsync(
            account.Id,
            land!.Id,
            season.Id,
            landProductUnit.Id!,
            templateLandProduct.CropName,
            templateLandProduct.HarvestStart,
            templateLandProduct.HarvestEnd,
            templateLandProduct.Quantity,
            templateLandProduct.IrrigationCount,
            templateLandProduct.IrrigationWaterSource,
            templateLandProduct.IrrigationPowerSource,
            templateLandProduct.IsGovernmentFunded,
            templateLandProduct.StoringMethod,
            templateLandProduct.Category,
            templateLandProduct.HasGreenhouse,
            templateLandProduct.Fertilizer,
            templateLandProduct.Insecticide,
            templateLandProduct.IrrigationMethod);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addLandProductTask);

            return (account, null!);
        }

        var landProduct = await addLandProductTask;

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct!));
        Assert.Equal(land.OwnedBy.Id, landProduct.Producer.OwnedBy.Id);
        Assert.Equal(season.Id, landProduct.ProducedIn.Id);
        Assert.Equal(land.Id, landProduct.Producer.Id);
        //Assert.Equal(landProductUnit.Id, landProduct.Unit.Id);
        Assert.Equal(templateLandProduct.CropName, landProduct.CropName);
        Assert.Equal(templateLandProduct.HarvestStart, landProduct.HarvestStart);
        Assert.Equal(templateLandProduct.HarvestEnd, landProduct.HarvestEnd);
        Assert.Equal(templateLandProduct.Quantity, landProduct.Quantity);
        Assert.Equal(templateLandProduct.IrrigationCount, landProduct.IrrigationCount);
        Assert.Equal(templateLandProduct.IrrigationWaterSource, landProduct.IrrigationWaterSource);
        Assert.Equal(templateLandProduct.IrrigationPowerSource, landProduct.IrrigationPowerSource);
        Assert.Equal(templateLandProduct.IsGovernmentFunded, landProduct.IsGovernmentFunded);

        return (account, landProduct);
    }

    [Theory]
    [InlineData(AccessLevel.Root, PermissionType.Create, true)]
    [InlineData(AccessLevel.Root, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
    public async Task<(Season, Account)> SeasonAddedTheory(
        AccessLevel accessLevel = AccessLevel.Root,
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissions);
        var tampletSeason = ValidEntitiesFactory.CreateValidSeason();

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Presedint, permissions);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.AddSeasonAsync(
                anotherAccount.Id,
                tampletSeason.Name,
                tampletSeason.StartsAt,
                tampletSeason.EndsAt
                ));

        var addSeasonTask = _landSvc.AddSeasonAsync(
            account.Id,
            tampletSeason.Name,
            tampletSeason.StartsAt,
            tampletSeason.EndsAt);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addSeasonTask);
            return (null!, account);
        }

        var season = await addSeasonTask;

        Assert.True(await _uow.Sessions.Query().ContainsAsync(season!));
        //Assert.Equal(tampletSeason.Id, season.Id);
        //Assert.Equal(account.User.Id, season.);   there is no attribute name for the season creator
        Assert.Equal(tampletSeason.Name, season.Name);
        Assert.Equal(tampletSeason.StartsAt, season.StartsAt);
        Assert.Equal(tampletSeason.EndsAt, season.EndsAt);

        return (season, account);
    }

    [Theory]
    [InlineData(AccessLevel.Root, PermissionType.Create, true)]
    [InlineData(AccessLevel.Root, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create, false)]
    [InlineData(AccessLevel.District, PermissionType.Create, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
    public async Task<(Account, LandProductUnit)> LandProductUnitAddedTheory(
        AccessLevel accessLevel = AccessLevel.Root,
        PermissionType permissionType = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);
        var templateLandProductUnit = ValidEntitiesFactory.CreateValidLandProductUnit();

        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Governorate, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.AddLandProductUnitAsync(anotherAccount.Id, templateLandProductUnit.Name));

        var addLandProductUnitTask = _landSvc.AddLandProductUnitAsync(
            account.Id,
            templateLandProductUnit.Name);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addLandProductUnitTask);
            return (account, null!);
        }

        var landProductUnit = await addLandProductUnitTask;

        Assert.True(await _uow.LandProductUnits.Query().ContainsAsync(landProductUnit));
        Assert.Equal(templateLandProductUnit.Name, landProductUnit.Name);
        //Assert.Equal(templateLandProductUnit.CreatedAt, landProductUnit.CreatedAt);

        return (account, landProductUnit);
    }

    [Theory]
    [InlineData(PermissionType.Create, true)]
    [InlineData(PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task<(Account, ProductExpense)> LandProductExpenseAddedTheory(
        PermissionType permissionType = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (account, landproduct) = await LandProductAddedTheroy();
        var templateLandProductExpense = ValidEntitiesFactory.CreateValidProductExpense();

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landproduct!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Create);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.AddLandProductExpense(
                anotherAccount.Id,
                landproduct.Id,
                templateLandProductExpense.Type,
                templateLandProductExpense.Price
                ));

        var addLandProductExpenseTask = _landSvc.AddLandProductExpense(
            account.Id,
            landproduct.Id,
            templateLandProductExpense.Type,
            templateLandProductExpense.Price
            );

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addLandProductExpenseTask);

            return (account, null!);
        }

        var landProductExpense = await addLandProductExpenseTask;

        Assert.True(await _uow.ProductExpenses.Query().ContainsAsync(landProductExpense));
        Assert.Equal(account.User.Id, landproduct.Producer.OwnedBy.Id);
        Assert.Equal(templateLandProductExpense.Price, landProductExpense.Price);
        Assert.Equal(templateLandProductExpense.Type, landProductExpense.Type);

        return (account, landProductExpense);
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Create | PermissionType.Read | PermissionType.Update, false)]
    public async Task LandRemovedTheory(PermissionType permissions, bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory(
            AccessLevel.Farmer,
            PermissionType.Create | permissions,
            true);

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));

        var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.RemoveLandAsync(otherAccount.Id, land!.Id)
                                                                  );

        var removeLandTask = _landSvc.RemoveLandAsync(account.Id, land!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await removeLandTask);
            return;
        }

        await removeLandTask;
        Assert.False(await _uow.Lands.Query().ContainsAsync(land));
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Read | PermissionType.Update | PermissionType.Create, false)]
    public async Task SeasonRemovedTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (season, account) = await SeasonAddedTheory();

        Assert.True(await _uow.Sessions.Query().ContainsAsync(season!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var differentAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.RemoveSeasonAsync(differentAccount.Id, season!.Id));

        var removeSeasonTask = _landSvc.RemoveSeasonAsync(account.Id, season.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeSeasonTask);
            return;
        }

        await removeSeasonTask;

        Assert.False(await _uow.Sessions.Query().ContainsAsync(season!));
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Read | PermissionType.Update | PermissionType.Create, false)]
    public async Task RemoveLandProductTheroy(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProduct) = await LandProductAddedTheroy();

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.RemoveLandProductAsync(otherAccount.Id, landProduct!.Id));

        var removeLandProductTask = _landSvc.RemoveLandProductAsync(account.Id, landProduct.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeLandProductTask);
            return;
        }

        await removeLandProductTask;
        Assert.False(await _uow.LandProducts.Query().ContainsAsync(landProduct!));
    }

    [Theory]
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Read | PermissionType.Update | PermissionType.Create, false)]
    public async Task RemoveLandProductUnitTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProductUnit) = await LandProductUnitAddedTheory();

        Assert.True(await _uow.LandProductUnits.Query().ContainsAsync(landProductUnit));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Presedint, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.RemoveLandProductUnitAsync(anotherAccount.Id, landProductUnit.Id));

        var removeLandProductUnitTask = _landSvc.RemoveLandProductUnitAsync(account.Id, landProductUnit.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeLandProductUnitTask);
            return;
        }

        var removeLlandProductUnit = await removeLandProductUnitTask;

        Assert.False(await _uow.LandProductUnits.Query().ContainsAsync(landProductUnit));
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Update, false)]
    [InlineData(PermissionType.Delete, false)]
    public async Task GetLandTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory();
        account = await _uow.CreateTestingAccountForUserAsync(
            account.User.Id,
            permissionType);
        var superviserAccount = await _uow.CreateTestingAccountForUserAsync(
            account.User.SupervisedBy!.Id,
            permissionType);

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        var otherFarmer = await _uow.CreateTestingAccountAsync(
            AccessLevel.Farmer,
            PermissionType.Create);
        var otherAccount = await _uow.CreateTestingAccountForUserAsync(
            otherFarmer.User.SupervisedBy!.Id, PermissionType.Read);
        var otheSuperviserAccount = await _uow.CreateTestingAccountForUserAsync(
            otherFarmer.User.SupervisedBy!.Id,
            PermissionType.Read);

        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.GetLandAsync(otherAccount.Id, land!.Id));
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.GetLandAsync(otheSuperviserAccount.Id, land!.Id));

        var getLandTask = _landSvc.GetLandAsync(superviserAccount.Id, land!.Id);
        var getLandTask1 = _landSvc.GetLandAsync(account.Id, land!.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandTask1);
            return;
        }

        await getLandTask;
        await getLandTask1;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Delete, false)]
    public async Task GetLandsTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory();

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        var ownerAccount = await _uow.CreateTestingAccountForUserAsync(
            account.User.Id,
            permissionType);

        var superviserAccount = await _uow.CreateTestingAccountForUserAsync(
            account.User.SupervisedBy!.Id,
            permissionType);

        var getLandsTask = _landSvc.GetLandsAsync(ownerAccount.Id);
        var getLandsTask1 = _landSvc.GetLandsAsync(superviserAccount.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandsTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandsTask1);
            return;
        }

        await getLandsTask;
        await getLandsTask1;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Delete, false)]
    public async Task GetLandProductsTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory();
        var (season, Aaccount) = await SeasonAddedTheory();
        var (AAacount, landProductUnit) = await LandProductUnitAddedTheory();
        var templateLandProduct = ValidEntitiesFactory.CreateValidLandProduct();

        var landProduct = await _landSvc.AddLandProductAsync(
            account.Id,
            land!.Id,
            season.Id,
            landProductUnit.Id!,
            templateLandProduct.CropName,
            templateLandProduct.HarvestStart,
            templateLandProduct.HarvestEnd,
            templateLandProduct.Quantity,
            templateLandProduct.IrrigationCount,
            templateLandProduct.IrrigationWaterSource,
            templateLandProduct.IrrigationPowerSource,
            templateLandProduct.IsGovernmentFunded,
            templateLandProduct.StoringMethod,
            templateLandProduct.Category,
            templateLandProduct.HasGreenhouse,
            templateLandProduct.Fertilizer,
            templateLandProduct.Insecticide,
            templateLandProduct.IrrigationMethod);

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));
        Assert.True(await _uow.Sessions.Query().ContainsAsync(season!));
        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        var superviserAccount = await _uow.CreateTestingAccountForUserAsync(
            account!.User.Id,
            permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Read);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.GetLandProductsAsync(anotherAccount.Id, land!.Id));

        var getLandProductsTask = _landSvc.GetLandProductsAsync(account.Id, land.Id);
        var getLandProductsTask1 = _landSvc.GetLandProductsAsync(superviserAccount.Id, land.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(
                async () => await getLandProductsTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(
                async () => await getLandProductsTask1);
            return;
        }
        await getLandProductsTask;
        await getLandProductsTask1;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Delete, false)]
    public async Task GetLandProductTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProduct) = await LandProductAddedTheroy();

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var superviserAccount = await _uow.CreateTestingAccountForUserAsync(
            account!.User.Id,
            permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Read);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.GetLandProductAsync(anotherAccount.Id, landProduct.Id));

        var getLandProductTask = _landSvc.GetLandProductAsync(account.Id, landProduct.Id);
        var getLandProductTask1 = _landSvc.GetLandProductAsync(superviserAccount.Id, landProduct.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandProductTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getLandProductTask1);
            return;
        }

        await getLandProductTask;
        await getLandProductTask1;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Delete, false)]
    public async Task GetSeasonTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (season, account) = await SeasonAddedTheory();

        Assert.True(await _uow.Sessions.Query().ContainsAsync(season));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Presedint, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.GetSeasonAsync(anotherAccount.Id, season!.Id));

        var getSeasonTask = _landSvc.GetSeasonAsync(account.Id, season.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await getSeasonTask);
            return;
        }

        var getSeason = await getSeasonTask;
    }

    [Theory]
    [InlineData(PermissionType.Read, true)]
    [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Delete, false)]
    public async Task GetLandProductUnitTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProductUnit) = await LandProductUnitAddedTheory();

        Assert.True(await _uow.LandProductUnits.Query().ContainsAsync(landProductUnit));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Governorate, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.GetLandProductUnitAsync(anotherAccount.Id, landProductUnit!.Id));

        var getLandProductUnitTask = _landSvc.GetLandProductUnitAsync(account.Id, landProductUnit.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await getLandProductUnitTask);
            return;
        }

        var getSeason = await getLandProductUnitTask;
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create | PermissionType.Read | PermissionType.Delete, false)]
    public async Task LandUpdatedTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory();

        Assert.True(await _uow.Lands.Query().ContainsAsync(land!));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

        var templateLand = ValidEntitiesFactory.CreateValidLand();
        templateLand.OwnedBy = account.User;

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Update);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
        _landSvc.UpdateLandAsync(anotherAccount.Id, land!.Id, l =>
        {
            l.Name = templateLand.Name;
            l.Area = templateLand.Area;
            l.Address = templateLand.Address;
            l.Longitude = templateLand.Longitude;
            l.Latitude = templateLand.Latitude;
            l.IsUsed = templateLand.IsUsed;
            l.IsUsable = templateLand.IsUsable;
        }));

        var updateLandTask = _landSvc.UpdateLandAsync(account.Id, land!.Id, l =>
        {
            l.Name = templateLand.Name;
            l.Area = templateLand.Area;
            l.Address = templateLand.Address;
            l.Longitude = templateLand.Longitude;
            l.Latitude = templateLand.Latitude;
            l.IsUsed = templateLand.IsUsed;
            l.IsUsable = templateLand.IsUsable;
        });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateLandTask);
            return;
        }

        var updateLand = await updateLandTask;
        Assert.Equal(account.User.Id, updateLand!.OwnedBy.Id);
        Assert.Equal(templateLand.Area, updateLand.Area);
        Assert.Equal(templateLand.Name, updateLand.Name);
        Assert.Equal(templateLand.Address, updateLand.Address);
        Assert.Equal(templateLand.IsUsable, updateLand.IsUsable);
        Assert.Equal(templateLand.IsUsed, updateLand.IsUsed);
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create | PermissionType.Read | PermissionType.Delete, false)]
    public async Task LandProductUpdatedTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProduct) = await LandProductAddedTheroy();
        var templateLandProduct = ValidEntitiesFactory.CreateValidLandProduct();
        templateLandProduct.Producer = landProduct.Producer;

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct));
        Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Update);
        await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _landSvc.UpdateLandProductAsync(anotherAccount.Id, landProduct.Id, l =>
            {
                l.StoringMethod = templateLandProduct.StoringMethod;
                l.Quantity = templateLandProduct.Quantity;
                l.IsGovernmentFunded = templateLandProduct.IsGovernmentFunded;
                l.IsConfirmed = templateLandProduct.IsConfirmed;
                l.IrrigationPowerSource = templateLandProduct.IrrigationPowerSource;
                l.IrrigationWaterSource = templateLandProduct.IrrigationWaterSource;
                l.Category = templateLandProduct.Category;
                l.CropName = templateLandProduct.CropName;
                l.Fertilizer = templateLandProduct.Fertilizer;
                l.HasGreenhouse = templateLandProduct.HasGreenhouse;
                l.HarvestStart = templateLandProduct.HarvestStart;
                l.HarvestEnd = templateLandProduct.HarvestEnd;
                l.Insecticide = templateLandProduct.Insecticide;
                l.IrrigationCount = templateLandProduct.IrrigationCount;
                l.IrrigationMethod = templateLandProduct.IrrigationMethod;
                l.Unit = templateLandProduct.Unit;
                l.ProducedIn = templateLandProduct.ProducedIn;
                l.Producer = templateLandProduct.Producer;
                l.Expenses = templateLandProduct.Expenses;
                l.CreatedAt = templateLandProduct.CreatedAt;
            }));

        var updateLandProductTask = _landSvc.UpdateLandProductAsync(account.Id, landProduct.Id, l =>
         {
             l.StoringMethod = templateLandProduct.StoringMethod;
             l.Quantity = templateLandProduct.Quantity;
             l.IsGovernmentFunded = templateLandProduct.IsGovernmentFunded;
             l.IsConfirmed = templateLandProduct.IsConfirmed;
             l.IrrigationPowerSource = templateLandProduct.IrrigationPowerSource;
             l.IrrigationWaterSource = templateLandProduct.IrrigationWaterSource;
             l.Category = templateLandProduct.Category;
             l.CropName = templateLandProduct.CropName;
             l.Fertilizer = templateLandProduct.Fertilizer;
             l.HasGreenhouse = templateLandProduct.HasGreenhouse;
             l.HarvestStart = templateLandProduct.HarvestStart;
             l.HarvestEnd = templateLandProduct.HarvestEnd;
             l.Insecticide = templateLandProduct.Insecticide;
             l.IrrigationCount = templateLandProduct.IrrigationCount;
             l.IrrigationMethod = templateLandProduct.IrrigationMethod;
             l.Unit = templateLandProduct.Unit;
             l.ProducedIn = templateLandProduct.ProducedIn;
             l.Producer = templateLandProduct.Producer;
             l.Expenses = templateLandProduct.Expenses;
             l.CreatedAt = templateLandProduct.CreatedAt;
         });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateLandProductTask);
            return;
        }

        var updateLandProduct = await updateLandProductTask;

        Assert.Equal(templateLandProduct.Fertilizer, updateLandProduct.Fertilizer);
        Assert.Equal(account.User.Id, updateLandProduct.Producer.OwnedBy.Id);
        Assert.Equal(templateLandProduct.Category, updateLandProduct.Category);
        Assert.Equal(templateLandProduct.CreatedAt, updateLandProduct.CreatedAt);
        Assert.Equal(templateLandProduct.CropName, updateLandProduct?.CropName);
        Assert.Equal(templateLandProduct.HarvestEnd, updateLandProduct?.HarvestEnd);
        Assert.Equal(templateLandProduct.HarvestStart, updateLandProduct?.HarvestStart);
        Assert.Equal(templateLandProduct.Unit, updateLandProduct?.Unit);
        Assert.Equal(templateLandProduct.Producer, updateLandProduct?.Producer);
        Assert.Equal(templateLandProduct.Quantity, updateLandProduct?.Quantity);
        Assert.Equal(templateLandProduct.ProducedIn, updateLandProduct?.ProducedIn);
        Assert.Equal(templateLandProduct.StoringMethod, updateLandProduct?.StoringMethod);
        Assert.Equal(templateLandProduct.Expenses, updateLandProduct?.Expenses);
        Assert.Equal(templateLandProduct.HasGreenhouse, updateLandProduct?.HasGreenhouse);
        Assert.Equal(templateLandProduct.Insecticide, updateLandProduct?.Insecticide);
        Assert.Equal(templateLandProduct.IrrigationCount, updateLandProduct?.IrrigationCount);
        Assert.Equal(templateLandProduct.IrrigationMethod, updateLandProduct?.IrrigationMethod);
        Assert.Equal(templateLandProduct.IrrigationPowerSource, updateLandProduct?.IrrigationPowerSource);
        Assert.Equal(templateLandProduct.IrrigationWaterSource, updateLandProduct?.IrrigationWaterSource);
        Assert.Equal(templateLandProduct.IsGovernmentFunded, updateLandProduct?.IsGovernmentFunded);
        Assert.Equal(templateLandProduct.IsConfirmed, updateLandProduct?.IsConfirmed);

        return;
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create | PermissionType.Read | PermissionType.Delete, false)]
    public async Task SeasonUpdatedTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (season, account) = await SeasonAddedTheory();
        var templateSeason = ValidEntitiesFactory.CreateValidSeason();

        Assert.True(await _uow.Sessions.Query().ContainsAsync(season!));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Directorate, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.UpdateSeasonAsync(anotherAccount.Id, season.Id, s =>
            {
                s.Name = templateSeason.Name;
                s.EndsAt = templateSeason.EndsAt;
                s.ModifiedAt = templateSeason.ModifiedAt;
                s.StartsAt = templateSeason.StartsAt;
            }));

        var updateSeasonTask = _landSvc.UpdateSeasonAsync(account.Id, season.Id, s =>
        {
            s.Name = templateSeason.Name;
            s.EndsAt = templateSeason.EndsAt;
            s.ModifiedAt = templateSeason.ModifiedAt;
            s.StartsAt = templateSeason.StartsAt;
        });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateSeasonTask);
            return;
        }

        var updateSeason = await updateSeasonTask;

        Assert.Equal(templateSeason.Name, updateSeason.Name);
        Assert.Equal(templateSeason.EndsAt, updateSeason.EndsAt);
        Assert.Equal(templateSeason.StartsAt, updateSeason.StartsAt);

        return;
    }

    [Theory]
    [InlineData(PermissionType.Update, true)]
    [InlineData(PermissionType.Create | PermissionType.Read | PermissionType.Delete, false)]
    public async Task LandProductUnitUpdatedTheory(
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, landProductUnit) = await LandProductUnitAddedTheory();
        var templateLandProductUnit = ValidEntitiesFactory.CreateValidLandProductUnit();

        Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));
        Assert.True(await _uow.LandProductUnits.Query().ContainsAsync(landProductUnit));

        account = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

        var anotherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.UpdateLandProductUnitAsync(anotherAccount.Id, landProductUnit.Id, u =>
            {
                u.Name = templateLandProductUnit.Name;
                u.CreatedAt = templateLandProductUnit.CreatedAt;
            }));

        var updateLandProductUnitTask = _landSvc.UpdateLandProductUnitAsync(account.Id, landProductUnit.Id, u =>
        {
            u.Name = templateLandProductUnit.Name;
            u.CreatedAt = templateLandProductUnit.CreatedAt;
        });

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateLandProductUnitTask);
            return;
        }

        var updatedLandProductUnit = await updateLandProductUnitTask;

        Assert.Equal(templateLandProductUnit.Name, updatedLandProductUnit.Name);
        Assert.Equal(templateLandProductUnit.CreatedAt, updatedLandProductUnit.CreatedAt);
    }

    #endregion Tests
}