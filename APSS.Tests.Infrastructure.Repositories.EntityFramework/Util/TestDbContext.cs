using APSS.Infrastructure.Repositores.EntityFramework;
using APSS.Tests.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace APSS.Tests.Infrastructure.Repositories.EntityFramework.Util;

public static class TestDbContext
{
    /// <summary>
    /// Creates an in-memory database context
    /// </summary>
    /// <returns></returns>
    public static ApssDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApssDbContext>()
            .UseInMemoryDatabase($"test_apss_db_{RandomGenerator.NextString(16)}")
            .ConfigureWarnings((o) => o.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new ApssDbContext(options);
    }
}