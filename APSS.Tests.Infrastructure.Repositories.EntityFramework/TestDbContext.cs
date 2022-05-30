using APSS.Infrastructure.Repositores.EntityFramework;

using Microsoft.EntityFrameworkCore;

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
            .Options;

        return new ApssDbContext(options);
    }
}
