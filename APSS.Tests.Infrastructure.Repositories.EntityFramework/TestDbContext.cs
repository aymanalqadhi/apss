using APSS.Infrastructure.Repositores.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace APSS.Tests.Infrastructure.Repositories.EntityFramework;

public static class TestDbContext
{
    /// <summary>
    /// Creates an in-memory database context
    /// </summary>
    /// <returns></returns>
    public static ApssDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApssDbContext>()
            .UseInMemoryDatabase("test_apss_db")
            .ConfigureWarnings((o) => o.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApssDbContext(options);
    }
}
