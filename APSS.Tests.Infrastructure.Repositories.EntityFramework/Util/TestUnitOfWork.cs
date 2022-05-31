using APSS.Domain.Repositories;
using APSS.Infrastructure.Repositores.EntityFramework;

namespace APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;

public static class TestUnitOfWork
{
    /// <summary>
    /// Creates a testing unit of work object
    /// </summary>
    /// <returns>The created object</returns>
    public static IUnitOfWork Create()
        => new ApssUnitOfWork(TestDbContext.Create());
}