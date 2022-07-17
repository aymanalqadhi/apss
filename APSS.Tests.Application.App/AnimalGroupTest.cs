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
        [InlineData(PermissionType.Create, true)]
        [InlineData(PermissionType.Delete, false)]
        [InlineData(PermissionType.Update, false)]
        [InlineData(PermissionType.Read, false)]
        public async Task<(Account, AnimalProduct?)> AnimalProductAddedTheory(

            PermissionType permissionType = PermissionType.Create,
            bool shouldSuccess = true
            )
        {
            var (account, animalGroups) = await AnimalAddedTheory();

            var productUnit = await AnimalProductUnitAddedTheory(AccessLevel.Farmer, PermissionType.Create, true);

            var accountnew = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

            var animalProduct = ValidEntitiesFactory.CreateValidAnimalProduct();

            var animalProductTask = _animal.AddAnimalProductAsync(
                accountnew.Id,
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
            Assert.Equal(accountnew.User.Id, product.Producer.OwnedBy.Id);
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
        [InlineData(AccessLevel.Farmer, PermissionType.Create, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Update, false)]
        [InlineData(AccessLevel.Group, PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Village, PermissionType.Create, false)]
        [InlineData(AccessLevel.District, PermissionType.Create, false)]
        public async Task<AnimalProductUnit?> AnimalProductUnitAddedTheory(
            AccessLevel accessLevel = AccessLevel.Farmer,
            PermissionType permissionType = PermissionType.Create, bool shouldSuccess = true)
        {
            var account = await _uow.CreateTestingAccountAsync(accessLevel, permissionType);

            var templateProductUnit = ValidEntitiesFactory.CreateValidLandProductUnit();

            var productUnitTask = _animal.CreateAnimalProductUnits(account.Id, templateProductUnit.Name);

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
        [InlineData(AccessLevel.Farmer, PermissionType.Update, true)]
        [InlineData(AccessLevel.Farmer, PermissionType.Read | PermissionType.Delete | PermissionType.Create, false)]
        [InlineData(AccessLevel.Group, PermissionType.Create, false)]
        [InlineData(AccessLevel.Governorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Directorate, PermissionType.Create, false)]
        [InlineData(AccessLevel.Village, PermissionType.Create, false)]
        [InlineData(AccessLevel.District, PermissionType.Create, false)]
        [InlineData(AccessLevel.Presedint, PermissionType.Create, false)]
        public async Task AnimalGroupUpdatedTheoty(AccessLevel accessLevel,
            PermissionType permissionType,
            bool ShouldSuccess = true)
        {
            var (account, animalGroup) = await AnimalAddedTheory(
                 AccessLevel.Farmer,
                 PermissionType.Create | permissionType, true
                 );

            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));
            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));

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
        [InlineData(PermissionType.Create, false)]
        [InlineData(PermissionType.Update, true)]
        [InlineData(PermissionType.Delete, false)]
        [InlineData(PermissionType.Read, false)]
        public async Task AnimalProductUpdateTheory(
            PermissionType permissionType = PermissionType.Update,
            bool shouldsuccess = true)
        {
            var (account, animalProduct) = await AnimalProductAddedTheory();

            Assert.True(await _uow.Accounts.Query().ContainsAsync(account));
            Assert.True(await _uow.AnimalProducts.Query().ContainsAsync(animalProduct!));

            var newaccount = await _uow.CreateTestingAccountForUserAsync(account.User.Id, permissionType);

            var name = RandomGenerator.NextString(RandomGenerator.NextInt(10, 20), RandomStringOptions.Alpha);
            var quantity = RandomGenerator.NextInt(10, 20);

            var animalProductUpdateTask = _animal
                .UpdateAnimalProductAsync(newaccount.Id, animalProduct!.Id,
                P =>
                {
                    P.Name = name;
                    P.Quantity = quantity;
                }
                );

            if (!shouldsuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await animalProductUpdateTask);
                return;
            }

            var otheraccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, permissionType);

            var otheranimalProductUpdateTask = _animal
                .UpdateAnimalProductAsync(otheraccount.Id, animalProduct!.Id,
                P =>
                {
                    P.Name = name;
                    P.Quantity = quantity;
                }
                );

            await Assert.ThrowsAsync<InsufficientPermissionsException>(async () => await
            otheranimalProductUpdateTask);

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
            var account = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, permissionType);

            var name = RandomGenerator.NextString(RandomGenerator.NextInt(10, 20), RandomStringOptions.Alpha);

            var updateProductUnit = _animal.UpdateProductUnit(account.Id, unit!.Id, U => U.Name = name);

            if (!ShouldSuccess)
            {
                await Assert.ThrowsAsync<InvalidAccessLevelException>(async () => await updateProductUnit);
                return;
            }

            var productUnit = await updateProductUnit;
            Assert.True(await _uow.AnimalProductUnits.Query().ContainsAsync(productUnit));
            Assert.Equal(name, productUnit.Name);
        }

        public void Dispose()
       => _uow.Dispose();

        #endregion Test
    }
}