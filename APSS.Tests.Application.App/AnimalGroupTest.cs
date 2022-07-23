using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Exceptions;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Services;
using APSS.Domain.Services.Exceptions;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using APSS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace APSS.Tests.Application.App
{
    public class AnimalGroupTest : IDisposable
    {
        #region Private Field

        private readonly IUnitOfWork _uow;
        private readonly IAnimalService _animal;

        #endregion Private Field

        #region Constructor

        public AnimalGroupTest(IUnitOfWork uow, IAnimalService animal)
        {
            _uow = uow;
            _animal = animal;
        }

        #endregion Constructor

        #region Test

        [Theory]
        [InlineData(AccessLevel.Farmer, PermissionType.Create, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
        [InlineData(AccessLevel.Group, PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Village, PermissionType.Create, false)]
        [InlineData(AccessLevel.District, PermissionType.Create, false)]
        [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
        public async Task<(Account, AnimalGroup?)> AnimalAddedTheory(
            AccessLevel accessLevel = AccessLevel.Farmer,
            PermissionType permessionType = PermissionType.Create,
            bool shouldSuccess = true)
        {
            var account = await _uow.CreateTestingAccountAsync(accessLevel, permessionType);
            var animalobj = ValidEntitiesFactory.CreateValidAnimalGroup();

            var addAnimalGroupTask = _animal.AddAnimalGroupAsync(
                account.Id,
                animalobj.Type,
                animalobj.Name,
                animalobj.Quantity,
                animalobj.Sex
                );

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await addAnimalGroupTask);
                return (account, null);
            }

            var animalGroup = await addAnimalGroupTask;

            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup));
            Assert.Equal(account.User.Id, animalGroup.OwnedBy.Id);
            Assert.Equal(animalobj.Type, animalGroup.Type);
            Assert.Equal(animalobj.Name, animalGroup.Name);
            Assert.Equal(animalobj.Quantity, animalGroup.Quantity);
            Assert.Equal(animalobj.Sex, animalGroup.Sex);

            return (account, animalGroup);
        }

        [Theory]
        [InlineData(AccessLevel.Farmer, PermissionType.Create, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
        public async Task<(Account, AnimalProduct?)> AnimalProductAddedTheory(
            AccessLevel accessLevel = AccessLevel.Farmer,
            PermissionType permissionType = PermissionType.Create,
            bool shouldSuccess = true
            )
        {
            var (animalAccount, animalGroups) = await AnimalAddedTheory();

            var productUnit = await AnimalProductUnitAddedTheory();

            var animalProduct = ValidEntitiesFactory.CreateValidAnimalProduct();
            var account = await _uow.CreateTestingAccountForUserAsync(animalAccount.User.Id, permissionType);
            var animalProductTask = _animal.AddAnimalProductAsync(
                account.Id,
                animalGroups!.Id,
                productUnit!.Id,
                animalProduct.Name,
                animalProduct.Quantity,
                animalProduct.PeriodTaken
                );

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await animalProductTask);
                return (account, null);
            }

            var product = await animalProductTask;

            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(product));
            Assert.Equal(account.User.Id, product.Producer.OwnedBy.Id);
            Assert.Equal(animalProduct.Name, product.Name);
            Assert.Equal(animalProduct.Quantity, product.Quantity);
            Assert.Equal(animalProduct.PeriodTaken, product.PeriodTaken);

            return (account, product);
        }

        [Theory]
        [InlineData(PermissionType.Delete, true)]
        [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Read, false)]
        public async Task AnimalDeleteTheory(PermissionType permissionType, bool ShouldSuccess = true)
        {
            var (account, animalGroup) = await AnimalAddedTheory(
                AccessLevel.Farmer,
                PermissionType.Create | permissionType, true
                );

            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));

            var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, permissionType);

            var DeleteAnimalTask = _animal.RemoveAnimalGroupAsync(account.Id, animalGroup!.Id);

            if (!ShouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await DeleteAnimalTask);
                return;
            }
            await DeleteAnimalTask;
            Assert.False(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup));
        }

        [Theory]
        [InlineData(PermissionType.Delete, true)]
        [InlineData(PermissionType.Update | PermissionType.Create | PermissionType.Read, false)]
        public async Task AnimalProductDeletedTheory(
            PermissionType permissionType = PermissionType.Delete, bool shouldSuccess = true)
        {
            var (account, animalProduct) = await AnimalProductAddedTheory();

            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct!));
            Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));

            var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);

            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await _animal.RemoveAnimalProductAsync(
                otherAccount.Id,
                animalProduct!.Id
                ));

            var removeAninalProductTask = _animal.RemoveAnimalProductAsync(account.Id, animalProduct!.Id);

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await removeAninalProductTask);
                return;
            }

            var bat = removeAninalProductTask;
            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct));
        }

        [Theory]
        [InlineData(AccessLevel.Root, PermissionType.Create, true)]
        [InlineData(AccessLevel.Root, PermissionType.Update | PermissionType.Delete | PermissionType.Read, false)]
        [InlineData(AccessLevel.Farmer, PermissionType.Create, false)]
        [InlineData(AccessLevel.Group, PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Village, PermissionType.Create, false)]
        [InlineData(AccessLevel.District, PermissionType.Create, false)]
        public async Task<AnimalProductUnit?> AnimalProductUnitAddedTheory(
            AccessLevel accessLevel = AccessLevel.Root,
            PermissionType permissionType = PermissionType.Create,
            bool shouldSuccess = true)
        {
            var account = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            var templateProductUnit = ValidEntitiesFactory.CreateValidAnimalProductUnit();

            var productUnitTask = _animal.CreateAnimalProductUnitAsync(account.Id, templateProductUnit.Name);

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await productUnitTask);
                return null;
            }

            var productUnit = await productUnitTask;

            Assert.True(await _uow.AnimalProductUnits.Query().ContainsAsync(productUnit));
            Assert.Equal(templateProductUnit.Name, productUnit.Name);
            return productUnit;
        }

        [Theory]
        [InlineData(AccessLevel.Group, PermissionType.Update, true)]
        [InlineData(AccessLevel.Group, PermissionType.Read | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Farmer, PermissionType.Update, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.Village, PermissionType.Update, false)]
        [InlineData(AccessLevel.District, PermissionType.Update, false)]
        [InlineData(AccessLevel.Presedint, PermissionType.Update, false)]
        public async Task AnimalGroupConfirmAddTheory(AccessLevel accessLevel = AccessLevel.Group,
            PermissionType permissionType = PermissionType.Update,
            bool shouldSuccess = true)
        {
            var (account, animalGroup) = await AnimalAddedTheory();

            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));
            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

            var groupAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            account.User.SupervisedBy = groupAccount.User;

            _uow.Accounts.Update(account);
            await _uow.CommitAsync();

            Assert.Equal(account.User.SupervisedBy.Id, groupAccount.User.Id);

            var confirmAnimalTask = _animal.ConfirmAnimalGroup(groupAccount.Id, animalGroup!.Id, true);

            var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, permissionType);

            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await _animal.ConfirmAnimalGroup(otherAccount.Id, animalGroup.Id, true));

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await confirmAnimalTask);
                return;
            }

            var confirm = await confirmAnimalTask;
            Assert.Equal(true, confirm.IsConfirmed);
        }

        [Theory]
        [InlineData(AccessLevel.Group, PermissionType.Update, true)]
        [InlineData(AccessLevel.Group, PermissionType.Read | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Farmer, PermissionType.Update, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.Village, PermissionType.Update, false)]
        [InlineData(AccessLevel.District, PermissionType.Update, false)]
        public async Task AnimalProductConfirmTheory(AccessLevel accessLevel = AccessLevel.Group, PermissionType permissionType = PermissionType.Update, bool shouldSuccess = true)
        {
            var (account, animalProduct) = await AnimalProductAddedTheory();

            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct!));
            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

            var groupAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            account.User.SupervisedBy = groupAccount.User;

            _uow.Accounts.Update(account);
            await _uow.CommitAsync();

            Assert.Equal(account.User.SupervisedBy.Id, groupAccount.User.Id);

            var confirmAnimalProductTask = _animal.ConfirmAnimalGroup(groupAccount.Id, animalProduct!.Id, true);

            var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Group, permissionType);

            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await _animal.ConfirmAnimalGroup(otherAccount.Id, animalProduct.Id, true));

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await confirmAnimalProductTask);
                return;
            }
            var confirm = await confirmAnimalProductTask;
            Assert.Equal(true, confirm.IsConfirmed);
        }

        [Theory]
        [InlineData(AccessLevel.Farmer, PermissionType.Update, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Update, false)]
        [InlineData(AccessLevel.District, PermissionType.Update, false)]
        [InlineData(AccessLevel.Village, PermissionType.Update, false)]
        [InlineData(AccessLevel.Group, PermissionType.Update, false)]
        public async Task AnimalGroupUpdatedTheory(AccessLevel accessLevel = AccessLevel.Farmer,
            PermissionType permissionType = PermissionType.Update,
            bool ShouldSuccess = true)
        {
            var (animalAccount, animalGroup) = await AnimalAddedTheory();

            Assert.True(await _uow.Accounts.Query().ContainsAsync(animalAccount));
            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));
            var account = await _uow.CreateTestingAccountForUserAsync(animalAccount.User.Id, permissionType);
            var type = RandomGenerator.NextString(RandomGenerator.NextInt(10, 15), RandomStringOptions.Alpha);
            var name = RandomGenerator.NextString(RandomGenerator.NextInt(15, 20), RandomStringOptions.Alpha);
            var quantity = RandomGenerator.NextInt(5, 10);

            var animalGroupUpdateTask = _animal.UpdateAnimalGroupAsync(account.Id, animalGroup!.Id,
                A =>
                {
                    A.Name = name;
                    A.Quantity = quantity;
                    A.Type = type;
                });

            if (!ShouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await animalGroupUpdateTask);
                return;
            }

            var otherAccount = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            var animalGroupUpdateTask2 = _animal.UpdateAnimalGroupAsync(otherAccount.Id, animalGroup!.Id,
               A =>
               {
                   A.Name = name;
                   A.Quantity = quantity;
                   A.Type = type;
               });

            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await animalGroupUpdateTask2);

            var animal = await animalGroupUpdateTask;
            Assert.Equal(type, animal.Type);
            Assert.Equal(quantity, animal.Quantity);
            Assert.Equal(animalGroup!.Id, animal.Id);
            Assert.Equal(name, animal.Name);
        }

        [Theory]
        [InlineData(AccessLevel.Farmer, PermissionType.Update, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Create, false)]
        public async Task AnimalProductUpdateTheory(
            AccessLevel accessLevel = AccessLevel.Farmer,
            PermissionType permissionType = PermissionType.Update,
            bool shouldsuccess = true)
        {
            var (animalAccount, animalProduct) = await AnimalProductAddedTheory();

            Assert.True(await _uow.Accounts.Query().ContainsAsync(animalAccount));
            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct!));

            var account = await _uow.CreateTestingAccountForUserAsync(animalAccount.User.Id, permissionType);
            var name = RandomGenerator.NextString(RandomGenerator.NextInt(10, 20), RandomStringOptions.Alpha);
            var quantity = RandomGenerator.NextInt(10, 20);

            var animalProductUpdateTask = _animal
                .UpdateAnimalProductAsync(account.Id, animalProduct!.Id,
                P =>
                {
                    P.Name = name;
                    P.Quantity = quantity;
                }
                );

            var otheraccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, permissionType);

            var otheranimalProductUpdateTask = _animal
                .UpdateAnimalProductAsync(otheraccount.Id, animalProduct!.Id,
                P =>
                {
                    P.Name = name;
                    P.Quantity = quantity;
                }
                );

            await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await
            otheranimalProductUpdateTask);

            if (!shouldsuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await animalProductUpdateTask);
                return;
            }

            var product = await animalProductUpdateTask;
            var productnew = await _uow.AnimalProducts.Query().FindAsync(product.Id);
            Assert.Equal(name, productnew.Name);
            Assert.Equal(quantity, productnew.Quantity);
        }

        [Theory]
        [InlineData(PermissionType.Update, true)]
        [InlineData(PermissionType.Create, false)]
        [InlineData(PermissionType.Read, false)]
        [InlineData(PermissionType.Delete, false)]
        public async Task AnimalProductUnitUpdatedTheory(
            PermissionType permissionType = PermissionType.Create, bool ShouldSuccess = true)
        {
            var unit = await AnimalProductUnitAddedTheory();
            var account = await _uow.CreateTestingAccountAsync(AccessLevel.Root, permissionType);

            var name = RandomGenerator.NextString(RandomGenerator.NextInt(10, 20), RandomStringOptions.Alpha);

            var updateProductUnit = _animal.UpdateProductUnit(account.Id, unit!.Id, U => U.Name = name);

            if (!ShouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await updateProductUnit);
                return;
            }

            var productUnit = await updateProductUnit;
            Assert.True(await _uow.AnimalProductUnits.Query().ContainsAsync(productUnit));
            Assert.Equal(name, productUnit.Name);
        }

        [Theory]
        [InlineData(PermissionType.Read, true)]
        [InlineData(PermissionType.Full, true)]
        [InlineData(PermissionType.Create, false)]
        [InlineData(PermissionType.Update, false)]
        [InlineData(PermissionType.Delete, false)]
        public async Task GetAnimalGroupTheory(PermissionType permissionType = PermissionType.Read, bool shouldSuccess = true)
        {
            var (account, animalGroup) = await AnimalAddedTheory();

            Assert.True(await _uow.Accounts.Query().ContainsAsync(account!));
            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));

            var superAccount = await _uow.CreateTestingAccountForUserAsync(account.User.SupervisedBy!.Id, PermissionType.Read);

            var otherFarmerAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Read);

            var otherSuper = await _uow.CreateTestingAccountForUserAsync(otherFarmerAccount.User.SupervisedBy!.Id, PermissionType.Read);
            await Assert.ThrowsAsync<InsufficientPermissionsException>(() =>
            _animal.GetAnimalGroupsAsync(otherSuper.Id, account.User.Id));

            var animalGetTask = _animal.GetAnimalGroupsAsync(superAccount.Id, account.User!.Id);

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await animalGetTask);
                return;
            }

            await animalGetTask;
        }

        [Theory]
        [InlineData(PermissionType.Read, true)]
        [InlineData(PermissionType.Full, true)]
        [InlineData(PermissionType.Create, false)]
        [InlineData(PermissionType.Update, false)]
        [InlineData(PermissionType.Delete, false)]
        public async Task GetAnimalProductTheory(PermissionType permissionType = PermissionType.Read, bool shouldSuccess = true)
        {
            var (account, animalProduct) = await AnimalProductAddedTheory();

            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct!));
            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

            var superAccount = await _uow.CreateTestingAccountForUserAsync(account.User.SupervisedBy!.Id, PermissionType.Read);

            var otherFarmerAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Read);

            var superOtherFarmerAccount = await _uow.CreateTestingAccountForUserAsync(otherFarmerAccount.User.SupervisedBy!.Id, PermissionType.Read);

            await Assert.ThrowsAsync<InvalidPermissionsExceptions>(async () => await
            _animal.RemoveAnimalProductAsync(superOtherFarmerAccount.Id, account.User.Id));

            var getProductTask = _animal.GetAnimalProductsAsync(superAccount.Id, account.User.Id);

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InsufficientPermissionsException>(() => getProductTask);
                return;
            }

            await getProductTask;
        }

        [Theory]
        [InlineData(AccessLevel.Root, PermissionType.Read, true)]
        [InlineData(AccessLevel.Root, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Read, true)]
        [InlineData(AccessLevel.Governorate, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Read, true)]
        [InlineData(AccessLevel.Directorate, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.District, PermissionType.Read, true)]
        [InlineData(AccessLevel.District, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Village, PermissionType.Read, true)]
        [InlineData(AccessLevel.Village, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Group, PermissionType.Read, true)]
        [InlineData(AccessLevel.Group, PermissionType.Update | PermissionType.Delete | PermissionType.Create, false)]
        public async Task GetAnimalProductUnitAddTheory(AccessLevel accessLevel, PermissionType permissionType = PermissionType.Read, bool shouldSuccess = true)
        {
            var productUnit = await AnimalProductUnitAddedTheory();
            var account = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            Assert.True(await _uow.AnimalProductUnits.Query().ContainsAsync(productUnit!));
            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));

            var getProductUnitTask = _animal.GetAnimalProductUnitAsync(account.Id);

            if (!shouldSuccess)
            {
                await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await getProductUnitTask);
                return;
            }

            var units = await getProductUnitTask;
        }

        public void Dispose()
       => _uow.Dispose();

        #endregion Test
    }
}