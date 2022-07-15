using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Tests.Extensions;
using APSS.Tests.Utils;

using System.Threading.Tasks;

using Xunit;

namespace APSS.Tests.Application.App;

public sealed class PermissionsServiceTests
{
    #region Fields

    private readonly IPermissionsService _permissionsSvc;
    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    public PermissionsServiceTests(IUnitOfWork uow, IPermissionsService permissionsSvc)
    {
        _uow = uow;
        _permissionsSvc = permissionsSvc;
    }

    #endregion Public Constructors

    #region Tests

    [Theory]
    [InlineData(PermissionType.Create, PermissionType.Create, true)]
    [InlineData(PermissionType.Read, PermissionType.Read, true)]
    [InlineData(PermissionType.Update, PermissionType.Update, true)]
    [InlineData(PermissionType.Delete, PermissionType.Delete, true)]
    [InlineData(PermissionType.Full, PermissionType.Create, true)]
    [InlineData(PermissionType.Full, PermissionType.Read, true)]
    [InlineData(PermissionType.Full, PermissionType.Update, true)]
    [InlineData(PermissionType.Full, PermissionType.Delete, true)]
    [InlineData(PermissionType.Read, PermissionType.Full, false)]
    [InlineData(PermissionType.Update, PermissionType.Full, false)]
    [InlineData(PermissionType.Delete, PermissionType.Full, false)]
    [InlineData(PermissionType.Create, PermissionType.Update, false)]
    [InlineData(PermissionType.Create, PermissionType.Read, false)]
    [InlineData(PermissionType.Create, PermissionType.Delete, false)]
    [InlineData(PermissionType.Read, PermissionType.Create, false)]
    [InlineData(PermissionType.Read, PermissionType.Update, false)]
    [InlineData(PermissionType.Read, PermissionType.Delete, false)]
    [InlineData(PermissionType.Update, PermissionType.Create, false)]
    [InlineData(PermissionType.Update, PermissionType.Read, false)]
    [InlineData(PermissionType.Update, PermissionType.Delete, false)]
    [InlineData(PermissionType.Delete, PermissionType.Create, false)]
    [InlineData(PermissionType.Delete, PermissionType.Read, false)]
    [InlineData(PermissionType.Delete, PermissionType.Update, false)]
    public async Task PermissionsValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Presedint),
            permissions);

        var validatePermissionsTask = _permissionsSvc.ValidatePermissionsAsync(
            account.Id,
            account.User.Id,
            expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validatePermissionsTask);
        }
        else
        {
            var validatedAccount = await validatePermissionsTask;

            Assert.Equal(validatedAccount.Id, account.Id);
            Assert.Equal(validatedAccount.Permissions, account.Permissions);
            Assert.True(validatedAccount.Permissions.HasFlag(expectedPermissions));
        }
    }

    #endregion Tests
}