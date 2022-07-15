using System.Threading.Tasks;

using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Tests.Extensions;
using APSS.Tests.Utils;

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

            Assert.Equal(account.Id, validatedAccount.Id);
            Assert.Equal(account.Permissions, validatedAccount.Permissions);
            Assert.True(validatedAccount.Permissions.HasFlag(expectedPermissions));
        }
    }

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
    public async Task PermissionSelfValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var accessLevel = RandomGenerator.NextAccessLevel(max: AccessLevel.Presedint);

        var account = await _uow.CreateTestingAccountAsync(accessLevel, PermissionType.Full);
        var otherAccount = await _uow.CreateTestingAccountForUserAsync(
            account.User.Id,
            permissions);

        var validatePermissionsTask = _permissionsSvc.ValidatePermissionsAsync(
            otherAccount.Id,
            account.User.Id,
            expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validatePermissionsTask);
        }
        else
        {
            var validatedAccount = await validatePermissionsTask;

            Assert.Equal(otherAccount.Id, validatedAccount.Id);
            Assert.Equal(otherAccount.Permissions, validatedAccount.Permissions);
            Assert.True(validatedAccount.Permissions.HasFlag(expectedPermissions));
        }
    }

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
    public async Task PermissionsRootAccessValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(),
            PermissionType.Full);

        var rootAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Root, permissions);

        var validatePermissionsTask = _permissionsSvc.ValidatePermissionsAsync(
            rootAccount.Id,
            account.User.Id,
            expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await validatePermissionsTask);
        }
        else
        {
            var validatedAccount = await validatePermissionsTask;

            Assert.Equal(rootAccount.Id, validatedAccount.Id);
            Assert.Equal(AccessLevel.Root, validatedAccount.User.AccessLevel);
            Assert.Equal(rootAccount.Permissions, validatedAccount.Permissions);
            Assert.True(validatedAccount.Permissions.HasFlag(expectedPermissions));
        }
    }

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
    public async Task ParenthoodValidationTheory(
        PermissionType permissions,
        PermissionType expectedPermissions,
        bool shouldSucceed)
    {
        var account = await _uow.CreateTestingAccountAsync(
            RandomGenerator.NextAccessLevel(max: AccessLevel.Directorate),
            PermissionType.Full);

        var accessLevel = shouldSucceed
            ? account.User.AccessLevel.NextLevelUpove()
            : RandomGenerator.NextAccessLevel(min: account.User.AccessLevel.NextLevelUpove().NextLevelUpove());

        var superuserAccount = await _uow.CreateTestingAccountAboveUserAsync(
            account.User.Id,
            accessLevel,
            permissions);

        var userParenthoodValidationTask = _permissionsSvc.ValidateUserPatenthoodAsync(
            superuserAccount.Id,
            account.User.Id,
            expectedPermissions);

        var accountParenthoodValidationTask = _permissionsSvc.ValidateAccountPatenthoodAsync(
            superuserAccount.Id,
            account.Id,
            expectedPermissions);

        if (!shouldSucceed)
        {
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await userParenthoodValidationTask);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await accountParenthoodValidationTask);
        }
        else
        {
            var validatedAccount = await userParenthoodValidationTask;

            if (!shouldSucceed)
                Assert.True(validatedAccount.Permissions.HasFlag(PermissionType.Read));

            Assert.Equal(superuserAccount.Id, validatedAccount.Id);
            Assert.Equal(superuserAccount.Permissions, validatedAccount.Permissions);
            Assert.Equal(superuserAccount.User.AccessLevel, validatedAccount.User.AccessLevel);

            var (validatedSuperuserAccount, validatedSubuserAccount) = await accountParenthoodValidationTask;

            if (!shouldSucceed)
                Assert.True(validatedSuperuserAccount.Permissions.HasFlag(PermissionType.Read));

            Assert.Equal(validatedSuperuserAccount.Id, validatedAccount.Id);
            Assert.Equal(validatedSuperuserAccount.Permissions, validatedAccount.Permissions);
            Assert.Equal(validatedSuperuserAccount.User.AccessLevel, validatedAccount.User.AccessLevel);

            Assert.Equal(validatedSubuserAccount.Id, account.Id);
            Assert.Equal(validatedSubuserAccount.Permissions, account.Permissions);
            Assert.Equal(validatedSubuserAccount.User.AccessLevel, account.User.AccessLevel);
        }
    }

    #endregion Tests
}