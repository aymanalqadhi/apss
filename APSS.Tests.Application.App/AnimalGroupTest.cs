using APSS.Domain.Entities;
using APSS.Domain.Repositories;
using APSS.Domain.Repositories.Extensions;
using APSS.Domain.Repositories.Extensions.Exceptions;
using APSS.Domain.Services;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace APSS.Tests.Application.App
{
    public class AnimalGroupTest
    {
        #region Private Field

        private readonly IUnitOfWork _uow;
        private readonly IAnimalService _animal;

        #endregion Private Field

        #region Constructor
        public AnimalGroupTest(IUnitOfWork uow,IAnimalService animal )
        {
            _uow = uow;
            _animal=animal;

        }
        #endregion Constructor

        #region Public Method

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
        [InlineData(PermissionType.Delete,true)]
        [InlineData(PermissionType.Create | PermissionType.Update | PermissionType.Read, false)]
        public async Task AnimalDeleteTheory(PermissionType permissionType,bool ShouldSuccess)
        {
            var (account, animalGroup) = await AnimalAddedTheory(
                AccessLevel.Farmer,
                PermissionType.Create | permissionType, true
                );

            Assert.True(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup!));

            var otherAccount = await _uow.CreateTestingAccountAsync(AccessLevel.Farmer, PermissionType.Delete);
            await Assert.ThrowsAsync<InsufficientExecutionStackException>(() => _animal.DeleteAnimalGroupAsync(otherAccount.Id,animalGroup!.Id));

            var DeleteAnimalTask=_animal.DeleteAnimalGroupAsync(account.Id,animalGroup!.Id);

            if (!ShouldSuccess)
            {
                await Assert.ThrowsAsync<InsufficientExecutionStackException>(async () => await DeleteAnimalTask);
                return;
            }
            await DeleteAnimalTask;
            Assert.False(await _uow.AnimalGroups.Query().ContainsAsync(animalGroup));

        }

        #endregion Public Method


      

    }
}
