using APSS.Domain.Repositories;
using APSS.Domain.Services;
using System;

namespace APSS.Tests.Application.App;

public sealed class PopulationServiceTest : IDisposable
{
    #region Private fields

    private readonly IUnitOfWork _uow;
    private readonly IPopulationService _logsSvc;

    #endregion Private fields
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
