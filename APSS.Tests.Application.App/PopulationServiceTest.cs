using APSS.Application.App;
using APSS.Domain.Repositories;
using APSS.Domain.Services;
using APSS.Tests.Domain.Entities.Validators;
using APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;
using System;
using System.Threading.Tasks;
using Xunit;

namespace APSS.Tests.Application.App;

public sealed class PopulationServiceTest : IDisposable
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly IPopulationService _populationSvc;
    private readonly IPermissionsService _permissionsSvc;

    #endregion Private fields

    #region Constructors

    public PopulationServiceTest(IUnitOfWork uow,IPermissionsService permissionSvc)
        => _populationSvc = new PopulationService(_uow = uow,_permissionsSvc= permissionSvc);

    #endregion Constructors

    #region Tests

    [Fact]
    public async Task FamilyAddedFact()
    {
        using var uow = TestUnitOfWork.Create();

        var accountId = ValidEntitiesFactory.Cre
         var name 
           var livingLocation
    } 

    #endregion Tests
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
