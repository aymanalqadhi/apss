using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Tests.Extensions;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;
using APSS.Tests.Utils;

using Xunit;

namespace APSS.Tests.Domain.Repositories.Extensions;

public sealed class QueryBuilderSecurityExtensionsTest
{
    #region Fields

    private readonly IUnitOfWork _uow;

    #endregion Fields

    #region Public Constructors

    public QueryBuilderSecurityExtensionsTest()
        => _uow = TestUnitOfWork.Create();

    #endregion Public Constructors

    #region Public Methods

    [Theory]
    [InlineData(PermissionType.Full, PermissionType.Full, true)]
    [InlineData(PermissionType.Full, PermissionType.Read, true)]
    [InlineData(PermissionType.Full, PermissionType.Update, true)]
    [InlineData(PermissionType.Full, PermissionType.Create, true)]
    [InlineData(PermissionType.Full, PermissionType.Delete, true)]
    [InlineData(PermissionType.Read, PermissionType.Read, true)]
    [InlineData(PermissionType.Read, PermissionType.Update, false)]
    [InlineData(PermissionType.Read, PermissionType.Create, false)]
    [InlineData(PermissionType.Read, PermissionType.Delete, false)]
    [InlineData(PermissionType.Update, PermissionType.Update, true)]
    [InlineData(PermissionType.Update, PermissionType.Read, false)]
    [InlineData(PermissionType.Update, PermissionType.Create, false)]
    [InlineData(PermissionType.Update, PermissionType.Delete, false)]
    [InlineData(PermissionType.Create, PermissionType.Create, true)]
    [InlineData(PermissionType.Create, PermissionType.Update, false)]
    [InlineData(PermissionType.Create, PermissionType.Read, false)]
    [InlineData(PermissionType.Create, PermissionType.Delete, false)]
    [InlineData(PermissionType.Delete, PermissionType.Delete, true)]
    [InlineData(PermissionType.Delete, PermissionType.Update, false)]
    [InlineData(PermissionType.Delete, PermissionType.Create, false)]
    [InlineData(PermissionType.Delete, PermissionType.Read, false)]
    public async Task PermissionsFindWithAccessLevelValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(RandomGenerator.NextAccessLevel(), permissions);

        var findTask = _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(
                account.Id,
                account.User.AccessLevel,
                expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await findTask);
        }
        else
        {
            var foundAccount = await findTask;

            Assert.Equal(account.Id, foundAccount.Id);
            Assert.Equal(account.Permissions, foundAccount.Permissions);
            Assert.Equal(account.User.Id, foundAccount.User.Id);
            Assert.Equal(account.User.AccessLevel, foundAccount.User.AccessLevel);
        }
    }

    [Theory]
    [InlineData(AccessLevel.Farmer, AccessLevel.Farmer, true)]
    [InlineData(AccessLevel.Group, AccessLevel.Group, true)]
    [InlineData(AccessLevel.Village, AccessLevel.Village, true)]
    [InlineData(AccessLevel.District, AccessLevel.District, true)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Directorate, true)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Governorate, true)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Presedint, true)]
    [InlineData(AccessLevel.Root, AccessLevel.Root, true)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Group, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Village, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.District, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.Farmer, AccessLevel.Root, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Village, false)]
    [InlineData(AccessLevel.Group, AccessLevel.District, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.Group, AccessLevel.Root, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Group, false)]
    [InlineData(AccessLevel.Village, AccessLevel.District, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.Village, AccessLevel.Root, false)]
    [InlineData(AccessLevel.District, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.District, AccessLevel.Group, false)]
    [InlineData(AccessLevel.District, AccessLevel.Village, false)]
    [InlineData(AccessLevel.District, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.District, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.District, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.District, AccessLevel.Root, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Group, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Village, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.District, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.Directorate, AccessLevel.Root, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Group, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Village, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.District, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Presedint, false)]
    [InlineData(AccessLevel.Governorate, AccessLevel.Root, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Farmer, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Group, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Village, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.District, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Directorate, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Governorate, false)]
    [InlineData(AccessLevel.Presedint, AccessLevel.Root, false)]
    public async Task AccessLevelFindWithAccessLevelValidationTheory(
        AccessLevel accessLevel,
        AccessLevel expectedAccessLevel,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(accessLevel, PermissionType.Full);

        var findTask = _uow.Accounts
            .Query()
            .FindWithAccessLevelValidationAsync(
                account.Id,
                expectedAccessLevel,
                PermissionType.Full);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await findTask);
        }
        else
        {
            var foundAccount = await findTask;

            Assert.Equal(account.Id, foundAccount.Id);
            Assert.Equal(account.Permissions, foundAccount.Permissions);
            Assert.Equal(account.User.Id, foundAccount.User.Id);
            Assert.Equal(account.User.AccessLevel, foundAccount.User.AccessLevel);
        }
    }

    [Theory]
    [InlineData(PermissionType.Full, PermissionType.Full, true)]
    [InlineData(PermissionType.Full, PermissionType.Read, true)]
    [InlineData(PermissionType.Full, PermissionType.Update, true)]
    [InlineData(PermissionType.Full, PermissionType.Create, true)]
    [InlineData(PermissionType.Full, PermissionType.Delete, true)]
    [InlineData(PermissionType.Read, PermissionType.Read, true)]
    [InlineData(PermissionType.Read, PermissionType.Update, false)]
    [InlineData(PermissionType.Read, PermissionType.Create, false)]
    [InlineData(PermissionType.Read, PermissionType.Delete, false)]
    [InlineData(PermissionType.Update, PermissionType.Update, true)]
    [InlineData(PermissionType.Update, PermissionType.Read, false)]
    [InlineData(PermissionType.Update, PermissionType.Create, false)]
    [InlineData(PermissionType.Update, PermissionType.Delete, false)]
    [InlineData(PermissionType.Create, PermissionType.Create, true)]
    [InlineData(PermissionType.Create, PermissionType.Update, false)]
    [InlineData(PermissionType.Create, PermissionType.Read, false)]
    [InlineData(PermissionType.Create, PermissionType.Delete, false)]
    [InlineData(PermissionType.Delete, PermissionType.Delete, true)]
    [InlineData(PermissionType.Delete, PermissionType.Update, false)]
    [InlineData(PermissionType.Delete, PermissionType.Create, false)]
    [InlineData(PermissionType.Delete, PermissionType.Read, false)]
    public async Task FindWithPermissionValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(RandomGenerator.NextAccessLevel(), permissions);

        var findTask = _uow.Accounts
            .Query()
            .FindWithPermissionsValidationAsync(account.Id, expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await findTask);
        }
        else
        {
            var foundAccount = await findTask;

            Assert.Equal(account.Id, foundAccount.Id);
            Assert.Equal(account.Permissions, foundAccount.Permissions);
            Assert.Equal(account.User.Id, foundAccount.User.Id);
            Assert.Equal(account.User.AccessLevel, foundAccount.User.AccessLevel);
        }
    }

    #endregion Public Methods
}