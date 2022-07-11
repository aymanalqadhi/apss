using APSS.Application.App;
using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Tests.Utils;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Domain.ValueTypes;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class LandServiceTest
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly LandService _landService;
    private readonly IPermissionsService _permissionsSvc;

    #endregion Private fields

    #region Constructors

    public LandServiceTest(IUnitOfWork uow, IPermissionsService permissionsSvc)
        => _landService = new LandService(_uow = uow, _permissionsSvc = permissionsSvc);

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

        var addLandTask = _landService.AddLandAsync(
            account.Id,
            templateLand.Area,
            new Coordinates(templateLand.Latitude, templateLand.Longitude),
            templateLand.Address,
            templateLand.Name,
            templateLand.IsUseable,
            templateLand.IsUsed);

        if (!shouldSucceed)
        {
            Assert.ThrowsAsync<InvalidAccessLevelException>(await addLandTask);
            return (account, null);
        }

        var land = await addLandTask;

        Assert.True(_uow.Lands.Query().ContainsAsync(land));
        Assert.Equal(templateLand.OwnedBy.Id, account.User.Id);
        Assert.Equal(templateLand.Area, land.Area);
        Assert.Equal(templateLand.Name, land.Name);
        Assert.Equal(templateLand.Address, land.Address);
        Assert.Equal(templateLand.IsUsable, land.IsUsable);
        Assert.Equal(templateLand.IsUsed, land.IsUsed);

        return (account, land);
    }

    [Fact]
    public async Task LandRemovedFact()
    {
        var (account, land) = await LandAddedTheory();

        Assert.True(await _uow.Lands.Query().ContainsAsync(land));
        Assert.ThrowsAsync<InsufficientPermissionsException>(() async =>
            await _landService.RemoveLandAsync(account.Id + 1, land.Id)
        );

        await _landService.RemoveLandAsync(account.Id, land.Id);
        Assert.False(await _uow.Lands.Query().ContainsAsync(land));
    }

    #endregion Tests
}