using APSS.Application.App;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Domain.ValueTypes;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Services.Exceptions;

using Xunit;

using System.Linq;
using System.Threading.Tasks;

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
    [InlineData(AccessLevel.Farmer, PermissionType.Create, true)]
    [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Root, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.District, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task<(Account, LandProduct)> LandProductAddedTheroy(
        AccessLevel accessLevel = AccessLevel.Farmer,
        PermissionType permissions = PermissionType.Create,
        bool shouldSucceed = true)
    {
        var (account, land) = await LandAddedTheory(
            AccessLevel.Farmer | accessLevel,
            PermissionType.Create | permissions,
            true);
        var (season, Aaccount) = await SeasonAddedTheory(
            AccessLevel.Root,
            PermissionType.Create,
            true);

        //var otherAccounts = await _uow.CreateTestingAccountAsync(accessLevel, permissions);

        var landProductUnit = ValidEntitiesFactory.CreateValidLandProductUnit();
        var templateLandProduct = ValidEntitiesFactory.CreateValidLandProduct();

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
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await addLandProductTask);

            //Assert.False(false, "shit happens");
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
    [InlineData(AccessLevel.Root, PermissionType.Create, true)]
    [InlineData(AccessLevel.Root, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.District, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task<(Season, Account)> SeasonAddedTheory(
        AccessLevel accessLevel,
        PermissionType permissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, permissions);
        var tampletSeason = ValidEntitiesFactory.CreateValidSeason();

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
    [InlineData(AccessLevel.Root, PermissionType.Delete, true)]
    [InlineData(AccessLevel.Root, PermissionType.Read | PermissionType.Create | PermissionType.Update, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Create | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.District, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task SeasonRemovedTheory(
        AccessLevel accessLevel,
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (season, account) = await SeasonAddedTheory(
            AccessLevel.Root,
            PermissionType.Create,
            true);

        Assert.True(await _uow.Sessions.Query().ContainsAsync(season!));

        //var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);
        //await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
        //    _landSvc.RemoveSeasonAsync(otherAccount.Id, season!.Id)
        //);
        var thiredAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);
        var removeSeasonTask = _landSvc.RemoveSeasonAsync(thiredAccount.Id, season.Id);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await removeSeasonTask);
            return;
        }

        await removeSeasonTask;
        Assert.False(await _uow.Sessions.Query().ContainsAsync(season!));
    }

    [Theory]
    [InlineData(AccessLevel.Farmer, PermissionType.Delete, true)]
    [InlineData(AccessLevel.Root, PermissionType.Read | PermissionType.Create | PermissionType.Update, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Create | PermissionType.Update, false)]
    [InlineData(AccessLevel.Group, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Village, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.District, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Directorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Governorate, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    [InlineData(AccessLevel.Presedint, PermissionType.Create | PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
    public async Task LandProductRemovedTheory(
        AccessLevel accessLevel,
        PermissionType permissionType,
        bool shouldSucceed)
    {
        var (account, land) = await LandAddedTheory(
             AccessLevel.Farmer,
             PermissionType.Create | permissionType,
             true);
        var (season, Aaccount) = await SeasonAddedTheory(
            AccessLevel.Root,
            PermissionType.Create,
            true);
        var landProductUnit = ValidEntitiesFactory.CreateValidLandProductUnit();
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

        Assert.True(await _uow.LandProducts.Query().ContainsAsync(landProduct!));

        var otherAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);
        await Assert.ThrowsAsync<InvalidAccessLevelException>(() =>
            _landSvc.RemoveLandProductAsync(otherAccount.Id, landProduct!.Id)
        );

        if (shouldSucceed)
        {
            var deleteLandProductTask = await _landSvc.RemoveLandProductAsync(land.OwnedBy.Id, landProduct!.Id); // The Owner has to have both create and delete permissions
            Assert.False(await _uow.LandProducts.Query().ContainsAsync(landProduct!));
        }
    }

    #endregion Tests
}