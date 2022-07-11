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
    [InlineData(AccessLevel.Farmer, PermissionType.Read, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Delete, false)]
    [InlineData(AccessLevel.Farmer, PermissionType.Update, false)]
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
    [InlineData(PermissionType.Delete, true)]
    [InlineData(PermissionType.Create, false)]
    [InlineData(PermissionType.Read, false)]
    [InlineData(PermissionType.Update, false)]
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

    #endregion Tests
}